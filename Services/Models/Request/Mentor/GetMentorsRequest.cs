using Services.Models.Request.Base;

namespace Services.Models.Request.Mentor;

public class GetMentorsRequest : PaginationQuery
{
    public string? Code { get; set; }
    public int? MinBaseSalaryPerHour { get; set; }
    public int? MaxBaseSalaryPerHour { get; set; }
}