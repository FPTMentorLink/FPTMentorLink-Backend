using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.MentorAvailability;

public class MentorAvailabilityResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid MentorId { get; set; }
    public DateTime Date { get; set; }
    public byte[] TimeMap { get; set; } = new byte[12];
    public List<AvailableTimeSlot> AvailableTimeSlots { get; set; } = [];
}