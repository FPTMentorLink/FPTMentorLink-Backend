using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Proposal;

public class ProposalResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ProposalStatus Status { get; set; }
    public string StatusName => Status.ToString();
}