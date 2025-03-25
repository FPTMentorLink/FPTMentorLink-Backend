using Repositories.Entities;
using Services.Models.Request.Appointment;
using Services.Models.Request.Student;
using Services.Models.Response.Appointment;
using Services.Utils;

namespace Services.Interfaces;

public interface IAppointmentService
{
    Task<Result<AppointmentResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<AppointmentResponse>>> GetPagedAsync(GetAppointmentsRequest request);
    Task<Result> CreateAsync(CreateAppointmentRequest request);
    Task<Result> UpdateStatusAsync(Guid id, AccountRole role, UpdateAppointmentStatusRequest request);
    Task<Result<PaginationResult<AppointmentResponse>>> GetMyAppointmentPagedAsync(
        GetMyAppointmentsRequest request, string role);
}