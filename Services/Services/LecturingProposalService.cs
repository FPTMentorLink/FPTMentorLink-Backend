using System.Linq.Expressions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.LecturingProposal;
using Services.Models.Response.LecturingProposal;
using Services.Utils;

namespace Services.Services;

public class LecturingProposalService : ILecturingProposalService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public LecturingProposalService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LecturingProposalResponse>> GetByIdAsync(Guid id)
    {
        var lecturingProposal = await _unitOfWork.LecturingProposals.FindAll()
            .Include(x => x.Project)
            .Include(x => x.Lecturer)
            .ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (lecturingProposal == null)
            return Result.Failure<LecturingProposalResponse>("lecturing proposal not found");

        return Result.Success(_mapper.Map<LecturingProposalResponse>(lecturingProposal));
    }

    public async Task<Result<PaginationResult<LecturingProposalResponse>>> GetPagedAsync(
        GetLecturingProposalsRequest request)
    {
        var query = _unitOfWork.LecturingProposals.FindAll()
            .Include(x => x.Project)
            .Include(x => x.Lecturer)
            .ThenInclude(x => x.Account).AsQueryable();
        Expression<Func<LecturingProposal, bool>> condition = x => true;
        if (!request.ProjectId.IsNullOrGuidEmpty())
            condition.CombineAndAlsoExpressions(x => x.ProjectId == request.ProjectId);
        if (!request.LecturerId.IsNullOrGuidEmpty())
            condition.CombineAndAlsoExpressions(x => x.LecturerId == request.LecturerId);

        if (!request.StudentId.IsNullOrGuidEmpty())
        {
            query = query.Include(x => x.Project).ThenInclude(x => x.ProjectStudents);
            condition.CombineAndAlsoExpressions(x =>
                x.Project.ProjectStudents.Any(y => y.StudentId == request.StudentId));
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<LecturingProposal, LecturingProposalResponse>(request);

        return Result.Success(result);
    }

    private static IQueryable<LecturingProposal> ApplySorting(IQueryable<LecturingProposal> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, LecturingProposal.GetSortValue(orderBy))
        };
    }

    public async Task<Result> CreateAsync(CreateLecturingProposalRequest request)
    {
        var isExistProject = await _unitOfWork.Projects.AnyAsync(x => x.Id == request.ProjectId);
        if (!isExistProject)
            return Result.Failure("Project not found");
        var isExistLecturer =
            await _unitOfWork.Accounts
                .FindAll(x => x.Email == request.Email && x.Role == AccountRole.Lecturer)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        if (isExistLecturer == Guid.Empty)
            return Result.Failure("Lecturer not found");
        var isExistProposal = await _unitOfWork.LecturingProposals.AnyAsync(x =>
            x.ProjectId == request.ProjectId && x.LecturerId == isExistLecturer);
        if (isExistProposal)
            return Result.Failure("Lecturing proposal already exists");

        var lecturingProposal = _mapper.Map<LecturingProposal>(request);
        lecturingProposal.LecturerId = isExistLecturer;
        // Default status is pending for mentor to accept
        lecturingProposal.Status = LecturingProposalStatus.Pending;
        _unitOfWork.LecturingProposals.Add(lecturingProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create lecturing proposal: {ex.Message}");
        }
    }

    /// <summary>
    /// Student can only set the status to closed once while the status is pending
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result> StudentUpdateAsync(Guid id, StudentUpdateLecturingProposalRequest request)
    {
        var lecturingProposal = await _unitOfWork.LecturingProposals.FindByIdAsync(id);
        if (lecturingProposal == null)
            return Result.Failure("Lecturing proposal not found");

        if (lecturingProposal.Status != LecturingProposalStatus.Pending)
            return Result.Failure("Lecturing proposal status already set");

        lecturingProposal.Status = request.IsClosed ? LecturingProposalStatus.Closed : LecturingProposalStatus.Pending;

        _mapper.Map(request, lecturingProposal);
        _unitOfWork.LecturingProposals.Update(lecturingProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update lecturing proposal: {ex.Message}");
        }
    }

    /// <summary>
    /// Mentor can either accept or reject the lecturing proposal once while the status is pending
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result> LecturerUpdateAsync(Guid id, LecturerUpdateLecturingProposalRequest request)
    {
        var lecturingProposal = await _unitOfWork.LecturingProposals.FindByIdAsync(id);
        if (lecturingProposal == null)
            return Result.Failure("Lecturing proposal not found");
        if (lecturingProposal.Status != LecturingProposalStatus.Pending)
            return Result.Failure("Lecturing proposal status already set");

        var project = await _unitOfWork.Projects.FindByIdAsync(lecturingProposal.ProjectId);
        if (project == null)
            return Result.Failure("Project not found");
        if (!project.LecturerId.IsNullOrGuidEmpty())
            return Result.Failure("Project already has a mentor");

        if (request.IsAccepted.HasValue)
        {
            if (request.IsAccepted.Value)
            {
                lecturingProposal.Status = LecturingProposalStatus.Accepted;
                project.LecturerId = lecturingProposal.LecturerId;
                _unitOfWork.Projects.Update(project);
            }
            else
            {
                lecturingProposal.Status = LecturingProposalStatus.Rejected;
            }
        }

        _mapper.Map(request, lecturingProposal);
        _unitOfWork.LecturingProposals.Update(lecturingProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update lecturing proposal: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var lecturingProposal = await _unitOfWork.LecturingProposals.FindByIdAsync(id);
        if (lecturingProposal == null)
            return Result.Failure("lecturing proposal not found");

        _unitOfWork.LecturingProposals.Delete(lecturingProposal);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete lecturing proposal: {ex.Message}");
        }
    }
}