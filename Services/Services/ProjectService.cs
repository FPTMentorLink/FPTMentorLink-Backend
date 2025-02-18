using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class ProjectService : IProjectService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ProjectDto>> GetByIdAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project == null)
            return Result.Failure<ProjectDto>("Project not found");

        return Result.Success(_mapper.Map<ProjectDto>(project));
    }

    public async Task<Result<PaginationResult<ProjectDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Projects.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<Project, ProjectDto>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<ProjectDto>> CreateAsync(CreateProjectDto dto)
    {
        var project = _mapper.Map<Project>(dto);
        _unitOfWork.Projects.Add(project);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ProjectDto>(project));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProjectDto>($"Failed to create project: {ex.Message}");
        }
    }

    public async Task<Result<ProjectDto>> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project == null)
            return Result.Failure<ProjectDto>("Project not found");

        _mapper.Map(dto, project);
        _unitOfWork.Projects.Update(project);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<ProjectDto>(project));
        }
        catch (Exception ex)
        {
            return Result.Failure<ProjectDto>($"Failed to update project: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(id);
        if (project == null)
            return Result.Failure("Project not found");

        _unitOfWork.Projects.Delete(project);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete project: {ex.Message}");
        }
    }
}