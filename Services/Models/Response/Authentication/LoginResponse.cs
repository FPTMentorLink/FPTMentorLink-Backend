using Repositories.Entities;

namespace Services.Models.Response.Authentication;

public class LoginResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public AccountRole Role { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}