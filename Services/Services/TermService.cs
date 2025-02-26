using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Result> CreateAsync(CreateTermRequest request)
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

    public async Task<Result> UpdateAsync(Guid id, UpdateTermRequest request)
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

        _mapper.Map(request, term);
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

    public async Task<Result> DeleteAsync(Guid id)
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
        GetTermsRequest paginationParams)
    {
        Expression<Func<Term, bool>> filter = term => true;
        if (!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            Expression<Func<Term, bool>> searchTermFilter = term =>
                term.Code.Contains(paginationParams.SearchTerm);
            filter = Helper.CombineAndAlsoExpressions(filter, searchTermFilter);
        }

        if (paginationParams.Status.HasValue)
        {
            Expression<Func<Term, bool>> statusFilter = term => term.Status == paginationParams.Status;
            filter = Helper.CombineAndAlsoExpressions(filter, statusFilter);
        }

        var query = _unitOfWork.Terms.FindAll(filter);
        if (!string.IsNullOrEmpty(paginationParams.OrderBy))
        {
            var sortProperty = GetSortProperty(paginationParams.OrderBy);
            query = query.ApplySorting(paginationParams.IsDescending, sortProperty);
        }

        var result = await query.ProjectToPaginatedListAsync<Term, TermResponse>(paginationParams);
        return Result.Success(result);
    }

    private Expression<Func<Term, object>> GetSortProperty(string sortBy) =>
        sortBy.ToLower().Replace(" ", "") switch
        {
            "code" => term => term.Code,
            "starttime" => term => term.StartTime,
            "endtime" => term => term.EndTime,
            "status" => term => term.Status,
            _ => term => term.Id
        };
}