namespace Services.Models.Request.Feedback;

public class UpdateFeedbackRequest
{
    public string? Content { get; set; }
    public int? Rate { get; set; }
}