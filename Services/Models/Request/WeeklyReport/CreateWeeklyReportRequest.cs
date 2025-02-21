namespace Services.Models.Request.WeeklyReport;

public class CreateWeeklyReportRequest
{
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}