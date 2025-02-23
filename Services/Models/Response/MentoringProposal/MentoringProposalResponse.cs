using System.ComponentModel.DataAnnotations;
using Services.Models.Response.Base;

namespace Services.Models.Response.MentoringProposal;

public class MentoringProposalResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid MentorId { get; set; }
    public Guid ProjectId { get; set; }
    public string? StudentNote { get; set; }
    public string? MentorNote { get; set; }
    public bool IsAccepted { get; set; }
}