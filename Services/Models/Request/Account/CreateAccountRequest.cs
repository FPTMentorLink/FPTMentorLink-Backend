using Repositories.Entities;

namespace Services.Models.Request.Account;

public class CreateAccountRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    public AccountRole Role { get; set; }
}