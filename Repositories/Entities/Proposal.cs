using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Proposal : AuditableEntity
{
    [MaxLength(255)] public required string Code { get; set; }
    [MaxLength(255)] public required string Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    [ForeignKey(nameof(Faculty))] public Guid FacultyId { get; set; }
    public ProposalStatus Status { get; set; }
    public virtual Faculty Faculty { get; set; } = null!;
    
    public static Expression<Func<Proposal, object>> GetSortValue(string sortColumn) =>
        sortColumn switch
        {
            "code" => proposal => proposal.Code,
            "name" => proposal => proposal.Name,
            "status" => proposal => proposal.Status,
            "createdat" => proposal => proposal.CreatedAt,
            "updatedat" => proposal => proposal.UpdatedAt ?? proposal.CreatedAt,
            _ => proposal => proposal.UpdatedAt ?? proposal.CreatedAt
        };
}

public enum ProposalStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4
}