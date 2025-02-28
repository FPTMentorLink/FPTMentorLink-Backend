namespace Services.Models.Request.WeeklyReportFeedback;

public class CreateWeeklyReportFeedback
{
    public Guid WeeklyReportId { get; set; }
    public Guid StudentId { get; set; }
    public string Content { get; set; } = null!;
}