using Services.Models.Request.CheckpointTask;
using Services.Models.Response.CheckpointTask;
using Services.Utils;

namespace Services.Interfaces;

public interface ICheckpointTaskService
{
    Task<Result<CheckpointTaskResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<CheckpointTaskResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result> CreateAsync(CreateCheckpointTaskRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateCheckpointTaskRequest request);
    Task<Result> DeleteAsync(Guid id);
} 