using System.Linq.Expressions;
using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Checkpoint;
using Services.Models.Response.Checkpoint;
using Services.Utils;

namespace Services.Services;

public class CheckpointService : ICheckpointService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CheckpointService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CheckpointResponse>> GetByIdAsync(Guid id)
    {
        var checkpoint = await _unitOfWork.Checkpoints.FindByIdAsync(id);
        if (checkpoint == null)
            return Result.Failure<CheckpointResponse>(DomainError.Checkpoint.CheckpointNotFound);

        return Result.Success(_mapper.Map<CheckpointResponse>(checkpoint));
    }

    public async Task<Result> CreateCheckpointAsync(CreateCheckpointRequest request)
    {
        var isExistTerm = await _unitOfWork.Terms.AnyAsync(x => x.Id == request.TermId);
        if (!isExistTerm)
        {
            return Result.Failure(DomainError.Term.TermNotFound);
        }

        var numberOfCheckpoints = _unitOfWork.Checkpoints.FindAll(x => x.TermId == request.TermId);
        if (numberOfCheckpoints.Count() >= 4)
        {
            return Result.Failure(DomainError.Checkpoint.ExceedCheckpoint);
        }

        var checkpoint = _mapper.Map<Checkpoint>(request);
        _unitOfWork.Checkpoints.Add(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create checkpoint: {ex.Message}");
        }
    }

    public async Task<Result> UpdateCheckpointAsync(Guid id, UpdateCheckpointRequest request)
    {
        var checkpoint = await _unitOfWork.Checkpoints.FindByIdAsync(id);
        if (checkpoint == null)
        {
            return Result.Failure(DomainError.Checkpoint.CheckpointNotFound);
        }

        if (request.TermId.HasValue && request.TermId.Value != Guid.Empty && request.TermId != checkpoint.TermId)
        {
            var isExistTerm = await _unitOfWork.Terms.AnyAsync(x => x.Id == request.TermId);
            if (!isExistTerm)
            {
                return Result.Failure(DomainError.Term.TermNotFound);
            }

            var numberOfCheckpoints = _unitOfWork.Checkpoints.FindAll(x => x.TermId == request.TermId);
            if (numberOfCheckpoints.Count() > 4)
            {
                return Result.Failure(DomainError.Checkpoint.ExceedCheckpoint);
            }

            checkpoint.TermId = request.TermId.Value;
        }

        checkpoint.StartTime = request.StartTime ?? checkpoint.StartTime;
        checkpoint.EndTime = request.EndTime ?? checkpoint.EndTime;

        if (checkpoint.StartTime > checkpoint.EndTime)
        {
            return Result.Failure(DomainError.Checkpoint.InvalidTime);
        }

        checkpoint.Name = request.Name ?? checkpoint.Name;
        _unitOfWork.Checkpoints.Update(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update checkpoint: {ex.Message}");
        }
    }

    public async Task<Result> DeleteCheckpointAsync(Guid id)
    {
        var checkpoint = await _unitOfWork.Checkpoints.FindByIdAsync(id);
        if (checkpoint == null)
        {
            return Result.Failure(DomainError.Checkpoint.CheckpointNotFound);
        }

        _unitOfWork.Checkpoints.Delete(checkpoint);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete checkpoint: {ex.Message}");
        }
    }

    public async Task<Result<PaginationResult<CheckpointResponse>>> GetPagedAsync(
        GetCheckpointsRequest paginationParams)
    {
        Expression<Func<Checkpoint, bool>> filter = checkpoint => true;
        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            Expression<Func<Checkpoint, bool>> searchTermFilter = checkpoint =>
                checkpoint.Name.Contains(paginationParams.SearchTerm);
            filter = Helper.CombineAndAlsoExpressions(filter, searchTermFilter);
        }

        if (paginationParams.TermId != Guid.Empty)
        {
            Expression<Func<Checkpoint, bool>> termIdFilter = checkpoint =>
                checkpoint.TermId == paginationParams.TermId;
            filter = Helper.CombineAndAlsoExpressions(filter, termIdFilter);
        }

        var query = _unitOfWork.Checkpoints.FindAll(filter);
        if (!string.IsNullOrEmpty(paginationParams.OrderBy))
        {
            var sortProperty = GetSortProperty(paginationParams.OrderBy);
            query = query.ApplySorting(paginationParams.IsDescending, sortProperty);
        }

        var result = await query.ProjectToPaginatedListAsync<Checkpoint, CheckpointResponse>(paginationParams);
        return Result.Success(result);
    }

    private Expression<Func<Checkpoint, object>> GetSortProperty(string sortBy) =>
        sortBy.ToLower().Replace(" ", "") switch
        {
            "name" => checkpoint => checkpoint.Name,
            "term" => checkpoint => checkpoint.TermId,
            "starttime" => checkpoint => checkpoint.StartTime,
            "endtime" => checkpoint => checkpoint.EndTime,
            _ => checkpoint => checkpoint.Id
        };
}