using Services.Models.Request.AppointmentFeedback;
using Services.Models.Response.AppointmentFeedback;
using Services.Utils;

namespace Services.Interfaces;

public interface IFeedbackService
{
    Task<Result<AppointmentFeedbackResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<AppointmentFeedbackResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result> CreateAsync(CreateAppointmentFeedbackRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateAppointmentFeedbackRequest request);
    Task<Result> DeleteAsync(Guid id);
} 