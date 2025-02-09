using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface ICheckpointTaskService
{
    Task<Result<CheckpointTaskDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<CheckpointTaskDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<CheckpointTaskDto>> CreateAsync(CreateCheckpointTaskDto dto);
    Task<Result<CheckpointTaskDto>> UpdateAsync(Guid id, UpdateCheckpointTaskDto dto);
    Task<Result> DeleteAsync(Guid id);
} 