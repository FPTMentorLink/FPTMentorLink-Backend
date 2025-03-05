using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
}

public enum MentoringProposalStatus
{
    Pending,
    Accepted,
    Rejected,
    Closed
}