using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Account;

namespace Services.Models.Request.Student;

public class UpdateStudentRequest : BaseUpdateAccountRequest
{
    [MaxLength(255)] public string? Code { get; set; }
    public Guid? FacultyId { get; set; }
}