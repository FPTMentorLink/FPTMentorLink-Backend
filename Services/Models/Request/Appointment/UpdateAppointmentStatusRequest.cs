using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Appointment;

public class UpdateAppointmentStatusRequest : ValidatableObject
{
    public AppointmentStatus? Status { get; set; }
}