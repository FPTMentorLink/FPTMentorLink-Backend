using System.Security.Claims;
using Services.Models.Request.Authentication;
using Services.Models.Request.Authorization;
using Services.Models.Response.Authentication;
using Services.Models.Response.Authorization;
using Services.Utils;

namespace Services.Interfaces;

public interface IAuthenticationService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<string>> LoginByGoogleAsync(ClaimsPrincipal claims);
    Task<Result<LoginResponse>> AdminLoginAsync(AdminLoginRequest request);
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}