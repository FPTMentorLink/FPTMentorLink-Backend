using Services.Models.Request.Checkpoint;
using Services.Models.Response.Checkpoint;
using Services.Utils;

namespace Services.Interfaces;

public interface ICheckpointService
{
    Task<Result<CheckpointResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<CheckpointResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result> CreateAsync(CreateCheckpointRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateCheckpointRequest request);
    Task<Result> DeleteAsync(Guid id);
}