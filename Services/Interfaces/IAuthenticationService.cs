using System.Security.Claims;
using Services.Models.Response.Authentication;
using Services.Utils;

namespace Services.Interfaces;

public interface IAuthenticationService
{
    Task<Result<LoginResponse>> LoginAsync(ClaimsPrincipal claims);
}