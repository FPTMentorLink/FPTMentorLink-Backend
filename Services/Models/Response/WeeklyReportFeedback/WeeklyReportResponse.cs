using Services.Models.Response.Base;

namespace Services.Models.Response.WeeklyReportFeedback;

public class WeeklyReportResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid WeeklyReportId { get; set; }
    public Guid StudentId { get; set; }
    public string Content { get; set; } = null!;
}