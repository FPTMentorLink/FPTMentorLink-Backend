using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.LecturingProposal;

public class StudentUpdateLecturingProposalRequest : ValidatableObject
{
    [MaxLength(2000)] public string? StudentNote { get; set; }
    public bool IsClosed { get; set; }
}