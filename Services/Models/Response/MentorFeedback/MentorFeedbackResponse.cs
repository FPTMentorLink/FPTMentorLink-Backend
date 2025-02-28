namespace Services.Models.Response.MentorFeedback;

public class MentorFeedbackResponse
{
    public Guid MentorId { get; set; }
    public Guid StudentId { get; set; }
    public string? Content { get; set; }
    public int Rate { get; set; }
}