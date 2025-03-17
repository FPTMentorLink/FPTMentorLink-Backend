using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Project;

public class ProjectResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public Guid TermId { get; set; }
    public string TermCode { get; set; } = null!;
    public Guid FacultyId { get; set; }
    public string FacultyCode { get; set; } = null!;
    public Guid? MentorId { get; set; }
    public Guid? LecturerId { get; set; }
}