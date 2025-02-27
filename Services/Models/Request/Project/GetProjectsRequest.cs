using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Project;

public class GetProjectsRequest : PaginationQuery
{
    public Guid? StudentId { get; set; }
    public Guid? MentorId { get; set; }
    public Guid? LecturerId { get; set; }
    public Guid? TermId { get; set; }
    public Guid? FacultyId { get; set; }
    public ProjectStatus? Status { get; set; }
}