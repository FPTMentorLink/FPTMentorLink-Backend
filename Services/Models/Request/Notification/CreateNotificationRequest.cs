using System.ComponentModel.DataAnnotations;
using Repositories.Entities;

namespace Services.Models.Request.Notification;

public class CreateNotificationRequest
{
    [Required] public string Content { get; set; } = null!;
    [Required] public string Title { get; set; } = null!;
    [Required] public AccountRole[] Roles { get; set; } = null!;
}