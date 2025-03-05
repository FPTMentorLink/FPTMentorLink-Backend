using Services.Models.Request.LecturingProposal;
using Services.Models.Response.LecturingProposal;
using Services.Utils;

namespace Services.Interfaces;

public interface ILecturingProposalService
{
    Task<Result<LecturingProposalResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<LecturingProposalResponse>>> GetPagedAsync(GetLecturingProposalsRequest request);
    Task<Result> CreateAsync(CreateLecturingProposalRequest request);
    Task<Result> StudentUpdateAsync(Guid id, StudentUpdateLecturingProposalRequest request);
    Task<Result> LecturerUpdateAsync(Guid id, LecturerUpdateLecturingProposalRequest request);
    Task<Result> DeleteAsync(Guid id);
}