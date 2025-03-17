using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.LecturingProposal;

public class LecturingProposalResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid LecturerId { get; set; }
    public Guid ProjectId { get; set; }
    public string? StudentNote { get; set; }
    public string? LecturerNote { get; set; }
    public LecturingProposalStatus Status { get; set; }
    public string StatusName => Status.ToString();
}