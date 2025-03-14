using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.LecturingProposal;

public class UpdateLecturingProposalRequest 
{
    [MaxLength(2000)]
    public string? Note { get; set; }
    public bool? IsAccepted { get; set; }
    public bool? IsClosed { get; set; }
}
