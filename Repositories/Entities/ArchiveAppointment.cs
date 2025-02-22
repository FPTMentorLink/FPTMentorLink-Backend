using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class ArchiveAppointment : AuditableEntity
{
    public Guid ProjectId { get; set; }
    public Guid MentorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int BaseSalaryPerHour { get; set; }
    public int TotalTime { get; set; }
    public int TotalPayment { get; set; }
    public AppointmentStatus Status { get; set; }
}