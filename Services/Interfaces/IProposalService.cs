using Services.Models.Request.Proposal;
using Services.Models.Response.Proposal;
using Services.Utils;

namespace Services.Interfaces;

public interface IProposalService
{
    Task<Result<ProposalResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<ProposalResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<ProposalResponse>> CreateAsync(CreateProposalRequest request);
    Task<Result<ProposalResponse>> UpdateAsync(Guid id, UpdateProposalRequest request);
    Task<Result> DeleteAsync(Guid id);
} 