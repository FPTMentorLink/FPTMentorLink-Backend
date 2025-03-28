using Repositories.Entities;
using Services.Models.Request.Notification;
using Services.Models.Response.Notification;
using Services.Utils;

namespace Services.Interfaces;

public interface INotificationService
{
    Task<Result<NotificationResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<NotificationResponse>>> GetPagedAsync(GetNotificationsRequest request);
    Task<Result<PaginationResult<NotificationResponse>>> GetMyNotificationsAsync(GetNotificationsRequest request, Guid userId, AccountRole role);
    Task<Result> CreateAsync(CreateNotificationRequest request);
    Task<Result> DeleteAsync(Guid id);
}