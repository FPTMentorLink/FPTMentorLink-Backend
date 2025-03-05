using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Account;

namespace Services.Models.Request.Lecturer;

public class CreateLecturerRequest : BaseCreateAccountRequest
{
    [Required] [MaxLength(50)] public string Code { get; set; } = null!;
    [MaxLength(2000)] public string? Description { get; set; }
    [Required] public Guid FacultyId { get; set; }
}