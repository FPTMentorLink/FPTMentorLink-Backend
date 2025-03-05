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
    [ForeignKey(nameof(Mentor))] public Guid? MentorId { get; set; }
    [ForeignKey(nameof(Lecturer))] public Guid? LecturerId { get; set; }
    [ForeignKey(nameof(Term))] public Guid TermId { get; set; }
    [ForeignKey(nameof(Faculty))] public Guid FacultyId { get; set; }
    public virtual Mentor? Mentor { get; set; }
    public virtual Lecturer? Lecturer { get; set; }
    public virtual Term Term { get; set; } = null!;
    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<CheckpointTask> Tasks { get; set; } = [];
    public virtual ICollection<WeeklyReport> Reports { get; set; } = [];
    public virtual ICollection<Appointment> Appointments { get; set; } = [];
    public virtual ICollection<ProjectStudent> ProjectStudents { get; set; } = [];
}

public enum ProjectStatus
{
    Pending = 1,
    InProgress = 2,
    PendingReview = 3,
    Completed = 4,
    Failed = 5,
    RevisionRequired = 6,
    Closed = 7
}