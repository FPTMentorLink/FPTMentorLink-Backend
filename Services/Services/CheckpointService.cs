using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Checkpoint;
using Services.Models.Response.Checkpoint;
using Services.Utils;

namespace Services.Services;

public class CheckpointService : ICheckpointService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CheckpointService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CheckpointResponse>> GetByIdAsync(Guid id)
    {
        var checkpoint = await _unitOfWork.Checkpoints.GetByIdAsync(id);
        if (checkpoint == null)
            return Result.Failure<CheckpointResponse>("Checkpoint not found");

        return Result.Success(_mapper.Map<CheckpointResponse>(checkpoint));
    }

    public async Task<Result> CreateAsync(CreateCheckpointRequest request)
    {
        var checkpoint = _mapper.Map<Checkpoint>(request);
        _unitOfWork.Checkpoints.Add(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create checkpoint: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCheckpointRequest request)
    {
        var checkpoint = await _unitOfWork.Checkpoints.GetByIdAsync(id);
        if (checkpoint == null)
            return Result.Failure("Checkpoint not found");

        _mapper.Map(request, checkpoint);
        _unitOfWork.Checkpoints.Update(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update checkpoint: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var checkpoint = await _unitOfWork.Checkpoints.GetByIdAsync(id);
        if (checkpoint == null)
            return Result.Failure("Checkpoint not found");

        _unitOfWork.Checkpoints.Delete(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete checkpoint: {ex.Message}");
        }
    }

    public async Task<Result<PaginationResult<CheckpointResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Checkpoints.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<Checkpoint, CheckpointResponse>(paginationParams);

        return Result.Success(result);
    }
}