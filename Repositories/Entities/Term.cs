using System.ComponentModel.DataAnnotations;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Term : AuditableEntity
{
    [MaxLength(255)] public required string Code { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TermStatus Status { get; set; }

    public virtual ICollection<Checkpoint> Checkpoints { get; set; } = [];
    public virtual ICollection<Project> Projects { get; set; } = [];
}

public enum TermStatus
{
    Pending = 1,
    Active = 2,
    Completed = 3 
}