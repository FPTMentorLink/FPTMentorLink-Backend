using AutoMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class CheckpointTaskService : ICheckpointTaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CheckpointTaskService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CheckpointTaskDto>> GetByIdAsync(Guid id)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.GetByIdAsync(id);
        if (checkpointTask == null)
            return Result.Failure<CheckpointTaskDto>("Checkpoint task not found");

        return Result.Success(_mapper.Map<CheckpointTaskDto>(checkpointTask));
    }

    public async Task<Result<PaginationResult<CheckpointTaskDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.CheckpointTasks.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<CheckpointTask, CheckpointTaskDto>(paginationParams, _mapper.ConfigurationProvider);
            
        return Result.Success(result);
    }

    public async Task<Result<CheckpointTaskDto>> CreateAsync(CreateCheckpointTaskDto dto)
    {
        var checkpointTask = _mapper.Map<CheckpointTask>(dto);
        _unitOfWork.CheckpointTasks.Add(checkpointTask);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<CheckpointTaskDto>(checkpointTask));
        }
        catch (Exception ex)
        {
            return Result.Failure<CheckpointTaskDto>($"Failed to create checkpoint task: {ex.Message}");
        }
    }

    public async Task<Result<CheckpointTaskDto>> UpdateAsync(Guid id, UpdateCheckpointTaskDto dto)
    {
        var checkpointTask = await _unitOfWork.CheckpointTasks.GetByIdAsync(id);
        if (checkpointTask == null)
            return Result.Failure<CheckpointTaskDto>("Checkpoint task not found");

        _mapper.Map(dto, checkpointTask);
        _unitOfWork.CheckpointTasks.Update(checkpointTask);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<CheckpointTaskDto>(checkpointTask));
        }
        catch (Exception ex)
        {
            return Result.Failure<CheckpointTaskDto>($"Failed to update checkpoint task: {ex.Message}");
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