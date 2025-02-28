using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.WeeklyReportFeedback;

public class UpdateWeeklyReportFeedback
{
    [Required] public Guid Id { get; set; }
    [Required] public string Content { get; set; } = null!;
}