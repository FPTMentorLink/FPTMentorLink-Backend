namespace Services.Models.Response.Lecturer;

public class GetLecturerResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string FullName { get; set; } = default!;  // From Account
    public string Email { get; set; } = default!;     // From Account
    public string? Department { get; set; }
    public DateTime CreatedAt { get; set; }
}