using Services.Models.Response.Base;

namespace Services.Models.Response.WeeklyReport;

public class WeeklyReportResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}