using Services.Models.Request.Account;

namespace Services.Models.Request.Lecturer;

public class CreateLecturerRequest : CreateAccountRequest
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Faculty { get; set; }
}