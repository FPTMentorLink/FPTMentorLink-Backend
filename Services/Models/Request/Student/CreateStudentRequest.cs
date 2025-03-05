using Services.Models.Request.Account;

namespace Services.Models.Request.Student;

public class CreateStudentRequest : BaseCreateAccountRequest
{
    public required string Code { get; set; }
    public Guid FacultyId { get; set; }
}