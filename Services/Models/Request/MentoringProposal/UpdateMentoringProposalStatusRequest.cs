using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.MentoringProposal;

public class UpdateMentoringProposalStatusRequest
{
    [Required] public Guid Id { get; set; }
    public string? MentorNote { get; set; }
    [Required] public bool IsAccepted { get; set; }
}