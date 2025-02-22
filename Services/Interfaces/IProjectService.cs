using Services.Models.Request.Project;
using Services.Models.Response.Project;
using Services.Utils;

namespace Services.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<ProjectResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<ProjectResponse>> CreateAsync(CreateProjectRequest dto);
    Task<Result<ProjectResponse>> UpdateAsync(Guid id, UpdateProjectRequest dto);
    Task<Result> DeleteAsync(Guid id);
}