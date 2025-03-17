using Services.Models.Response.Base;

namespace Services.Models.Response.WeeklyReportFeedback;

public class WeeklyReportFeedBackResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid WeeklyReportId { get; set; }
    public string LecturerName { get; set; } = null!;
    public string Content { get; set; } = null!;
}