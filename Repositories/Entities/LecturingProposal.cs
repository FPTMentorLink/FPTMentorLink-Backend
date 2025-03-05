using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class LecturingProposal : AuditableEntity
{
    [ForeignKey(nameof(Lecturer))] public Guid LecturerId { get; set; }
    [ForeignKey(nameof(Project))] public Guid ProjectId { get; set; }
    [MaxLength(2000)] public string? StudentNote { get; set; }
    [MaxLength(2000)] public string? LecturerNote { get; set; }
    public LecturingProposalStatus Status { get; set; }
    public virtual Lecturer Lecturer { get; set; } = null!;
    public virtual Project Project { get; set; } = null!;
}

public enum LecturingProposalStatus
{
    Pending,
    Accepted,
    Rejected,
    Closed
}