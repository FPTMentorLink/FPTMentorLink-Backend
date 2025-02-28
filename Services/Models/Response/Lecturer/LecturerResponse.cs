using Services.Models.Response.Base;

namespace Services.Models.Response.Lecturer;

public class LecturerResponse  : AuditableResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Faculty { get; set; }
}