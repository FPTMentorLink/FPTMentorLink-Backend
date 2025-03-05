using Services.Models.Request.Base;

namespace Services.Models.Request.Appointment;

public class UpdateAppointmentRequest : ValidatableObject
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}