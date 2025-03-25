using Services.Models.Response.Account;
using Services.Models.Response.MentorAvailability;

namespace Services.Models.Response.Mentor;

public class GetMentorResponse : BaseAccountResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Balance { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int BaseSalaryPerHour { get; set; }
    public List<MentorAvailabilityResponse> Availabilities { get; set; } = new();
}