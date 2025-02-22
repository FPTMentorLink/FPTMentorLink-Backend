using Services.Models.Request.MentorAvailability;
using Services.Models.Response.MentorAvailability;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentorAvailabilityService
{
    Task<Result<MentorAvailabilityResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<MentorAvailabilityResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<MentorAvailabilityResponse>> CreateAsync(CreateMentorAvailabilityRequest dto);
    Task<Result<MentorAvailabilityResponse>> UpdateAsync(Guid id, UpdateMentorAvailabilityRequest dto);
    Task<Result> DeleteAsync(Guid id);
} 