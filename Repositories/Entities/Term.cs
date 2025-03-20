using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
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

    public static Expression<Func<Term, object>> GetSortValue(string sortColumn) =>
        sortColumn switch
        {
            "code" => term => term.Code,
            "starttime" => term => term.StartTime,
            "endtime" => term => term.EndTime,
            "status" => term => term.Status,
            "createdat" => term => term.CreatedAt,
            "updatedat" => term => term.UpdatedAt ?? term.CreatedAt,
            _ => term => term.UpdatedAt ?? term.CreatedAt
        };
}

public enum TermStatus
{
    Pending = 1,
    Active = 2,
    Completed = 3
}