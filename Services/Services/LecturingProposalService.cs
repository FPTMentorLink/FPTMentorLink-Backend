using MapsterMapper;
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
        var lecturingProposal = await _unitOfWork.LecturingProposals.FindByIdAsync(id);
        if (lecturingProposal == null)
            return Result.Failure<LecturingProposalResponse>("lecturing proposal not found");

        return Result.Success(_mapper.Map<LecturingProposalResponse>(lecturingProposal));
    }

    public async Task<Result<PaginationResult<LecturingProposalResponse>>> GetPagedAsync(
        GetLecturingProposalsRequest request)
    {
        var query = _unitOfWork.LecturingProposals.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<LecturingProposal, LecturingProposalResponse>(request);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateLecturingProposalRequest request)
    {
        var lecturingProposal = _mapper.Map<LecturingProposal>(request);
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