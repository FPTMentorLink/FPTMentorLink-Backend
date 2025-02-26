using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Project;
using Services.Models.Response.Project;
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

    public async Task<Result<ProjectResponse>> GetByIdAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.FindByIdAsync(id);
        if (project == null)
            return Result.Failure<ProjectResponse>("Project not found");

        return Result.Success(_mapper.Map<ProjectResponse>(project));
    }

    public async Task<Result<PaginationResult<ProjectResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Projects.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<Project, ProjectResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateProjectRequest dto)
    {
        var project = _mapper.Map<Project>(dto);
        _unitOfWork.Projects.Add(project);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create project: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateProjectRequest dto)
    {
        var project = await _unitOfWork.Projects.FindByIdAsync(id);
        if (project == null)
            return Result.Failure("Project not found");

        _mapper.Map(dto, project);
        _unitOfWork.Projects.Update(project);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update project: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.FindByIdAsync(id);
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