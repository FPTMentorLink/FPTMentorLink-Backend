using Services.Models.Request.Feedback;
using Services.Models.Response.Feedback;
using Services.Utils;

namespace Services.Interfaces;

public interface IFeedbackService
{
    Task<Result<FeedbackResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<FeedbackResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result> CreateAsync(CreateFeedbackRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateFeedbackRequest request);
    Task<Result> DeleteAsync(Guid id);
} 