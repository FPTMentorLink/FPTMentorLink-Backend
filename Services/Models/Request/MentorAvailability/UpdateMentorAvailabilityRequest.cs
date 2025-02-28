namespace Services.Models.Request.MentorAvailability;

public class UpdateMentorAvailabilityRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}