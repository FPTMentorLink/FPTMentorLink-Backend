using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Response.Appointment;
using Services.StateMachine.Appointment;
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

    public async Task<Result> UpdateStatusAsync(Guid id, AccountRole role, UpdateAppointmentStatusRequest request)
    {
        var appointment = await _unitOfWork.Appointments.FindByIdAsync(id);
        if (appointment == null)
            return Result.Failure("Appointment not found");

        // Validate state transition and role permission
        var validation = AppointmentStateManager.ValidateTransition(appointment.Status, request.Status, role);
        if (!validation.IsSuccess)
            return validation;

        // Handle specific status transition logic
        var result = appointment.Status switch
        {
            AppointmentStatus.Pending => request.Status switch
            {
                AppointmentStatus.Accepted => await UpdateToAccepted(appointment),
                AppointmentStatus.Rejected => await UpdateToRejected(appointment, request),
                AppointmentStatus.Canceled => await HandlePendingCancellation(appointment, request, role),
                AppointmentStatus.CancelRequested => await UpdateToCancelRequested(appointment, request),
                _ => Result.Failure("Invalid status transition")
            },
            AppointmentStatus.Accepted => request.Status switch
            {
                AppointmentStatus.Canceled => await HandleAcceptedCancellation(appointment, request, role),
                _ => Result.Failure("Invalid status transition")
            },
            AppointmentStatus.PendingConfirmation => request.Status switch
            {
                AppointmentStatus.ConfirmedByStudent => await UpdateToConfirmedByStudent(appointment),
                AppointmentStatus.ConfirmedByMentor => await UpdateToConfirmedByMentor(appointment),
                AppointmentStatus.CancelRequested => await UpdateToCancelRequested(appointment, request),
                AppointmentStatus.Canceled => await HandleAdminCancellation(appointment, request),
                _ => Result.Failure("Invalid status transition")
            },
            AppointmentStatus.ConfirmedByStudent => request.Status switch
            {
                AppointmentStatus.ConfirmedByMentor => await UpdateToConfirmedByMentor(appointment),
                AppointmentStatus.Canceled => await HandleAdminCancellation(appointment, request),
                _ => Result.Failure("Invalid status transition")
            },
            AppointmentStatus.ConfirmedByMentor => request.Status switch
            {
                AppointmentStatus.ConfirmedByStudent => await UpdateToConfirmedByStudent(appointment),
                AppointmentStatus.CancelRequested => await UpdateToCancelRequested(appointment, request),
                AppointmentStatus.Canceled => await HandleAdminCancellation(appointment, request),
                _ => Result.Failure("Invalid status transition")
            },
            AppointmentStatus.CancelRequested => request.Status switch
            {
                AppointmentStatus.Canceled => await HandleAdminCancellation(appointment, request),
                AppointmentStatus.Completed => await UpdateToCompleted(appointment),
                _ => Result.Failure("Invalid status transition")
            },
            _ => Result.Failure("Invalid current status")
        };

        if (!result.IsSuccess)
            return result;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private async Task<Result> UpdateToConfirmedByMentor(Appointment appointment)
    {
        // If both confirmed, proceed to completed
        if (appointment.Status == AppointmentStatus.ConfirmedByStudent)
        {
            return await UpdateToCompleted(appointment);
        }

        appointment.Status = AppointmentStatus.ConfirmedByMentor;
        _unitOfWork.Appointments.Update(appointment);
        return Result.Success();
    }

    private Task<Result> UpdateToCancelRequested(Appointment appointment,
        UpdateAppointmentStatusRequest request)
    {
        if (DateTime.UtcNow < appointment.EndTime)
            return Task.FromResult(
                Result.Failure("Student can only request cancellation after the appointment has ended"));
        
        appointment.Status = AppointmentStatus.CancelRequested;
        appointment.CancelReason = request.CancelReason;
        _unitOfWork.Appointments.Update(appointment);
        
        return Task.FromResult(Result.Success());
    }

    private async Task<Result> UpdateToConfirmedByStudent(Appointment appointment)
    {
        // If both confirmed, proceed to completed
        if (appointment.Status == AppointmentStatus.ConfirmedByMentor)
        {
            return await UpdateToCompleted(appointment);
        }

        appointment.Status = AppointmentStatus.ConfirmedByStudent;
        _unitOfWork.Appointments.Update(appointment);
        return Result.Success();
    }

    /// <summary>
    /// At pending status, student, admin can cancel
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    private async Task<Result> HandlePendingCancellation(Appointment appointment,
        UpdateAppointmentStatusRequest request,
        AccountRole role)
    {
        var result = role switch
        {
            AccountRole.Student => await HandleStudentPendingCancellation(appointment, request),
            AccountRole.Admin => await HandleAdminCancellation(appointment, request),
            _ => Result.Failure($"Role {role} is not allowed to cancel pending appointments")
        };

        if (!result.IsSuccess)
            return result;

        return Result.Success();
    }

    /// <summary>
    /// At accepted status, student, mentor, admin can cancel
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    private async Task<Result> HandleAcceptedCancellation(Appointment appointment,
        UpdateAppointmentStatusRequest request, AccountRole role)
    {
        var result = role switch
        {
            AccountRole.Student => await HandleStudentAcceptedCancellation(appointment, request),
            AccountRole.Mentor => await HandleMentorAcceptedCancellation(appointment, request),
            AccountRole.Admin => await HandleAdminCancellation(appointment, request),
            _ => Result.Failure($"Role {role} is not allowed to cancel accepted appointments")
        };
        if (!result.IsSuccess)
            return result;
        return Result.Success();
    }

    /// <summary>
    /// This method updates the status of an appointment to "Canceled"
    /// This is done by student only
    /// This will also process refund and return mentor availability
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Result> HandleStudentPendingCancellation(Appointment appointment,
        UpdateAppointmentStatusRequest request)
    {
        if (DateTime.UtcNow.AddHours(Constants.RequireCancelAppointmentInAdvance) > appointment.StartTime)
            return Result.Failure(
                $"Appointment cannot be canceled within {Constants.RequireCancelAppointmentInAdvance} hours of the start time");
        var refundResult = await ProcessAppointmentRefund(appointment);
        if (refundResult.IsFailure)
            return refundResult;

        appointment.Status = AppointmentStatus.Canceled;
        appointment.CancelReason = request.CancelReason;
        _unitOfWork.Appointments.Update(appointment);
        
        return await ReturnMentorAvailability(appointment);
    }

    /// <summary>
    /// This method updates the status of an appointment to "Canceled"
    /// This is done by student only
    /// This will also process refund and return mentor availability
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Result> HandleStudentAcceptedCancellation(Appointment appointment,
        UpdateAppointmentStatusRequest request)
    {
        if (DateTime.UtcNow.AddHours(Constants.RequireCancelAppointmentInAdvance) > appointment.StartTime)
            return Result.Failure(
                $"Appointment cannot be canceled within {Constants.RequireCancelAppointmentInAdvance} hours of the start time");
        var refundResult = await ProcessAppointmentRefund(appointment);
        if (refundResult.IsFailure)
            return refundResult;

        appointment.Status = AppointmentStatus.Canceled;
        appointment.CancelReason = request.CancelReason;
        _unitOfWork.Appointments.Update(appointment);

        return await ReturnMentorAvailability(appointment);
    }

    /// <summary>
    /// This method updates the status of an appointment to "Canceled"
    /// This is done by mentor only
    /// This will also process refund
    /// Mentor must set availability manually as mentor may not be available
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Result> HandleMentorAcceptedCancellation(Appointment appointment,
        UpdateAppointmentStatusRequest request)
    {
        if (DateTime.UtcNow.AddHours(Constants.RequireCancelAppointmentInAdvance) > appointment.StartTime)
            return Result.Failure(
                $"Appointment cannot be canceled within {Constants.RequireCancelAppointmentInAdvance} hours of the start time");
        var refundResult = await ProcessAppointmentRefund(appointment);
        if (refundResult.IsFailure)
            return refundResult;

        appointment.Status = AppointmentStatus.Canceled;
        appointment.CancelReason = request.CancelReason;
        _unitOfWork.Appointments.Update(appointment);

        return Result.Success();
    }

    /// <summary>
    /// This method updates the status of an appointment to "Canceled"
    /// This is done by admin only
    /// This will also process refund
    /// Mentor must set availability manually as mentor may not be available
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Result> HandleAdminCancellation(Appointment appointment, UpdateAppointmentStatusRequest request)
    {
        // Admin can cancel with full refund on any appointment on any status
        var refundResult = await ProcessAppointmentRefund(appointment);
        if (refundResult.IsFailure)
            return refundResult;

        appointment.Status = AppointmentStatus.Canceled;
        appointment.CancelReason = request.CancelReason;
        _unitOfWork.Appointments.Update(appointment);

        return Result.Success();
    }

    /// <summary>
    /// This method updates the status of an appointment to "Accepted"
    /// This is done by mentor only
    /// Mentor can only accept appointment before the start time
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    private Task<Result> UpdateToAccepted(Appointment appointment)
    {
        if (DateTime.UtcNow > appointment.StartTime)
            return Task.FromResult(Result.Failure("Appointment cannot be accepted after the start time"));
        appointment.Status = AppointmentStatus.Accepted;
        _unitOfWork.Appointments.Update(appointment);
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// This method updates the status of an appointment to "Rejected"
    /// This is done by mentor only
    /// This will also process refund
    /// Mentor must set availability manually as mentor may not be available
    /// </summary>
    /// <param name="appointment"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task<Result> UpdateToRejected(Appointment appointment, UpdateAppointmentStatusRequest request)
    {
        if (DateTime.UtcNow.AddHours(Constants.RequireCancelAppointmentInAdvance) > appointment.StartTime)
            return Result.Failure(
                $"Appointment cannot be rejected within {Constants.RequireCancelAppointmentInAdvance} hours of the start time");
        appointment.Status = AppointmentStatus.Rejected;
        appointment.RejectReason = request.RejectReason;

        // Process refund
        var refundResult = await ProcessAppointmentRefund(appointment);
        if (refundResult.IsFailure)
            return refundResult;

        _unitOfWork.Appointments.Update(appointment);
        return Result.Success();
    }

    /// <summary>
    /// This method updates the status of an appointment to "Completed"
    /// This is done by admin only
    /// This will also process payment to mentor
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    private async Task<Result> UpdateToCompleted(Appointment appointment)
    {
        // Process payment
        var paymentResult = await ProcessAppointmentPayment(appointment);
        if (paymentResult.IsFailure)
            return paymentResult;

        appointment.Status = AppointmentStatus.Completed;
        _unitOfWork.Appointments.Update(appointment);
        return Result.Success();
    }

    /// <summary>
    /// This method update mentor balance
    /// by adding appointment total payment to mentor balance
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    private async Task<Result> ProcessAppointmentPayment(Appointment appointment)
    {
        var mentor = await _unitOfWork.Mentors.FindByIdAsync(appointment.MentorId);
        if (mentor == null)
            return Result.Failure("Mentor not found");
        mentor.Balance += appointment.TotalPayment;
        _unitOfWork.Mentors.Update(mentor);
        return Result.Success();
    }

    /// <summary>
    /// This method update student balance
    /// by adding appointment total payment back to student balance
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    private async Task<Result> ProcessAppointmentRefund(Appointment appointment)
    {
        // Get project leader for refund
        var leader = await _unitOfWork.ProjectStudents
            .FindAll(x => x.ProjectId == appointment.ProjectId && x.IsLeader)
            .Include(x => x.Student)
            .FirstOrDefaultAsync();

        if (leader == null)
            return Result.Failure("Project leader not found");

        // Process refund
        var student = leader.Student;
        student.Balance += appointment.TotalPayment;
        _unitOfWork.Students.Update(student);

        return Result.Success();
    }

    /// <summary>
    /// This method return mentor availability (set as available)
    /// </summary>
    /// <param name="appointment"></param>
    /// <returns></returns>
    private async Task<Result> ReturnMentorAvailability(Appointment appointment)
    {
        var mentorAvailability = await _unitOfWork.MentorAvailabilities.FindSingleAsync(x =>
            x.MentorId == appointment.MentorId && x.Date == appointment.StartTime.Date);

        if (mentorAvailability == null)
            return Result.Failure("Mentor availability not found");

        mentorAvailability.SetAvailabilityRange(appointment.StartTime.TimeOfDay, appointment.EndTime.TimeOfDay, true);
        _unitOfWork.MentorAvailabilities.Update(mentorAvailability);

        return Result.Success();
    }
}