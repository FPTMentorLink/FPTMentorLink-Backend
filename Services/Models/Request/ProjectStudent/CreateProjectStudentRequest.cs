namespace Services.Models.Request.ProjectStudent;

public class CreateProjectStudentRequest
{
    public Guid StudentId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsLeader { get; set; }
}