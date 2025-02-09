using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface ITaskLogService
{
    Task<Result<TaskLogDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<TaskLogDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<TaskLogDto>> CreateAsync(CreateTaskLogDto dto);
    Task<Result<TaskLogDto>> UpdateAsync(Guid id, UpdateTaskLogDto dto);
    Task<Result> DeleteAsync(Guid id);
} 