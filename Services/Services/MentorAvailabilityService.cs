using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
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

    public async Task<Result<MentorAvailabilityDto>> GetByIdAsync(Guid id)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.GetByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure<MentorAvailabilityDto>("Mentor availability not found");

        return Result.Success(_mapper.Map<MentorAvailabilityDto>(mentorAvailability));
    }

    public async Task<Result<PaginationResult<MentorAvailabilityDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.MentorAvailabilities.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<MentorAvailability, MentorAvailabilityDto>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<MentorAvailabilityDto>> CreateAsync(CreateMentorAvailabilityDto dto)
    {
        var mentorAvailability = _mapper.Map<MentorAvailability>(dto);
        _unitOfWork.MentorAvailabilities.Add(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<MentorAvailabilityDto>(mentorAvailability));
        }
        catch (Exception ex)
        {
            return Result.Failure<MentorAvailabilityDto>($"Failed to create mentor availability: {ex.Message}");
        }
    }

    public async Task<Result<MentorAvailabilityDto>> UpdateAsync(Guid id, UpdateMentorAvailabilityDto dto)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.GetByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure<MentorAvailabilityDto>("Mentor availability not found");

        _mapper.Map(dto, mentorAvailability);
        _unitOfWork.MentorAvailabilities.Update(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<MentorAvailabilityDto>(mentorAvailability));
        }
        catch (Exception ex)
        {
            return Result.Failure<MentorAvailabilityDto>($"Failed to update mentor availability: {ex.Message}");
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