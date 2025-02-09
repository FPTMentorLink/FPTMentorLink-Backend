using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IGroupService
{
    Task<Result<GroupDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<GroupDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<GroupDto>> CreateAsync(CreateGroupDto dto);
    Task<Result<GroupDto>> UpdateAsync(Guid id, UpdateGroupDto dto);
    Task<Result> DeleteAsync(Guid id);
} 