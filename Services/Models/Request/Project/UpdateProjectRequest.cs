namespace Services.Models.Request.Project;

public class UpdateProjectRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? FacultyId { get; set; }
    public Guid? MentorId { get; set; }
    public Guid? LecturerId { get; set; }
}