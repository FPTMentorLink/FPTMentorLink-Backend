using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Account;

namespace Services.Models.Request.Lecturer;

public class UpdateLecturerRequest : BaseUpdateAccountRequest
{
    [MaxLength(255)] public string? Code { get; set; }
    [MaxLength(255)] public string? Description { get; set; }
    public Guid? FacultyId { get; set; }
}