using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class CheckpointTask : AuditableEntity
{
    [ForeignKey(nameof(Checkpoint))] public Guid CheckpointId { get; set; }
    [ForeignKey(nameof(Project))] public Guid ProjectId { get; set; }
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public CheckpointTaskStatus Status { get; set; }
    public double? Score { get; set; }
    public virtual Checkpoint Checkpoint { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;

    public static Expression<Func<CheckpointTask, object>> GetSortValue(string sortColumn) =>
        sortColumn switch
        {
            "name" => task => task.Name,
            "status" => task => task.Status,
            "score" => task => task.Score ?? 0,
            "createdat" => task => task.CreatedAt,
            "updatedat" => task => task.UpdatedAt ?? task.CreatedAt,
            _ => task => task.UpdatedAt ?? task.CreatedAt
        };
}

public enum CheckpointTaskStatus
{
    Pending = 1,
    InProgress = 2,
    PendingReview = 3,
    Completed = 4,
    Failed = 5
}