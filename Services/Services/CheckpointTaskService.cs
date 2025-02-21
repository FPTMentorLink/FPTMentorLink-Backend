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
        var checkpointTask = await _unitOfWork.CheckpointTasks.GetByIdAsync(id);
        if (checkpointTask == null)
            return Result.Failure<CheckpointTaskResponse>("Checkpoint task not found");

        return Result.Success(_mapper.Map<CheckpointTaskResponse>(checkpointTask));
    }

    public async Task<Result<PaginationResult<CheckpointTaskResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.CheckpointTasks.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<CheckpointTask, CheckpointTaskResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateCheckpointTaskRequest request)
    {
        var checkpointTask = _mapper.Map<CheckpointTask>(request);
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

    public async Task<Result> UpdateAsync(Guid id, UpdateCheckpointTaskRequest request)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.GetByIdAsync(id);
        if (checkpointTask == null)
            return Result.Failure("Checkpoint task not found");

        _mapper.Map(request, checkpointTask);
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

    public async Task<Result> DeleteAsync(Guid id)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.GetByIdAsync(id);
        if (checkpointTask == null)
            return Result.Failure("Checkpoint task not found");

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
}