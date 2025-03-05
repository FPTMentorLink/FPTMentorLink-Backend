using Services.Models.Request.MentorAvailability;
using Services.Models.Response.MentorAvailability;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentorAvailabilityService
{
    Task<Result<MentorAvailabilityResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<MentorAvailabilityResponse>>> GetPagedAsync(GetMentorAvailabilitiesRequest request);
    Task<Result> CreateAsync(CreateMentorAvailabilityRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateMentorAvailabilityRequest request);
    Task<Result> DeleteAsync(Guid id);
} 