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
        GetCheckpointsRequest request)
    {
        var query = _unitOfWork.Checkpoints.FindAll();
        Expression<Func<Checkpoint, bool>> condition = checkpoint => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Checkpoint, bool>> searchTermFilter = checkpoint =>
                checkpoint.Name.Contains(request.SearchTerm);
            condition = condition.CombineAndAlsoExpressions(searchTermFilter);
        }

        if (request.TermId != Guid.Empty)
        {
            Expression<Func<Checkpoint, bool>> termIdFilter = checkpoint =>
                checkpoint.TermId == request.TermId;
            condition = condition.CombineAndAlsoExpressions(termIdFilter);
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<Checkpoint, CheckpointResponse>(request);
        return Result.Success(result);
    }

    private static IQueryable<Checkpoint> ApplySorting(IQueryable<Checkpoint> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, Checkpoint.GetSortValue(orderBy))
        };
    }
}