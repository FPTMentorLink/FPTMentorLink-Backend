using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.LecturingProposal;

public class CreateLecturingProposalRequest : ValidatableObject
{
    [Required] public Guid LecturerId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [MaxLength(2000)] public string? StudentNote { get; set; }
}