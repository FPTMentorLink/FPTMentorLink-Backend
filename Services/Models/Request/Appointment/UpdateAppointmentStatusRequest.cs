using Repositories.Entities;

namespace Services.Models.Request.Appointment;

public class UpdateAppointmentStatusRequest
{
    public AppointmentStatus? Status { get; set; }
}