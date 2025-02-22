using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.MentorAvailability;
using Services.Models.Response.MentorAvailability;
using Services.Utils;

namespace Services.Services;

public class MentorAvailabilityService : IMentorAvailabilityService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MentorAvailabilityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<MentorAvailabilityResponse>> GetByIdAsync(Guid id)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.GetByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure<MentorAvailabilityResponse>("Mentor availability not found");

        return Result.Success(_mapper.Map<MentorAvailabilityResponse>(mentorAvailability));
    }

    public async Task<Result<PaginationResult<MentorAvailabilityResponse>>> GetPagedAsync(
        PaginationParams paginationParams)
    {
        var query = _unitOfWork.MentorAvailabilities.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<MentorAvailability, MentorAvailabilityResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<MentorAvailabilityResponse>> CreateAsync(CreateMentorAvailabilityRequest dto)
    {
        var mentorAvailability = _mapper.Map<MentorAvailability>(dto);
        _unitOfWork.MentorAvailabilities.Add(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<MentorAvailabilityResponse>(mentorAvailability));
        }
        catch (Exception ex)
        {
            return Result.Failure<MentorAvailabilityResponse>($"Failed to create mentor availability: {ex.Message}");
        }
    }

    public async Task<Result<MentorAvailabilityResponse>> UpdateAsync(Guid id, UpdateMentorAvailabilityRequest dto)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.GetByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure<MentorAvailabilityResponse>("Mentor availability not found");

        _mapper.Map(dto, mentorAvailability);
        _unitOfWork.MentorAvailabilities.Update(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<MentorAvailabilityResponse>(mentorAvailability));
        }
        catch (Exception ex)
        {
            return Result.Failure<MentorAvailabilityResponse>($"Failed to update mentor availability: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.GetByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure("Mentor availability not found");

        _unitOfWork.MentorAvailabilities.Delete(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete mentor availability: {ex.Message}");
        }
    }
}