using Services.Models.Response.Base;

namespace Services.Models.Response.Student;

public class StudentResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public int Balance { get; set; }
}