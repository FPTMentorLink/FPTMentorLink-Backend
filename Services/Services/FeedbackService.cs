using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
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

    public async Task<Result<FeedbackDto>> GetByIdAsync(Guid id)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return Result.Failure<FeedbackDto>("Feedback not found");

        return Result.Success(_mapper.Map<FeedbackDto>(feedback));
    }

    public async Task<Result<PaginationResult<FeedbackDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Feedbacks.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<Feedback, FeedbackDto>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<FeedbackDto>> CreateAsync(CreateFeedbackDto dto)
    {
        var feedback = _mapper.Map<Feedback>(dto);
        _unitOfWork.Feedbacks.Add(feedback);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<FeedbackDto>(feedback));
        }
        catch (Exception ex)
        {
            return Result.Failure<FeedbackDto>($"Failed to create feedback: {ex.Message}");
        }
    }

    public async Task<Result<FeedbackDto>> UpdateAsync(Guid id, UpdateFeedbackDto dto)
    {
        var feedback = await _unitOfWork.Feedbacks.GetByIdAsync(id);
        if (feedback == null)
            return Result.Failure<FeedbackDto>("Feedback not found");

        _mapper.Map(dto, feedback);
        _unitOfWork.Feedbacks.Update(feedback);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<FeedbackDto>(feedback));
        }
        catch (Exception ex)
        {
            return Result.Failure<FeedbackDto>($"Failed to update feedback: {ex.Message}");
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