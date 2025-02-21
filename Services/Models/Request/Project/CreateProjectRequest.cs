using Repositories.Entities;

namespace Services.Models.Request.Project;

public class CreateProjectRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid TermId { get; set; }
    public Guid MentorId { get; set; }
    public Guid LecturerId { get; set; }
}