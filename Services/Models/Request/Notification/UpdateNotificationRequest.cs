using System.ComponentModel.DataAnnotations;
using Repositories.Entities;

namespace Services.Models.Request.Notification;

public class UpdateNotificationRequest
{
    [Required] public Guid Id { get; set; }
    public string? Content { get; set; } = null!;
    public string? Title { get; set; } = null!;
    public AccountRole[]? Roles { get; set; } = null!;
}