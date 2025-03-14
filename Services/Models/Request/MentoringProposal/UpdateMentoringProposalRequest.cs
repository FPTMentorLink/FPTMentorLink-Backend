using Services.Models.Request.Base;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.MentoringProposal;

public class UpdateMentoringProposalRequest : ValidatableObject
{
    [MaxLength(2000)]
    public string? Note { get; set; }
    public bool? IsAccepted { get; set; }
    public bool? IsClosed { get; set; }
}