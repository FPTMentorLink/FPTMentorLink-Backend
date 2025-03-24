using Services.Models.Request.Mentor;
using Services.Models.Request.Student;
using Services.Models.Response.Appointment;
using Services.Models.Response.Mentor;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentorService
{
    Task<Result<PaginationResult<AppointmentResponse>>>
        GetMyAppointmentPagedAsync(GetMentorAppointmentsRequest request);

    Task<Result<PaginationResult<GetMentorResponse>>> GetMentorsPagedAsync(GetMentorsRequest request);
}