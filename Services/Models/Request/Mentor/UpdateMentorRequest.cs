using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Account;

namespace Services.Models.Request.Mentor;

public class UpdateMentorRequest : BaseUpdateAccountRequest
{
    [MaxLength(255)] public string? Code { get; set; }
    [MaxLength(255)] public string? BankName { get; set; }
    [MaxLength(255)] public string? BankCode { get; set; }
    public int? BaseSalaryPerHour { get; set; }
}