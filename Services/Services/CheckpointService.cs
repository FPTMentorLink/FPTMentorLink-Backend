using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
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

    public async Task<Result<CheckpointDto>> GetByIdAsync(Guid id)
    {
        var checkpoint = await _unitOfWork.Checkpoints.GetByIdAsync(id);
        if (checkpoint == null)
            return Result.Failure<CheckpointDto>("Checkpoint not found");

        return Result.Success(_mapper.Map<CheckpointDto>(checkpoint));
    }

    public async Task<Result<CheckpointDto>> CreateAsync(CreateCheckpointDto dto)
    {
        var checkpoint = _mapper.Map<Checkpoint>(dto);
        _unitOfWork.Checkpoints.Add(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<CheckpointDto>(checkpoint));
        }
        catch (Exception ex)
        {
            return Result.Failure<CheckpointDto>($"Failed to create checkpoint: {ex.Message}");
        }
    }

    public async Task<Result<CheckpointDto>> UpdateAsync(Guid id, UpdateCheckpointDto dto)
    {
        var checkpoint = await _unitOfWork.Checkpoints.GetByIdAsync(id);
        if (checkpoint == null)
            return Result.Failure<CheckpointDto>("Checkpoint not found");

        _mapper.Map(dto, checkpoint);
        _unitOfWork.Checkpoints.Update(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<CheckpointDto>(checkpoint));
        }
        catch (Exception ex)
        {
            return Result.Failure<CheckpointDto>($"Failed to update checkpoint: {ex.Message}");
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

    public async Task<Result<PaginationResult<CheckpointDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Checkpoints.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<Checkpoint, CheckpointDto>(paginationParams);

        return Result.Success(result);
    }
}