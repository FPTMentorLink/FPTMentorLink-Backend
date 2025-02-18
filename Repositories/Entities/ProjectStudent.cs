using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class ProjectStudent : AuditableEntity
{
    [ForeignKey(nameof(Student))] public Guid StudentId { get; set; }
    [ForeignKey(nameof(Project))] public Guid ProjectId { get; set; }
    public bool IsLeader { get; set; }

    public virtual Student Student { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
}