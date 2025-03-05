using Services.Models.Request.Project;
using Services.Models.Response.Project;
using Services.Utils;

namespace Services.Interfaces;

public interface IProjectService
{
    Task<Result<ProjectDetailResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<ProjectResponse>>> GetPagedAsync(GetProjectsRequest request);
    Task<Result> CreateAsync(CreateProjectRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<Result> UpdateStatusAsync(Guid id, UpdateProjectStatusRequest request);
    Task<Result> DeleteAsync(Guid id);
}