using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Repositories.Entities;
using Services.Settings;

namespace Services.Utils;

public static class TokenGenerator
{
    public static string GenerateAccessToken(JwtSettings jwtSettings, IEnumerable<Claim> claims)
    {
        var key = Encoding.ASCII.GetBytes(jwtSettings.AccessTokenSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static IEnumerable<Claim> GetClaims(Account account)
    {
        switch (account.Role)
        {
            case AccountRole.Admin:
                return new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new(ClaimTypes.Email, account.Email),
                    new(ClaimTypes.Role, account.Role.ToString())
                };
            case AccountRole.Student:
                return new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new(ClaimTypes.Email, account.Email),
                    new(ClaimTypes.Role, account.Role.ToString())
                };
            case AccountRole.Mentor:
                return new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new(ClaimTypes.Email, account.Email),
                    new(ClaimTypes.Role, account.Role.ToString())
                };
            case AccountRole.Lecturer:
                return new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new(ClaimTypes.Email, account.Email),
                    new(ClaimTypes.Role, account.Role.ToString())
                };
            default:
                return new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new(ClaimTypes.Email, account.Email),
                    new(ClaimTypes.Role, account.Role.ToString())
                };
        }
    }


    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Get principal from token with optional lifetime validation
    /// </summary>
    /// <param name="jwtSettings"></param>
    /// <param name="token"></param>
    /// <param name="validateLifetime"></param>
    /// <returns></returns>
    public static ClaimsPrincipal? GetPrincipalFromToken(JwtSettings jwtSettings, string token, bool validateLifetime = true)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),
            ValidateLifetime = validateLifetime,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero // Reduces the default 5 min clock skew
        };

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Validate token format
            if (!tokenHandler.CanReadToken(token))
                return null;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }

    /// <summary>
    /// Generate invitation token for project invitation (Expire in 24 hours)
    /// </summary>
    /// <param name="jwtSettings"></param>
    /// <param name="studentId"></param>
    /// <param name="projectId"></param>
    /// <returns></returns>
    public static string GenerateInvitationToken(JwtSettings jwtSettings, Guid studentId, Guid projectId)
    {
        var key = Encoding.ASCII.GetBytes(jwtSettings.AccessTokenSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim("StudentId", studentId.ToString()), 
                new Claim("ProjectId", projectId.ToString())
                ]),
            Expires = DateTime.UtcNow.AddMinutes(24 * 60), // Expire in 24 hours
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static bool IsTokenValid(JwtSettings jwtSettings, string token)
    {
        return GetPrincipalFromToken(jwtSettings, token) != null;
    }
}