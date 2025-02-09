using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<ProjectDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto);
    Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<Result> DeleteAsync(Guid id);
} 