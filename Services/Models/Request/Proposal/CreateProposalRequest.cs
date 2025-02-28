using Repositories.Entities;

namespace Services.Models.Request.Proposal;

public class CreateProposalRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProposalStatus Status { get; set; }
}