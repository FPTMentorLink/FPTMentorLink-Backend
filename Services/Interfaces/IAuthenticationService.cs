using System.Security.Claims;
using Services.Models.Request.Authentication;
using Services.Models.Response.Authentication;
using Services.Utils;

namespace Services.Interfaces;

public interface IAuthenticationService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<string>> LoginByGoogleAsync(ClaimsPrincipal claims);
    Task<Result<LoginResponse>> AdminLoginAsync(AdminLoginRequest request);
}