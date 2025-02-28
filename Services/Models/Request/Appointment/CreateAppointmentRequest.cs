using Repositories.Entities;

namespace Services.Models.Request.Appointment;

public class CreateAppointmentRequest
{
    public Guid ProjectId { get; set; }
    public Guid MentorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
}