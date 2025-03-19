using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class WeeklyReport : AuditableEntity
{
    [ForeignKey(nameof(Project))] public Guid ProjectId { get; set; }
    [MaxLength(255)] public required string Title { get; set; }
    [MaxLength(2000)] public required string Content { get; set; }

    public virtual Project Project { get; set; } = null!;

    public static Expression<Func<WeeklyReport, object>> GetSortValue(string sortColumn) =>
        sortColumn switch
        {
            "title" => report => report.Title,
            "content" => report => report.Content,
            "createdat" => report => report.CreatedAt,
            "updatedat" => report => report.UpdatedAt ?? report.CreatedAt,
            _ => report => report.UpdatedAt ?? report.CreatedAt
        };
}