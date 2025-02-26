using Services.Models.Request.Checkpoint;
using Services.Models.Response.Checkpoint;
using Services.Utils;

namespace Services.Interfaces;

public interface ICheckpointService
{
    Task<Result<CheckpointResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<CheckpointResponse>>> GetPagedAsync(GetCheckpointsRequest paginationParams);
    Task<Result> CreateCheckpointAsync(CreateCheckpointRequest request);
    Task<Result> UpdateCheckpointAsync(Guid id, UpdateCheckpointRequest request);
    Task<Result> DeleteCheckpointAsync(Guid id);
}