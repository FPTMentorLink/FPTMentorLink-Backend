using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Account;

public class AccountResponse : AuditableResponse
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    public AccountRole Role { get; set; } 
}