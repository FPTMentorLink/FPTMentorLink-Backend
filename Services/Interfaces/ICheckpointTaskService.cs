using Services.Models.Request.CheckpointTask;
using Services.Models.Response.CheckpointTask;
using Services.Utils;

namespace Services.Interfaces;

public interface ICheckpointTaskService
{
    Task<Result<CheckpointTaskResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<CheckpointTaskResponse>>> GetPagedAsync(GetCheckpointTasksRequest request);
    Task<Result> CreateCheckpointTaskAsync(CreateCheckpointTaskRequest request);
    Task<Result> UpdateCheckpointTaskAsync(Guid id, UpdateCheckpointTaskRequest request);
    Task<Result> UpdateCheckpointTaskStatusAsync(Guid id, UpdateCheckpointTaskStatusRequest request);
    Task<Result> DeleteCheckpointTaskAsync(Guid id);
}