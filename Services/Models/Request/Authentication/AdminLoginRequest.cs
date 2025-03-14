using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.Authentication;

public class AdminLoginRequest
{
    [Required] [MaxLength(255)] public string Username { get; set; }
    [Required] [MaxLength(255)] public string Password { get; set; }
}