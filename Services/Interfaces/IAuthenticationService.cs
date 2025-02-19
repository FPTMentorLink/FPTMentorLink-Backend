using System.Security.Claims;
using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IAuthenticationService
{
    Task<Result<LoginResponse>> LoginAsync(ClaimsPrincipal claims);
}