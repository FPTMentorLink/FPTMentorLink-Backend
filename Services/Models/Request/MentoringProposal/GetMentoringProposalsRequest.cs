using Services.Models.Request.Base;

namespace Services.Models.Request.MentoringProposal;

public class GetMentoringProposalsRequest : PaginationQuery
{
    public Guid? MentorId { get; set; }
    public Guid? StudentId { get; set; }
    public Guid? ProjectId { get; set; }
}