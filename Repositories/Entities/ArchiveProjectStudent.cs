using Repositories.Entities.Base;

namespace Repositories.Entities;

public class ArchiveProjectStudent : AuditableEntity
{
    public Guid StudentId { get; set; }
    public Guid ProjectId { get; set; }
    public bool IsLeader { get; set; }
}