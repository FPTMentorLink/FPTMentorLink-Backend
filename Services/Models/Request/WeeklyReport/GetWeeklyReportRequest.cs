using Services.Utils;

namespace Services.Models.Request.WeeklyReport;

public class GetWeeklyReportRequest : PaginationParams
{
    public Guid? ProjectId { get; set; }
}