using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Checkpoint : AuditableEntity
{
    [MaxLength(255)] public required string Name { get; set; }
    [ForeignKey(nameof(Term))] public Guid TermId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public virtual Term Term { get; set; } = null!;

    public virtual ICollection<CheckpointTask> Tasks { get; set; } = [];
}