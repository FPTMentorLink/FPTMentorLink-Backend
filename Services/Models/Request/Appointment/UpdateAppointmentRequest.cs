namespace Services.Models.Request.Appointment;

public class UpdateAppointmentRequest
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}