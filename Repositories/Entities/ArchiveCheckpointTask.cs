using System.ComponentModel.DataAnnotations;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class ArchiveCheckpointTask : AuditableEntity
{
    public Guid CheckpointId { get; set; }
    public Guid ProjectId { get; set; }
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public CheckpointTaskStatus Status { get; set; }
}