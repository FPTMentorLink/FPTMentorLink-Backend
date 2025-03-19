using System.Linq.Expressions;
using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Proposal;
using Services.Models.Response.Proposal;
using Services.Utils;

namespace Services.Services;

public class ProposalService : IProposalService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProposalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProposalResponse>> GetByIdAsync(Guid id)
    {
        var proposal = await _unitOfWork.Proposals.FindByIdAsync(id);
        if (proposal == null)
            return Result.Failure<ProposalResponse>("Proposal not found");

        return Result.Success(_mapper.Map<ProposalResponse>(proposal));
    }

    public async Task<Result<PaginationResult<ProposalResponse>>> GetPagedAsync(GetProposalsRequest request)
    {
        var query = _unitOfWork.Proposals.FindAll();
        Expression<Func<Proposal, bool>> condition = x => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Proposal, bool>> searchTermFilter = proposal =>
                proposal.Name.Contains(request.SearchTerm) ||
                proposal.Code.Contains(request.SearchTerm);
            condition = condition.CombineAndAlsoExpressions(searchTermFilter);
        }

        if (request.Status.HasValue)
        {
            Expression<Func<Proposal, bool>> statusFilter = proposal => proposal.Status == request.Status;
            condition = condition.CombineAndAlsoExpressions(statusFilter);
        }

        if (!request.FacultyId.IsNullOrGuidEmpty())
        {
            condition = condition.CombineAndAlsoExpressions(x => x.FacultyId == request.FacultyId);
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<Proposal, ProposalResponse>(request);

        return Result.Success(result);
    }

    private static IQueryable<Proposal> ApplySorting(IQueryable<Proposal> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, Proposal.GetSortValue(orderBy))
        };
    }

    public async Task<Result<ProposalResponse>> CreateAsync(CreateProposalRequest request)
    {
        var proposal = _mapper.Map<Proposal>(request);
        _unitOfWork.Proposals.Add(proposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ProposalResponse>(proposal));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProposalResponse>($"Failed to create proposal: {ex.Message}");
        }
    }

    public async Task<Result<ProposalResponse>> UpdateAsync(Guid id, UpdateProposalRequest request)
    {
        var proposal = await _unitOfWork.Proposals.FindByIdAsync(id);
        if (proposal == null)
            return Result.Failure<ProposalResponse>("Proposal not found");

        _mapper.Map(request, proposal);
        _unitOfWork.Proposals.Update(proposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ProposalResponse>(proposal));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProposalResponse>($"Failed to update proposal: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var proposal = await _unitOfWork.Proposals.FindByIdAsync(id);
        if (proposal == null)
            return Result.Failure("Proposal not found");

        _unitOfWork.Proposals.Delete(proposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete proposal: {ex.Message}");
        }
    }
}