using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Project : AuditableEntity
{
    [MaxLength(255)] public required string Code { get; set; }
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    [ForeignKey(nameof(Mentor))] public Guid MentorId { get; set; }
    [ForeignKey(nameof(Lecturer))] public Guid LecturerId { get; set; }
    [ForeignKey(nameof(Term))] public Guid TermId { get; set; }
    public virtual Mentor Mentor { get; set; } = null!;
    public virtual Lecturer Lecturer { get; set; } = null!;
    public virtual Term Term { get; set; } = null!;

    public virtual ICollection<CheckpointTask> Tasks { get; set; } = [];
    public virtual ICollection<WeeklyReport> Reports { get; set; } = [];
    public virtual ICollection<Appointment> Appointments { get; set; } = [];
    public virtual ICollection<ProjectStudent> Students { get; set; } = [];
}

public enum ProjectStatus
{
    Pending = 1,
    InProgress = 2,
    PendingReview = 3,
    Completed = 4,
    Failed = 5,
    RevisionRequired = 6
}