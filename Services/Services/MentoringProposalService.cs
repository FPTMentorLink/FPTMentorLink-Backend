using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.MentoringProposal;
using Services.Models.Response.MentoringProposal;
using Services.Utils;

namespace Services.Services;

public class MentoringProposalService : IMentoringProposalService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MentoringProposalService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MentoringProposalResponse>> GetByIdAsync(Guid id)
    {
        var mentoringProposal = await _unitOfWork.MentoringProposals.FindByIdAsync(id);
        if (mentoringProposal == null)
            return Result.Failure<MentoringProposalResponse>("mentoring proposal not found");

        return Result.Success(_mapper.Map<MentoringProposalResponse>(mentoringProposal));
    }

    public async Task<Result<PaginationResult<MentoringProposalResponse>>> GetPagedAsync(
        GetMentoringProposalsRequest request)
    {
        var query = _unitOfWork.MentoringProposals.FindAll();
        Expression<Func<MentoringProposal, bool>> condition = x => true;

        if (!request.ProjectId.IsNullOrGuidEmpty())
            condition = condition.CombineAndAlsoExpressions(x => x.ProjectId == request.ProjectId);

        if (!request.MentorId.IsNullOrGuidEmpty())
            condition = condition.CombineAndAlsoExpressions(x => x.MentorId == request.MentorId);

        if (!request.StudentId.IsNullOrGuidEmpty())
        {
            query = query.Include(x => x.Project).ThenInclude(x => x.ProjectStudents);
            condition.CombineAndAlsoExpressions(x =>
                x.Project.ProjectStudents.Any(y => y.StudentId == request.StudentId));
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<MentoringProposal, MentoringProposalResponse>(request);

        return Result.Success(result);
    }

    private static IQueryable<MentoringProposal> ApplySorting(IQueryable<MentoringProposal> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, MentoringProposal.GetSortValue(orderBy))
        };
    }

    public async Task<Result> CreateAsync(CreateMentoringProposalRequest request)
    {
        var isExistProject = await _unitOfWork.Projects.AnyAsync(x => x.Id == request.ProjectId);
        if (!isExistProject)
            return Result.Failure("Project not found");

        var isExistMentor =
            await _unitOfWork.Accounts
                .FindAll(x => x.Email == request.Email && x.Role == AccountRole.Mentor)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        if (isExistMentor == Guid.Empty)
            return Result.Failure("Mentor not found");

        var isExistProposal = await _unitOfWork.MentoringProposals.AnyAsync(x =>
            x.ProjectId == request.ProjectId && x.MentorId == isExistMentor);
        if (isExistProposal)
            return Result.Failure("Mentoring proposal already exists");

        var mentoringProposal = _mapper.Map<MentoringProposal>(request);
        mentoringProposal.MentorId = isExistMentor;
        // Default status is pending for mentor to accept
        mentoringProposal.Status = MentoringProposalStatus.Pending;
        _unitOfWork.MentoringProposals.Add(mentoringProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create mentoring proposal: {ex.Message}");
        }
    }

    /// <summary>
    /// Student can only set the status to closed once while the status is pending
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result> StudentUpdateAsync(Guid id, StudentUpdateMentoringProposalRequest request)
    {
        var mentoringProposal = await _unitOfWork.MentoringProposals.FindByIdAsync(id);
        if (mentoringProposal == null)
            return Result.Failure("Mentoring proposal not found");

        if (mentoringProposal.Status != MentoringProposalStatus.Pending)
            return Result.Failure("Mentoring proposal status already set");

        mentoringProposal.Status = request.IsClosed ? MentoringProposalStatus.Closed : MentoringProposalStatus.Pending;

        _mapper.Map(request, mentoringProposal);
        _unitOfWork.MentoringProposals.Update(mentoringProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update mentoring proposal: {ex.Message}");
        }
    }

    /// <summary>
    /// Mentor can either accept or reject the mentoring proposal once while the status is pending
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result> MentorUpdateAsync(Guid id, MentorUpdateMentoringProposalRequest request)
    {
        var mentoringProposal = await _unitOfWork.MentoringProposals.FindByIdAsync(id);
        if (mentoringProposal == null)
            return Result.Failure("Mentoring proposal not found");
        if (mentoringProposal.Status != MentoringProposalStatus.Pending)
            return Result.Failure("Mentoring proposal status already set");

        var project = await _unitOfWork.Projects.FindByIdAsync(mentoringProposal.ProjectId);
        if (project == null)
            return Result.Failure("Project not found");
        if (!project.MentorId.IsNullOrGuidEmpty())
            return Result.Failure("Project already has a mentor");

        if (request.IsAccepted.HasValue)
        {
            if (request.IsAccepted.Value)
            {
                mentoringProposal.Status = MentoringProposalStatus.Accepted;
                project.MentorId = mentoringProposal.MentorId;
                _unitOfWork.Projects.Update(project);
            }
            else
            {
                mentoringProposal.Status = MentoringProposalStatus.Rejected;
            }
        }

        _mapper.Map(request, mentoringProposal);
        _unitOfWork.MentoringProposals.Update(mentoringProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update mentoring proposal: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var mentoringProposal = await _unitOfWork.MentoringProposals.FindByIdAsync(id);
        if (mentoringProposal == null)
            return Result.Failure("mentoring proposal not found");

        _unitOfWork.MentoringProposals.Delete(mentoringProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete mentoring proposal: {ex.Message}");
        }
    }
}