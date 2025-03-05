using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.MentoringProposal;

public class StudentUpdateMentoringProposalRequest : ValidatableObject
{
    [MaxLength(2000)] public string? StudentNote { get; set; }
    public bool IsClosed { get; set; }
}