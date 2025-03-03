using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Account;

public class CreateAccountRequest : ValidatableObject
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; set; }

    [Required] [MaxLength(255)] public required string Username { get; set; }
    [Required] [MaxLength(255)] public required string Password { get; set; }
    [Required] [MaxLength(255)] public required string FirstName { get; set; }
    [Required] [MaxLength(255)] public required string LastName { get; set; }
    [MaxLength(255)] public string? ImageUrl { get; set; }
    public bool IsSuspended { get; set; }
    [Range(1,4)]
    public AccountRole Role { get; set; }
}