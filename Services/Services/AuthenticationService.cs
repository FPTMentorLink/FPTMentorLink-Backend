using System.Security.Claims;
using Mapster;
using Microsoft.Extensions.Options;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Response.Authentication;
using Services.Utils;

namespace Services.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationService(IUnitOfWork unitOfWork,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings.Value;
    }


    public async Task<Result<LoginResponse>> LoginAsync(ClaimsPrincipal claims)
    {
        var email = claims.FindFirst(x => x.Type == ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Result.Failure<LoginResponse>(DomainError.Account.AccountNotFound);
        }

        var isFptEmail = email.EndsWith("@fpt.edu.vn") || email.EndsWith("@fe.edu.vn");
        if (!isFptEmail)
        {
            return Result.Failure<LoginResponse>(DomainError.Account.InvalidFptEmail);
        }

        var user = await _unitOfWork.Accounts.FindSingleAsync(x => x.Email == email);
        if (user == null)
        {
            return Result.Failure<LoginResponse>(DomainError.Account.AccountNotFound);
        }

        var userClaims = TokenGenerator.GetClaims(user);
        var token = TokenGenerator.GenerateAccessToken(_jwtSettings, userClaims);
        var refreshToken = TokenGenerator.GenerateRefreshToken();
        var loginResponse = user.Adapt<LoginResponse>();
        loginResponse.AccessToken = token;
        loginResponse.RefreshToken = refreshToken;
        return Result.Success(loginResponse);
    }
}