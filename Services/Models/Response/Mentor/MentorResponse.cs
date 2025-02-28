using Services.Models.Response.Base;

namespace Services.Models.Response.Mentor;

public class MentorResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Balance { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int BaseSalaryPerHour { get; set; }
}