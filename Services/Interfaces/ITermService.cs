using Services.Models.Request.Term;
using Services.Models.Response.Term;
using Services.Utils;

namespace Services.Interfaces;

public interface ITermService
{
    Task<Result<TermResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<TermResponse>>> GetPagedAsync(GetTermsRequest paginationParams);
    Task<Result> CreateAsync(CreateTermRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateTermRequest request);
    Task<Result> DeleteAsync(Guid id);
}