using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Notification;

public class NotificationResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; } = null!;
    public string Title { get; set; } = null!;
    public AccountRole[] Roles { get; set; } = null!;
}