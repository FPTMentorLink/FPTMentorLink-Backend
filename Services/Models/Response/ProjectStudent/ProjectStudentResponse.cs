using Services.Models.Response.Base;

namespace Services.Models.Response.ProjectStudent;

public class ProjectStudentResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string Code { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsLeader { get; set; }
}