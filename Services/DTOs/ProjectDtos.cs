using Repositories.Entities;

namespace Services.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid TermId { get; set; }
    public Guid MentorId { get; set; }
    public Guid LecturerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProjectDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public Guid TermId { get; set; }
    public Guid MentorId { get; set; }
    public Guid LecturerId { get; set; }
}

public class UpdateProjectDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class UpdateProjectStatus
{
    public ProjectStatus? Status { get; set; }
}