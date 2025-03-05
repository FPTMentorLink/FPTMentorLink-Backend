using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Account;

public abstract class BaseAccountResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    public AccountRole Role { get; set; }
}

public class AdminResponse : BaseAccountResponse
{
}