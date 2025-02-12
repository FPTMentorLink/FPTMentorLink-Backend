using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class TaskLog : AuditableEntity
{
    [ForeignKey(nameof(CheckpointTask))] public Guid TaskId { get; set; }
    public TaskStatus Status { get; set; }
    [ForeignKey(nameof(Lecturer))] public Guid SetBy { get; set; }

    public virtual CheckpointTask CheckpointTask { get; set; } = null!;
    public virtual Lecturer Lecturer { get; set; } = null!;
}