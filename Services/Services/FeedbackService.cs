using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Feedback;
using Services.Models.Response.Feedback;
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

    public async Task<Result<FeedbackResponse>> GetByIdAsync(Guid id)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return Result.Failure<FeedbackResponse>("Feedback not found");

        return Result.Success(_mapper.Map<FeedbackResponse>(feedback));
    }

    public async Task<Result<PaginationResult<FeedbackResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Feedbacks.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<Feedback, FeedbackResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateFeedbackRequest request)
    {
        var feedback = _mapper.Map<Feedback>(request);
        _unitOfWork.Feedbacks.Add(feedback);

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

    public async Task<Result> UpdateAsync(Guid id, UpdateFeedbackRequest request)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return Result.Failure("Feedback not found");

        _mapper.Map(request, feedback);
        _unitOfWork.Feedbacks.Update(feedback);

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
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return Result.Failure("Feedback not found");

        _unitOfWork.Feedbacks.Delete(feedback);

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