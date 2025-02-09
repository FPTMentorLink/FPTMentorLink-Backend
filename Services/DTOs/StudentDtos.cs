namespace Services.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public int Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateStudentDto
{
    public required string Code { get; set; }
    public int Balance { get; set; }
}

public class UpdateStudentDto
{
    public string? Code { get; set; }
    public int? Balance { get; set; }
} 