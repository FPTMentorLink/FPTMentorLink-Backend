using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Proposal;

public class GetProposalsRequest : PaginationQuery
{
    public Guid? FacultyId { get; set; }
    public ProposalStatus? Status { get; set; }
}