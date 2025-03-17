using Services.Models.Request.Term;
using Services.Models.Response.Term;
using Services.Utils;

namespace Services.Interfaces;

public interface ITermService
{
    Task<Result<TermResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<TermResponse>>> GetPagedAsync(GetTermsRequest paginationParams);
    Task<Result> CreateTermAsync(CreateTermRequest request);
    Task<Result> UpdateTermAsync(Guid id, UpdateTermRequest request);
    Task<Result> UpdateTermStatusAsync(Guid id, UpdateTermStatusRequest request);
    Task<Result> DeleteTermAsync(Guid id);
}