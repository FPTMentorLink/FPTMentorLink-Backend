using Services.Models.Response.WeeklyReportFeedback;

namespace Services.Models.Response.WeeklyReport;

public class WeeklyReportDetailResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Content { get; set; }
    public required string Title { get; set; }
    public WeeklyReportFeedBackResponse[]? Feedback { get; set; } = null!;
}