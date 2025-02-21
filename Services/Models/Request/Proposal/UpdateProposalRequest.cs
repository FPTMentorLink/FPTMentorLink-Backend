using Repositories.Entities;

namespace Services.Models.Request.Proposal;

public class UpdateProposalRequest
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ProposalStatus? Status { get; set; }
}