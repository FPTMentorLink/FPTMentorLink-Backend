using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Authentication;
using Services.Models.Request.Authorization;
using Services.Models.Response.Authentication;
using Services.Models.Response.Authorization;
using Services.Settings;
using Services.Utils;

namespace Services.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;
    private readonly RedirectUrlSettings _redirectUrlSettings;
    private readonly IRedisService _redis;

    public AuthenticationService(
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> jwtSettings,
        IOptions<RedirectUrlSettings> redirectUrlSettings, IRedisService redis)
    {
        _unitOfWork = unitOfWork;
        _redis = redis;
        _jwtSettings = jwtSettings.Value;
        _redirectUrlSettings = redirectUrlSettings.Value;
    }


    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var data = await _redis.GetCacheAsync(request.ContentHash);
        if (data == null)
        {
            return Result.Failure<LoginResponse>("Token not found or expire");
        }

        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(data);
        if (loginResponse == null)
        {
            return Result.Failure<LoginResponse>("Token not found or expire");
        }

        await _redis.DeleteCacheAsync(request.ContentHash);

        return Result.Success(loginResponse);
    }

    public async Task<Result<string>> LoginByGoogleAsync(ClaimsPrincipal claims)
    {
        var email = claims.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Result.Failure<string>(DomainError.Account.AccountNotFound);
        }

        var user = await _unitOfWork.Accounts.FindSingleAsync(x => x.Email == email);
        if (user == null)
        {
            return Result.Failure<string>(DomainError.Account.AccountNotFound);
        }

        // Generate login response
        var userClaims = TokenGenerator.GetClaims(user);
        var accessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, userClaims);
        var refreshToken = TokenGenerator.GenerateRefreshToken();
        // var loginResponse = user.Adapt<LoginResponse>();
        var loginResponse = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        var jsonOption = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var data = JsonSerializer.Serialize(loginResponse, jsonOption);
        var contentHash = HmacHelper.GenerateSignature(_redirectUrlSettings.Key, data);
        // Store payload in Redis with an expiration of 5 minutes
        await _redis.SetCacheAsync(contentHash, data, TimeSpan.FromMinutes(_redirectUrlSettings.TimeOut), false);
        // Redirect to FE with key in Redis
        var redirectUrl = _redirectUrlSettings.PostGoogleLoginUrl + contentHash;
        return Result.Success(redirectUrl);
    }

    public async Task<Result<LoginResponse>> AdminLoginAsync(AdminLoginRequest request)
    {
        var user = await _unitOfWork.Accounts.FindSingleAsync(x =>
            x.Username == request.Username && x.Role == AccountRole.Admin);
        if (user == null)
        {
            return Result.Failure<LoginResponse>(DomainError.Authentication.InvalidCredentials);
        }

        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
        {
            return Result.Failure<LoginResponse>(DomainError.Authentication.InvalidCredentials);
        }

        var userClaims = TokenGenerator.GetClaims(user);
        var accessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, userClaims);
        var refreshToken = TokenGenerator.GenerateRefreshToken();
        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Check if access token is valid when it is expired
        var principal = TokenGenerator.GetPrincipalFromToken(_jwtSettings, request.AccessToken);
        if (principal == null)
            return Result.Failure<RefreshTokenResponse>("Invalid token");

        var userId = principal.GetUserId();
        if (userId == null)
            return Result.Failure<RefreshTokenResponse>("User not found");

        var claims = principal.Claims.ToList();
        var newAccessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, claims);
        var newRefreshToken = TokenGenerator.GenerateRefreshToken();

        // TODO: implement refresh token invalidation and database update logic
        await Task.Delay(10);

        return Result.Success(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}