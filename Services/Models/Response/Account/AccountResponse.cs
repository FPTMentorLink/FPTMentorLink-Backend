using Repositories.Entities;

namespace Services.Models.Response.Account;

public class AccountResponse
{
    public AccountRole Role { get; set; }
    public object? Data { get; set; }
}

public class AccountsResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    public AccountRole Role { get; set; }
}