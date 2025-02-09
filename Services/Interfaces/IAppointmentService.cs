using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IAppointmentService
{
    Task<Result<AppointmentDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<AppointmentDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto);
    Task<Result<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto);
    Task<Result> DeleteAsync(Guid id);
}