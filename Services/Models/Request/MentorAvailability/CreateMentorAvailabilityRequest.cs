namespace Services.Models.Request.MentorAvailability;

public class CreateMentorAvailabilityRequest
{
    public Guid MentorId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}