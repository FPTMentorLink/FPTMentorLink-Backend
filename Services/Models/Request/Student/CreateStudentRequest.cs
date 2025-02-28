namespace Services.Models.Request.Student;

public class CreateStudentRequest
{
    public required string Code { get; set; }
    public int Balance { get; set; }
}