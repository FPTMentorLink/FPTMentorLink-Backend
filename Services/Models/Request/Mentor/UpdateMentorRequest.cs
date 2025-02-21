namespace Services.Models.Request.Mentor;

public class UpdateMentorRequest
{
    public string? Code { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int? BaseSalaryPerHour { get; set; }
}