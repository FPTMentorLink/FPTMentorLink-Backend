using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class MentoringProposal : AuditableEntity
{
    [ForeignKey(nameof(Mentor))] public Guid MentorId { get; set; }
    [ForeignKey(nameof(Project))] public Guid ProjectId { get; set; }
    [MaxLength(2000)] public string? StudentNote { get; set; }
    [MaxLength(2000)] public string? MentorNote { get; set; }
    public MentoringProposalStatus Status { get; set; }
    public virtual Mentor Mentor { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
    
    public static Expression<Func<MentoringProposal, object>> GetSortValue(string sortColumn) =>
        sortColumn switch
        {
            "status" => proposal => proposal.Status,
            "createdat" => proposal => proposal.CreatedAt,
            "updatedat" => proposal => proposal.UpdatedAt ?? proposal.CreatedAt,
            _ => proposal => proposal.UpdatedAt ?? proposal.CreatedAt
        };
}

public enum MentoringProposalStatus
{
    Pending,
    Accepted,
    Rejected,
    Closed
}