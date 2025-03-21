using Services.Models.Response.Base;
using Services.Models.Response.WeeklyReportFeedback;

namespace Services.Models.Response.WeeklyReport;

public class WeeklyReportResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
}