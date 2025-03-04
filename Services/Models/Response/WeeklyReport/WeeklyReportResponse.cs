using Services.Models.Response.Base;

namespace Services.Models.Response.WeeklyReport;

public class WeeklyReportResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}