using Services.Models.Request.Base;

namespace Services.Models.Request.LecturingProposal;

public class LecturerUpdateLecturingProposalRequest : ValidatableObject
{
    public string? LecturerNote { get; set; }
    public bool? IsAccepted { get; set; }
}