using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface ICheckpointService
{
    Task<Result<CheckpointDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<CheckpointDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<CheckpointDto>> CreateAsync(CreateCheckpointDto dto);
    Task<Result<CheckpointDto>> UpdateAsync(Guid id, UpdateCheckpointDto dto);
    Task<Result> DeleteAsync(Guid id);
}