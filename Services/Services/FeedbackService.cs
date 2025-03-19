using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.AppointmentFeedback;
using Services.Models.Response.AppointmentFeedback;
using Services.Utils;

namespace Services.Services;

public class FeedbackService : IFeedbackService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentFeedbackResponse>> GetByIdAsync(Guid id)
    {
        var feedback = await _unitOfWork.AppointmentFeedbacks.FindByIdAsync(id);
        if (feedback == null)
            return Result.Failure<AppointmentFeedbackResponse>("Feedback not found");

        return Result.Success(_mapper.Map<AppointmentFeedbackResponse>(feedback));
    }

    public async Task<Result<PaginationResult<AppointmentFeedbackResponse>>> GetPagedAsync(
        PaginationParams paginationParams)
    {
        var query = _unitOfWork.AppointmentFeedbacks.FindAll();
        var result =
            await query.ProjectToPaginatedListAsync<AppointmentFeedback, AppointmentFeedbackResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateAppointmentFeedbackRequest request)
    {
        var feedback = _mapper.Map<AppointmentFeedback>(request);
        _unitOfWork.AppointmentFeedbacks.Add(feedback);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create feedback: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateAppointmentFeedbackRequest request)
    {
        var feedback = await _unitOfWork.AppointmentFeedbacks.FindByIdAsync(id);
        if (feedback == null)
            return Result.Failure("Feedback not found");

        _mapper.Map(request, feedback);
        _unitOfWork.AppointmentFeedbacks.Update(feedback);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update feedback: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var feedback = await _unitOfWork.AppointmentFeedbacks.FindByIdAsync(id);
        if (feedback == null)
            return Result.Failure("Feedback not found");

        _unitOfWork.AppointmentFeedbacks.Delete(feedback);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete feedback: {ex.Message}");
        }
    }
}