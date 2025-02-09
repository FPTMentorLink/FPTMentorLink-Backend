using AutoMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class GroupService : IGroupService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GroupService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GroupDto>> GetByIdAsync(Guid id)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(id);
        if (group == null)
            return Result.Failure<GroupDto>("Group not found");

        return Result.Success(_mapper.Map<GroupDto>(group));
    }

    public async Task<Result<PaginationResult<GroupDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Groups.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Group, GroupDto>(paginationParams, _mapper.ConfigurationProvider);
            
        return Result.Success(result);
    }

    public async Task<Result<GroupDto>> CreateAsync(CreateGroupDto dto)
    {
        var group = _mapper.Map<Group>(dto);
        _unitOfWork.Groups.Add(group);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<GroupDto>(group));
        }
        catch (Exception ex)
        {
            return Result.Failure<GroupDto>($"Failed to create group: {ex.Message}");
        }
    }

    public async Task<Result<GroupDto>> UpdateAsync(Guid id, UpdateGroupDto dto)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(id);
        if (group == null)
            return Result.Failure<GroupDto>("Group not found");

        _mapper.Map(dto, group);
        _unitOfWork.Groups.Update(group);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<GroupDto>(group));
        }
        catch (Exception ex)
        {
            return Result.Failure<GroupDto>($"Failed to update group: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var group = await _unitOfWork.Groups.GetByIdAsync(id);
        if (group == null)
            return Result.Failure("Group not found");

        _unitOfWork.Groups.Delete(group);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete group: {ex.Message}");
        }
    }
} 