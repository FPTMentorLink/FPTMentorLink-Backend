using Services.Models.Response.Base;

namespace Services.Models.Response.MentorAvailability;

public class MentorAvailabilityResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid MentorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}