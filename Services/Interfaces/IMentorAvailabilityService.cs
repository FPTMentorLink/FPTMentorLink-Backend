using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentorAvailabilityService
{
    Task<Result<MentorAvailabilityDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<MentorAvailabilityDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<MentorAvailabilityDto>> CreateAsync(CreateMentorAvailabilityDto dto);
    Task<Result<MentorAvailabilityDto>> UpdateAsync(Guid id, UpdateMentorAvailabilityDto dto);
    Task<Result> DeleteAsync(Guid id);
} 