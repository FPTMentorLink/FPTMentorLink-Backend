using Services.Models.Request.ProjectStudent;
using Services.Models.Response.ProjectStudent;
using Services.Utils;

namespace Services.Interfaces;

public interface IProjectStudentService 
{
    Task<Result<ProjectStudentResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<ProjectStudentResponse>>> GetPagedAsync(GetProjectStudentsRequest request);
    // Task<Result> CreateAsync(CreateProjectStudentRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result> SendProjectInvitationAsync(SendProjectInvitationRequest request);
    Task<Result> AcceptProjectInvitationAsync(AcceptProjectInvitationRequest request);
}