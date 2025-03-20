using Services.Models.Request.Base;

namespace Services.Models.Request.LecturingProposal;

public class GetLecturingProposalsRequest : PaginationQuery
{
    public Guid? LecturerId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? StudentId { get; set; }
}