using Repositories.Entities;

namespace Services.DTOs;

public class AppointmentDto
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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateAppointmentDto
{
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

public class UpdateAppointmentDto
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AppointmentStatus? Status { get; set; }
}