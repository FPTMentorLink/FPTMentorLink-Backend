using System.Security.Claims;
using System.Text.Json;
using Mapster;
using Microsoft.Extensions.Options;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Authentication;
using Services.Models.Response.Authentication;
using Services.Utils;
using Services.Utils.Hmac;

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

        var isFptEmail = email.EndsWith("@fpt.edu.vn") || email.EndsWith("@fe.edu.vn");
        if (!isFptEmail)
        {
            return Result.Failure<string>(DomainError.Account.InvalidFptEmail);
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
        var loginResponse = user.Adapt<LoginResponse>();
        loginResponse.AccessToken = accessToken;
        loginResponse.RefreshToken = refreshToken;

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
}