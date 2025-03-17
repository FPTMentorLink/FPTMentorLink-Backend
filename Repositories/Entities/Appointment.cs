using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Appointment : AuditableEntity
{
    [ForeignKey(nameof(Project))] public Guid ProjectId { get; set; }
    [ForeignKey(nameof(Mentor))] public Guid MentorId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int BaseSalaryPerHour { get; set; }
    public int TotalTime { get; set; } // in minutes
    public int TotalPayment { get; set; }

    [MaxLength(2000)] public string? CancelReason { get; set; }
    [MaxLength(2000)] public string? RejectReason { get; set; }
    public bool IsMentorCanceled { get; set; }


    public AppointmentStatus Status { get; set; }

    public virtual Project Project { get; set; } = null!;
    public virtual Mentor Mentor { get; set; } = null!;
}

public enum AppointmentStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
    PendingConfirmation = 4,
    ConfirmedByStudent = 5,
    ConfirmedByMentor = 6,
    Completed = 7,
    Canceled = 8,
    CancelRequested = 9
}