using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IFeedbackService
{
    Task<Result<FeedbackDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<FeedbackDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<FeedbackDto>> CreateAsync(CreateFeedbackDto dto);
    Task<Result<FeedbackDto>> UpdateAsync(Guid id, UpdateFeedbackDto dto);
    Task<Result> DeleteAsync(Guid id);
} 