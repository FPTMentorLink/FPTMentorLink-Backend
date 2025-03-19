using System.Linq.Expressions;
using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Term;
using Services.Models.Response.Term;
using Services.Utils;

namespace Services.Services;

public class TermService : ITermService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public TermService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TermResponse>> GetByIdAsync(Guid id)
    {
        var term = await _unitOfWork.Terms.FindByIdAsync(id);
        if (term == null)
        {
            return Result.Failure<TermResponse>(DomainError.Term.TermNotFound);
        }

        return Result.Success(_mapper.Map<TermResponse>(term));
    }

    public async Task<Result> CreateTermAsync(CreateTermRequest request)
    {
        var isExistingCode = await _unitOfWork.Terms.AnyAsync(x => x.Code == request.Code);
        if (isExistingCode)
        {
            return Result.Failure(DomainError.Term.TermCodeExists);
        }

        var term = _mapper.Map<Term>(request);
        term.Status = TermStatus.Pending;
        _unitOfWork.Terms.Add(term);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create term: {ex.Message}");
        }
    }

    public async Task<Result> UpdateTermAsync(Guid id, UpdateTermRequest request)
    {
        var term = await _unitOfWork.Terms.FindByIdAsync(id);
        if (term == null)
        {
            return Result.Failure(DomainError.Term.TermNotFound);
        }

        if (request.Code != null && request.Code != term.Code)
        {
            var isExistingCode = await _unitOfWork.Terms.AnyAsync(x => x.Code == request.Code);
            if (isExistingCode)
            {
                return Result.Failure(DomainError.Term.TermCodeExists);
            }

            term.Code = request.Code;
        }

        var startTime = request.StartTime ?? term.StartTime;
        var endTime = request.EndTime ?? term.EndTime;

        if (startTime >= endTime)
        {
            return Result.Failure(DomainError.Term.InvalidTime);
        }

        _unitOfWork.Terms.Update(term);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update term: {ex.Message}");
        }
    }

    public async Task<Result> UpdateTermStatusAsync(Guid id, UpdateTermStatusRequest request)
    {
        var term = await _unitOfWork.Terms.FindByIdAsync(id);
        if (term == null)
        {
            return Result.Failure(DomainError.Term.TermNotFound);
        }

        // Validate status transition
        if (term.Status >= (TermStatus)request.Status)
        {
            return Result.Failure("Cannot transition to a previous or same status");
        }

        if ((int)((TermStatus)request.Status) - (int)term.Status > 1)
        {
            return Result.Failure("Can only transition to the next status");
        }

        // Check for active term if trying to activate
        if ((TermStatus)request.Status == TermStatus.Active)
        {
            var anyActiveTerm = await _unitOfWork.Terms.AnyAsync(x =>
                x.Status == TermStatus.Active && x.Id != id);
            if (anyActiveTerm)
            {
                return Result.Failure("There is already an active term");
            }
        }

        term.Status = (TermStatus)request.Status;
        _unitOfWork.Terms.Update(term);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update term status: {ex.Message}");
        }
    }

    public async Task<Result> DeleteTermAsync(Guid id)
    {
        var term = await _unitOfWork.Terms.FindByIdAsync(id);
        if (term == null)
        {
            return Result.Failure(DomainError.Term.TermNotFound);
        }

        _unitOfWork.Terms.Delete(term);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete term: {ex.Message}");
        }
    }

    public async Task<Result<PaginationResult<TermResponse>>> GetPagedAsync(
        GetTermsRequest request)
    {
        var query = _unitOfWork.Terms.FindAll();
        Expression<Func<Term, bool>> condition = term => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Term, bool>> searchTermFilter = term =>
                term.Code.Contains(request.SearchTerm);
            condition = condition.CombineAndAlsoExpressions(searchTermFilter);
        }

        if (request.Status.HasValue)
        {
            Expression<Func<Term, bool>> statusFilter = term => term.Status == request.Status;
            condition = condition.CombineAndAlsoExpressions(statusFilter);
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<Term, TermResponse>(request);
        return Result.Success(result);
    }

    private static IQueryable<Term> ApplySorting(IQueryable<Term> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, Term.GetSortValue(orderBy))
        };
    }
}