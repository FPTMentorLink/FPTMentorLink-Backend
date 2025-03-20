using System.Linq.Expressions;
using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.CheckpointTask;
using Services.Models.Response.CheckpointTask;
using Services.Utils;

namespace Services.Services;

public class CheckpointTaskService : ICheckpointTaskService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CheckpointTaskService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CheckpointTaskResponse>> GetByIdAsync(Guid id)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.FindByIdAsync(id);
        if (checkpointTask == null)
        {
            return Result.Failure<CheckpointTaskResponse>(DomainError.CheckpointTask.CheckpointTaskNotFound);
        }


        return Result.Success(_mapper.Map<CheckpointTaskResponse>(checkpointTask));
    }

    public async Task<Result> CreateCheckpointTaskAsync(CreateCheckpointTaskRequest request)
    {
        var isCheckpointExists = await _unitOfWork.Checkpoints.AnyAsync(x => x.Id == request.CheckpointId);
        if (!isCheckpointExists)
        {
            return Result.Failure(DomainError.Checkpoint.CheckpointNotFound);
        }

        var isProjectExists = await _unitOfWork.Projects.AnyAsync(x => x.Id == request.ProjectId);
        if (!isProjectExists)
        {
            return Result.Failure(DomainError.Project.ProjectNotFound);
        }

        var checkpointTask = _mapper.Map<CheckpointTask>(request);
        checkpointTask.Status = CheckpointTaskStatus.Pending;
        _unitOfWork.CheckpointTasks.Add(checkpointTask);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create checkpoint task: {ex.Message}");
        }
    }

    public async Task<Result> UpdateCheckpointTaskAsync(Guid id, UpdateCheckpointTaskRequest request)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.FindByIdAsync(id);
        if (checkpointTask == null)
        {
            return Result.Failure(DomainError.CheckpointTask.CheckpointTaskNotFound);
        }

        checkpointTask.Name = request.Name ?? checkpointTask.Name;
        checkpointTask.Description = request.Description ?? checkpointTask.Description;
        checkpointTask.Score = request.Score ?? checkpointTask.Score;
        _unitOfWork.CheckpointTasks.Update(checkpointTask);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update checkpoint task: {ex.Message}");
        }
    }

    public async Task<Result> UpdateCheckpointTaskStatusAsync(Guid id, UpdateCheckpointTaskStatusRequest request)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.FindByIdAsync(id);
        if (checkpointTask == null)
        {
            return Result.Failure(DomainError.CheckpointTask.CheckpointTaskNotFound);
        }

        if (request.Status.HasValue && checkpointTask.Status != request.Status)
        {
            if (checkpointTask.Status > request.Status)
            {
                return Result.Failure(DomainError.CheckpointTask.InvalidStatusTransition);
            }

            if (checkpointTask.Status == CheckpointTaskStatus.Completed &&
                request.Status != CheckpointTaskStatus.Failed)
            {
                return Result.Failure(DomainError.CheckpointTask.CheckpointTaskHasBeenCompleted);
            }

            checkpointTask.Status = request.Status ?? checkpointTask.Status;
        }

        _unitOfWork.CheckpointTasks.Update(checkpointTask);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update checkpoint task status: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCheckpointTaskAsync(Guid id)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.FindByIdAsync(id);
        if (checkpointTask == null)
        {
            return Result.Failure(DomainError.CheckpointTask.CheckpointTaskNotFound);
        }

        if (checkpointTask.Status != CheckpointTaskStatus.Pending)
        {
            return Result.Failure(DomainError.CheckpointTask.CheckpointTaskInProgress);
        }

        _unitOfWork.CheckpointTasks.Delete(checkpointTask);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete checkpoint task: {ex.Message}");
        }
    }

    public async Task<Result<PaginationResult<CheckpointTaskResponse>>> GetPagedAsync(
        GetCheckpointTasksRequest request)
    {
        var query = _unitOfWork.CheckpointTasks.FindAll();
        Expression<Func<CheckpointTask, bool>> condition = x => true;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<CheckpointTask, bool>> searchTermFilter = task =>
                task.Name.Contains(request.SearchTerm) || task.Description!.Contains(request.SearchTerm);
            condition = condition.CombineAndAlsoExpressions(searchTermFilter);
        }

        if (request.CheckpointId.HasValue)
        {
            condition = condition.CombineAndAlsoExpressions(task =>
                task.CheckpointId == request.CheckpointId.Value);
        }

        if (request.ProjectId.HasValue)
        {
            condition = condition.CombineAndAlsoExpressions(task =>
                task.ProjectId == request.ProjectId.Value);
        }

        if (request.Status.HasValue)
        {
            condition = condition.CombineAndAlsoExpressions(task =>
                task.Status == request.Status.Value);
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<CheckpointTask, CheckpointTaskResponse>(request);
        return Result.Success(result);
    }

    private static IQueryable<CheckpointTask> ApplySorting(IQueryable<CheckpointTask> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, CheckpointTask.GetSortValue(orderBy))
        };
    }
}