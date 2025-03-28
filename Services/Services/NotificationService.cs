using System.Linq.Expressions;
using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Notification;
using Services.Models.Response.Notification;
using Services.Utils;

namespace Services.Services;

public class NotificationService : INotificationService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<NotificationResponse>> GetByIdAsync(Guid id)
    {
        var notification = await _unitOfWork.Notifications.FindByIdAsync(id);
        if (notification == null)
            return Result.Failure<NotificationResponse>("Notification not found");

        return Result.Success(_mapper.Map<NotificationResponse>(notification));
    }

    public async Task<Result<PaginationResult<NotificationResponse>>> GetPagedAsync(GetNotificationsRequest request)
    {
        var query = _unitOfWork.Notifications.GetQueryable();
        Expression<Func<Notification, bool>> condition = x => true;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            condition = condition.CombineAndAlsoExpressions(x =>
                x.Title.Contains(searchTerm) ||
                x.Content.Contains(searchTerm));
        }

        if (request.FromDate.HasValue)
            condition = condition.CombineAndAlsoExpressions(x => x.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            condition = condition.CombineAndAlsoExpressions(x => x.CreatedAt <= request.ToDate.Value);

        query = query.Where(condition);

        // Order by creation date descending
        query = query.OrderByDescending(x => x.CreatedAt);

        var result = await query.ProjectToPaginatedListAsync<Notification, NotificationResponse>(request);

        return Result.Success(result);
    }

    public async Task<Result<PaginationResult<NotificationResponse>>> GetMyNotificationsAsync(
        GetNotificationsRequest request, Guid userId, AccountRole role)
    {
        var query = _unitOfWork.Notifications.GetQueryable()
            .Where(x => x.Roles.Contains(role));
        
        Expression<Func<Notification, bool>> condition = x => true;

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            condition = condition.CombineAndAlsoExpressions(x =>
                x.Title.ToLower().Contains(searchTerm) ||
                x.Content.ToLower().Contains(searchTerm));
        }

        if (request.FromDate.HasValue)
            condition = condition.CombineAndAlsoExpressions(x => x.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            condition = condition.CombineAndAlsoExpressions(x => x.CreatedAt <= request.ToDate.Value);

        query = query.Where(condition);

        // Order by creation date descending
        query = query.OrderByDescending(x => x.CreatedAt);

        var result = await query.ProjectToPaginatedListAsync<Notification, NotificationResponse>(request);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateNotificationRequest request)
    {
        var notification = _mapper.Map<Notification>(request);

        _unitOfWork.Notifications.Add(notification);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create notification: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var notification = await _unitOfWork.Notifications.FindByIdAsync(id);
        if (notification == null)
            return Result.Failure("Notification not found");

        _unitOfWork.Notifications.Delete(notification);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete notification: {ex.Message}");
        }
    }
}