using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.MentoringProposal;

public class CreateMentoringProposalRequest : ValidatableObject
{
    [Required] public Guid MentorId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [MaxLength(2000)] public string? StudentNote { get; set; }
}