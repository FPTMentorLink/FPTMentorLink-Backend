using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Services.Models.Request.WeeklyReportFeedback;

public class CreateWeeklyReportFeedback
{
    [JsonIgnore]
    [Required]
    public Guid LecturerId { get; set; }
    [MaxLength(2000)]
    public string Content { get; set; } = null!;
}