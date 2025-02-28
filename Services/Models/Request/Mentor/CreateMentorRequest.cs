using Services.Models.Request.Account;

namespace Services.Models.Request.Mentor;

public class CreateMentorRequest : CreateAccountRequest
{
    public string Code { get; set; } = null!;
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int BaseSalaryPerHour { get; set; }
}