using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
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

    public static Expression<Func<Checkpoint, object>> GetSortValue(string sortColumn) =>
        sortColumn switch
        {
            "name" => checkpoint => checkpoint.Name,
            "term" => checkpoint => checkpoint.TermId,
            "starttime" => checkpoint => checkpoint.StartTime,
            "endtime" => checkpoint => checkpoint.EndTime,
            "createdat" => checkpoint => checkpoint.CreatedAt,
            "updatedat" => checkpoint => checkpoint.UpdatedAt ?? checkpoint.CreatedAt,
            _ => checkpoint => checkpoint.UpdatedAt ?? checkpoint.CreatedAt
        };
}