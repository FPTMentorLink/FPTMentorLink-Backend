namespace Services.DTOs;

public class ProjectStudentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsLeader { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProjectStudentDto
{
    public Guid StudentId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsLeader { get; set; }
}

public class UpdateProjectStudentDto
{
    public bool? IsLeader { get; set; }
} 