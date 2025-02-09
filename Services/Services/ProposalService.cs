using AutoMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class ProposalService : IProposalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProposalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProposalDto>> GetByIdAsync(Guid id)
    {
        var proposal = await _unitOfWork.Proposals.GetByIdAsync(id);
        if (proposal == null)
            return Result.Failure<ProposalDto>("Proposal not found");

        return Result.Success(_mapper.Map<ProposalDto>(proposal));
    }

    public async Task<Result<PaginationResult<ProposalDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Proposals.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Proposal, ProposalDto>(paginationParams, _mapper.ConfigurationProvider);
            
        return Result.Success(result);
    }

    public async Task<Result<ProposalDto>> CreateAsync(CreateProposalDto dto)
    {
        var proposal = _mapper.Map<Proposal>(dto);
        _unitOfWork.Proposals.Add(proposal);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ProposalDto>(proposal));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProposalDto>($"Failed to create proposal: {ex.Message}");
        }
    }

    public async Task<Result<ProposalDto>> UpdateAsync(Guid id, UpdateProposalDto dto)
    {
        var proposal = await _unitOfWork.Proposals.GetByIdAsync(id);
        if (proposal == null)
            return Result.Failure<ProposalDto>("Proposal not found");

        _mapper.Map(dto, proposal);
        _unitOfWork.Proposals.Update(proposal);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ProposalDto>(proposal));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProposalDto>($"Failed to update proposal: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var proposal = await _unitOfWork.Proposals.GetByIdAsync(id);
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