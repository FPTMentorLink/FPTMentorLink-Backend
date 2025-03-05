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
        Expression<Func<CheckpointTask, bool>> filter = task => true;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<CheckpointTask, bool>> searchFilter = task =>
                task.Name.Contains(request.SearchTerm) || task.Description!.Contains(request.SearchTerm);
            filter = Helper.CombineAndAlsoExpressions(filter, searchFilter);
        }

        if (request.CheckpointId.HasValue)
        {
            Expression<Func<CheckpointTask, bool>> checkpointFilter = task =>
                task.CheckpointId == request.CheckpointId.Value;
            filter = Helper.CombineAndAlsoExpressions(filter, checkpointFilter);
        }

        if (request.ProjectId.HasValue)
        {
            Expression<Func<CheckpointTask, bool>> projectFilter = task =>
                task.ProjectId == request.ProjectId.Value;
            filter = Helper.CombineAndAlsoExpressions(filter, projectFilter);
        }

        if (request.Status.HasValue)
        {
            Expression<Func<CheckpointTask, bool>> statusFilter = task =>
                task.Status == request.Status.Value;
            filter = Helper.CombineAndAlsoExpressions(filter, statusFilter);
        }

        var query = _unitOfWork.CheckpointTasks.FindAll(filter);
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            var sortProperty = GetSortProperty(request.OrderBy);
            query = query.ApplySorting(request.IsDescending, sortProperty);
        }

        var result = await query.ProjectToPaginatedListAsync<CheckpointTask, CheckpointTaskResponse>(request);
        return Result.Success(result);
    }

    private Expression<Func<CheckpointTask, object>> GetSortProperty(string sortBy) =>
        sortBy.ToLower().Replace(" ", "") switch
        {
            "name" => task => task.Name,
            "status" => task => task.Status,
            "score" => task => task.Score ?? 0,
            "checkpoint" => task => task.CheckpointId,
            "project" => task => task.ProjectId,
            _ => task => task.Id
        };
}