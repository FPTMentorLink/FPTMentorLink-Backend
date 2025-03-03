using Services.Models.Request.MentoringProposal;
using Services.Models.Response.MentoringProposal;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentoringProposalService
{
    Task<Result<MentoringProposalResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<MentoringProposalResponse>>> GetPagedAsync(GetMentoringProposalsRequest request);
    Task<Result> CreateAsync(CreateMentoringProposalRequest request);
    Task<Result> StudentUpdateAsync(Guid id, StudentUpdateMentoringProposalRequest request);
    Task<Result> MentorUpdateAsync(Guid id, MentorUpdateMentoringProposalRequest request);
    Task<Result> DeleteAsync(Guid id);
}