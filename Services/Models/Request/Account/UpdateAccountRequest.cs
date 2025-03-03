using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.Account;

public class UpdateAccountRequest : ValidatableObject
{
    [MaxLength(255)] public string? FirstName { get; set; }
    [MaxLength(255)] public string? LastName { get; set; }
    [MaxLength(255)] public string? ImageUrl { get; set; }
    public bool? IsSuspended { get; set; }
}