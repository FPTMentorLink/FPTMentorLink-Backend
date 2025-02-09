using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IProposalService
{
    Task<Result<ProposalDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<ProposalDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<ProposalDto>> CreateAsync(CreateProposalDto dto);
    Task<Result<ProposalDto>> UpdateAsync(Guid id, UpdateProposalDto dto);
    Task<Result> DeleteAsync(Guid id);
} 