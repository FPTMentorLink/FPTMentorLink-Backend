using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Response.Appointment;
using Services.Utils;

namespace Services.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentResponse>> GetByIdAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointments.FindByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentResponse>("Appointment not found");

        return Result.Success(_mapper.Map<AppointmentResponse>(appointment));
    }

    public async Task<Result<PaginationResult<AppointmentResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Appointments.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Appointment, AppointmentResponse>(paginationParams);

        return Result.Success(result);
    }

    /// <summary>
    /// Creates a new appointment between a mentor and project leader, updates mentor availability, and processes payment.
    /// </summary>
    /// <param name="request">The appointment creation request containing:
    ///     - ProjectId: ID of the project
    ///     - MentorId: ID of the mentor
    ///     - LeaderId: ID of the project leader
    ///     - StartTime: Appointment start date and time
    ///     - EndTime: Appointment end date and time
    ///     - TotalMinutes: Duration of appointment in minutes
    /// </param>
    /// <remarks>
    /// This method performs the following operations:
    /// 1. Creates new appointment with pending status
    /// 2. Updates mentor's availability for the specified time slot
    /// 3. Deducts payment from leader's balance based on mentor's base salary
    /// </remarks>
    /// <returns>A <see cref="Result"/> indicating success or failure.
    /// Success result if appointment is created successfully
    /// Failure result if:
    ///     - Project not found
    ///     - Mentor not found
    ///     - Leader not found
    ///     - Mentor is not available in this time range
    ///     - Leader balance is not enough
    ///     - Database operation fails
    /// </returns>
    /// <exception cref="Exception">Thrown when database operation fails</exception>
    public async Task<Result> CreateAsync(CreateAppointmentRequest request)
    {
        var isExistProject = await _unitOfWork.Projects.AnyAsync(x => x.Id == request.ProjectId);
        if (!isExistProject)
            return Result.Failure("Project not found");
        var mentor = await _unitOfWork.Mentors.FindByIdAsNoTrackingAsync(request.MentorId);
        if (mentor == null)
            return Result.Failure("Mentor not found");
        var leader = await _unitOfWork.Students.FindAll()
            .Include(x => x.ProjectStudent)
            .Where(x => x.Id == request.LeaderId &&
                        x.ProjectStudent != null &&
                        x.ProjectStudent.ProjectId == request.ProjectId &&
                        x.ProjectStudent.IsLeader).FirstOrDefaultAsync();
        if (leader == null)
            return Result.Failure("Leader not found");

        var appointment = _mapper.Map<Appointment>(request);
        appointment.BaseSalaryPerHour = mentor.BaseSalaryPerHour;
        appointment.TotalTime = request.TotalMinutes;
        appointment.TotalPayment = appointment.BaseSalaryPerHour * appointment.TotalTime;
        appointment.Status = AppointmentStatus.Pending;

        var mentorAvailability = await _unitOfWork.MentorAvailabilities.FindSingleAsync(x =>
            x.MentorId == request.MentorId && x.Date == request.StartTime.Date);
        // Check if mentor has set availability on this date
        if (mentorAvailability == null)
            return Result.Failure("Mentor is not available on this date");
        // Check if mentor is available in this time range
        var isAvailable =
            mentorAvailability.IsAvailabilityInRange(request.StartTime.TimeOfDay, request.EndTime.TimeOfDay);
        if (!isAvailable)
            return Result.Failure("Mentor is not available in this time range");
        // Check if leader balance is enough
        if (leader.Balance < appointment.TotalPayment)
            return Result.Failure("Leader balance is not enough");

        _unitOfWork.Appointments.Add(appointment);

        // Update mentor availability
        mentorAvailability.SetAvailabilityRange(request.StartTime.TimeOfDay, request.EndTime.TimeOfDay, false);
        _unitOfWork.MentorAvailabilities.Update(mentorAvailability);

        // Update leader balance
        leader.Balance -= appointment.TotalPayment;
        _unitOfWork.Students.Update(leader);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create appointment: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateAppointmentRequest request)
    {
        var appointment = await _unitOfWork.Appointments.FindByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentResponse>("Appointment not found");

        _mapper.Map(request, appointment);
        _unitOfWork.Appointments.Update(appointment);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update appointment: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointments.FindByIdAsync(id);
        if (appointment == null)
            return Result.Failure("Appointment not found");

        _unitOfWork.Appointments.Delete(appointment);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete appointment: {ex.Message}");
        }
    }
}