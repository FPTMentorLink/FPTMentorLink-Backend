using Services.Models.Request.Base;

namespace Services.Models.Request.MentoringProposal;

public class MentorUpdateMentoringProposalRequest : ValidatableObject
{
    public string? MentorNote { get; set; }
    public bool? IsAccepted { get; set; }
}