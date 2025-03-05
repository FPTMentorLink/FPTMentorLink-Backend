using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.Account;

public abstract class BaseUpdateAccountRequest
{
    [MaxLength(255)] public string? FirstName { get; set; }
    [MaxLength(255)] public string? LastName { get; set; }
    [MaxLength(255)] public string? ImageUrl { get; set; }
    public bool? IsSuspended { get; set; } = false;
}