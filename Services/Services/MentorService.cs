using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Request.Mentor;
using Services.Models.Response.Appointment;
using Services.Utils;

namespace Services.Services;

public class MentorService : IMentorService
{
    private readonly IAppointmentService _appointmentService;

    public MentorService(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<Result<PaginationResult<AppointmentResponse>>> GetMyAppointmentPagedAsync(
        GetMentorAppointmentsRequest request)
    {
        return await _appointmentService.GetPagedAsync(new GetAppointmentsRequest
        {
            StudentId = request.MentorId,
            Status = request.Status,
            SearchTerm = request.SearchTerm,
            OrderBy = request.OrderBy,
            IsDescending = request.IsDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }
}