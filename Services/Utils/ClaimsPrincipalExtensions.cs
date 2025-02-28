using System.Security.Claims;

namespace Services.Utils;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal? principal)
    {
        return principal != null && principal.IsInRole("Admin");
    }

    public static Guid? GetUserId(this ClaimsPrincipal? principal)
    {
        var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
    }
    
    public static Guid? GetGuid(this ClaimsPrincipal? principal, string claimType)
    {
        var claim = principal?.FindFirst(claimType)?.Value;
        return claim != null ? Guid.Parse(claim) : null;
    }

    public static string? GetUserEmail(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.Email)?.Value;
    }
} 