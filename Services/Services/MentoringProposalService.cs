using MapsterMapper;
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
        var query = _unitOfWork.MentoringProposals.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<MentoringProposal, MentoringProposalResponse>(request);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateMentoringProposalRequest request)
    {
        var mentoringProposal = _mapper.Map<MentoringProposal>(request);
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

        var project = await _unitOfWork.Projects.FindByIdAsNoTrackingAsync(mentoringProposal.ProjectId);
        if (project == null)
            return Result.Failure("Project not found");
        if (!project.MentorId.IsNullOrGuidEmpty())
            return Result.Failure("Project already has a mentor");

        if (request.IsAccepted.HasValue)
            mentoringProposal.Status = request.IsAccepted == true
                ? MentoringProposalStatus.Accepted
                : MentoringProposalStatus.Rejected;

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