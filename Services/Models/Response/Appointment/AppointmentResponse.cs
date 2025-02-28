using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Appointment;

public class AppointmentResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid GroupId { get; set; }
    public Guid MentorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int BaseSalaryPerHour { get; set; }
    public int TotalTime { get; set; }
    public int TotalPayment { get; set; }
    public AppointmentStatus Status { get; set; }
}