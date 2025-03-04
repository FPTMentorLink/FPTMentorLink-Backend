using Services.Utils;

namespace Services.Models.Request.WeeklyReport;

public class GetWeeklyReportsRequest : PaginationParams
{
    public Guid? ProjectId { get; set; }
}