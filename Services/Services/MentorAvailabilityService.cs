using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Repositories.Utils;
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
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.FindByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure<MentorAvailabilityResponse>("Mentor availability not found");

        return Result.Success(_mapper.Map<MentorAvailabilityResponse>(mentorAvailability));
    }

    public async Task<Result<PaginationResult<MentorAvailabilityResponse>>> GetPagedAsync(
        GetMentorAvailabilitiesRequest request)
    {
        var query = _unitOfWork.MentorAvailabilities.FindAll();
        Expression<Func<MentorAvailability, bool>> condition = x => true;
        if (request.MentorId != Guid.Empty)
            condition = condition.CombineAndAlsoExpressions(x => x.MentorId == request.MentorId);
        if (request.Date != null)
            condition = condition.CombineAndAlsoExpressions(x => x.Date.Date == request.Date.Value.Date);
        if (request.StartDate != null)
            condition = condition.CombineAndAlsoExpressions(x => x.Date >= request.StartDate.Value.Date);
        if (request.EndDate != null)
            condition = condition.CombineAndAlsoExpressions(x => x.Date <= request.EndDate.Value.Date);
        query = query.Where(condition);
        query = query.OrderBy(x => x.Date);

        var result =
            await query.ProjectToPaginatedListAsync<MentorAvailability, MentorAvailabilityResponse>(request);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateMentorAvailabilityRequest request)
    {
        var date = request.Date.Date;
        var existingMentorAvailability = await _unitOfWork.MentorAvailabilities
            .FindSingleAsync(x => x.MentorId == request.MentorId && x.Date == date);
        var mentorExist = await _unitOfWork.Mentors.AnyAsync(x => x.Id == request.MentorId);
        if (!mentorExist)
            return Result.Failure("Mentor not found");
        if (existingMentorAvailability != null)
            return Result.Failure("Mentor availability for this date already exists");
        var mentorAvailability = _mapper.Map<MentorAvailability>(request);
        _unitOfWork.MentorAvailabilities.Add(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create mentor availability: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateMentorAvailabilityRequest request)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.FindByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure("Mentor availability not found");
        if (mentorAvailability.MentorId != request.MentorId)
            return Result.Failure("Mentor availability cannot be updated for another mentor");
        var mentorExist = await _unitOfWork.Mentors.AnyAsync(x => x.Id == request.MentorId);
        if (!mentorExist)
            return Result.Failure("Mentor not found");
        var date = mentorAvailability.Date.Date;

        // Check if the new date is in the past
        if (date <= DateTime.Now.Date)
            return Result.Failure("Mentor Availability must be updated at least 1 day in advance.");

        // Create a temporary MentorAvailability with the new time map
        var tempAvailability = new MentorAvailability
        {
            MentorId = mentorAvailability.MentorId,
            Date = mentorAvailability.Date,
            TimeMap = request.TimeMap
        };

        // Get existing appointments for this mentor on this date
        var nextDay = date.AddDays(1);
        var existingAppointments = await _unitOfWork.Appointments
            .FindAll(a => a.MentorId == mentorAvailability.MentorId &&
                          a.StartTime >= date &&
                          a.StartTime < nextDay &&
                          a.Status != AppointmentStatus.Rejected &&
                          a.Status != AppointmentStatus.Canceled).ToListAsync();

        // Check for conflicts with existing appointments
        foreach (var appointment in existingAppointments)
        {
            // Convert appointment times to slot indices
            var startSlot = TimeMapUtils.GetSlotFromTime(
                new TimeSpan(appointment.StartTime.Hour, appointment.StartTime.Minute, 0));
            var endSlot = TimeMapUtils.GetSlotFromTime(
                new TimeSpan(appointment.EndTime.Hour, appointment.EndTime.Minute, 0));

            // Check if any required slot is available in the new time map
            // Slots with appointments should be unavailable
            for (var slot = startSlot; slot < endSlot; slot++)
            {
                if (tempAvailability.IsAvailable(slot))
                {
                    return Result.Failure($"Cannot update availability: Time slot from " +
                                          $"{appointment.StartTime:HH:mm} to {appointment.EndTime:HH:mm} is already booked");
                }
            }
        }

        _mapper.Map(request, mentorAvailability);
        _unitOfWork.MentorAvailabilities.Update(mentorAvailability);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update mentor availability: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, Guid mentorId)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.FindByIdAsync(id);
        if (mentorAvailability == null)
            return Result.Failure("Mentor availability not found");
        if (mentorAvailability.MentorId != mentorId)
            return Result.Failure("Mentor availability cannot be deleted for another mentor");
        var mentorExist = await _unitOfWork.Mentors.AnyAsync(x => x.Id == mentorId);
        if (!mentorExist)
            return Result.Failure("Mentor not found");

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