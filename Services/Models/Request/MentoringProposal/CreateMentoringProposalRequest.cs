using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.MentoringProposal;

public class CreateMentoringProposalRequest
{
    [Required] public Guid MentorId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [MaxLength(2000)] public string? StudentNote { get; set; }
}