using System.Security.Claims;

namespace Services.Utils;

public static class ClaimsPrincipalExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal? user)
    {
        return user != null && user.IsInRole("Admin");
    }

    public static Guid? GetUserId(this ClaimsPrincipal? user)
    {
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
    }

    public static string? GetUserEmail(this ClaimsPrincipal? user)
    {
        return user?.FindFirst(ClaimTypes.Email)?.Value;
    }
} 