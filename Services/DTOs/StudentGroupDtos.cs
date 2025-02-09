namespace Services.DTOs;

public class StudentGroupDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid GroupId { get; set; }
    public bool IsLeader { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateStudentGroupDto
{
    public Guid StudentId { get; set; }
    public Guid GroupId { get; set; }
    public bool IsLeader { get; set; }
}

public class UpdateStudentGroupDto
{
    public bool? IsLeader { get; set; }
} 