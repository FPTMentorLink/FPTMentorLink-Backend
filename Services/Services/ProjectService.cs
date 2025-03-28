using System.Linq.Expressions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Project;
using Services.Models.Response.Project;
using Services.Models.Response.ProjectStudent;
using Services.StateMachine.Project;
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

    public async Task<Result<ProjectDetailResponse>> GetByIdAsync(Guid id)
    {
        var project = await _unitOfWork.Projects.FindAll()
            .Include(x => x.Term)
            .Include(x => x.Faculty)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (project == null)
            return Result.Failure<ProjectDetailResponse>("Project not found");

        var result = _mapper.Map<ProjectDetailResponse>(project);

        // Get mentor name
        if (result.MentorId.HasValue && result.MentorId != Guid.Empty)
        {
            var mentor = await _unitOfWork.Accounts.FindAll()
                .Where(x => x.Id == result.MentorId)
                .Select(x => new { FullName = x.FirstName + " " + x.LastName })
                .FirstOrDefaultAsync();
            if (mentor != null)
                result.MentorName = mentor.FullName;
        }

        // Get lecturer name
        if (result.LecturerId.HasValue && result.LecturerId != Guid.Empty)
        {
            var lecturer = await _unitOfWork.Accounts.FindAll()
                .Where(x => x.Id == result.LecturerId)
                .Select(x => new { FullName = x.FirstName + " " + x.LastName })
                .FirstOrDefaultAsync();
            if (lecturer != null)
                result.LecturerName = lecturer.FullName;
        }

        // Get project students
        var projectStudent = await _unitOfWork.ProjectStudents.FindAll()
            .Include(x => x.Student).ThenInclude(x => x.Account)
            .Where(x => x.ProjectId == id)
            .ProjectToType<ProjectStudentResponse>(_mapper.Config)
            .ToListAsync();

        result.ProjectStudents = projectStudent;
        return Result.Success(result);
    }

    public async Task<Result<PaginationResult<ProjectResponse>>> GetPagedAsync(GetProjectsRequest request)
    {
        var query = _unitOfWork.Projects.FindAll()
            .Include(x => x.Term)
            .Include(x => x.Faculty)
            // Include project students to prevent N+1 problem on condition checking
            .Include(x => x.ProjectStudents)
            .AsQueryable();

        Expression<Func<Project, bool>> condition = x => true;

        if (request.TermId.HasValue && request.TermId != Guid.Empty)
            condition = condition.CombineAndAlsoExpressions(x => x.TermId == request.TermId);

        if (request.FacultyId.HasValue && request.FacultyId != Guid.Empty)
            condition = condition.CombineAndAlsoExpressions(x => x.FacultyId == request.FacultyId);

        if (request.MentorId.HasValue && request.MentorId != Guid.Empty)
            condition = condition.CombineAndAlsoExpressions(x => x.MentorId == request.MentorId);

        if (request.LecturerId.HasValue && request.LecturerId != Guid.Empty)
            condition = condition.CombineAndAlsoExpressions(x => x.LecturerId == request.LecturerId);

        if (request.StudentId.HasValue && request.StudentId != Guid.Empty)
            condition = condition.CombineAndAlsoExpressions(x => x.ProjectStudents.Any(y => y.StudentId == request.StudentId));


        if (request.Status.HasValue)
            condition = condition.CombineAndAlsoExpressions(x => x.Status == request.Status);

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<Project, ProjectResponse>(request);

        return Result.Success(result);
    }

    /// <summary>
    /// Apply sorting to query
    /// (Custom sorting & default sorting)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="paginationParams"></param>
    /// <returns></returns>
    private static IQueryable<Project> ApplySorting(IQueryable<Project> query, PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            "termcode" => query
                .Include(x => x.Term)
                .ApplySorting(isDescending, x => x.Term.Code),
            _ => query.ApplySorting(isDescending, Project.GetSortValue(orderBy))
        };
    }

    public async Task<Result> CreateAsync(CreateProjectRequest request)
    {
        // Get the faculty
        var faculty = await _unitOfWork.Faculties.FindAll()
            .Where(x => x.Id == request.FacultyId).FirstOrDefaultAsync();
        if (faculty == null)
            return Result.Failure("Faculty not found");

        // Get the next pending term
        var term = await _unitOfWork.Terms.FindAll()
            .Where(x => x.Status == TermStatus.Pending)
            .OrderBy(x => x.StartTime).FirstOrDefaultAsync();
        if (term == null)
            return Result.Failure("Term not found");

        // Count the number of projects in the same faculty 
        var projectNum = await _unitOfWork.Projects.FindAll()
            .Where(x => x.FacultyId == request.FacultyId && x.TermId == term.Id &&
                        (int)x.Status < (int)ProjectStatus.InProgress)
            .CountAsync();
        var project = _mapper.Map<Project>(request);

        project.Id = Guid.NewGuid();
        project.Status = ProjectStatus.Pending;
        project.TermId = term.Id;
        // Generate project code
        // Example: FA24SE103
        project.Code = CodeGenerator.GenerateProjectCode(term.Code, faculty.Code, projectNum + 1);

        _unitOfWork.Projects.Add(project);

        var leader = new ProjectStudent
        {
            ProjectId = project.Id,
            StudentId = request.LeaderId,
            IsLeader = true
        };

        _unitOfWork.ProjectStudents.Add(leader);

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

    public async Task<Result> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _unitOfWork.Projects.FindByIdAsync(id);
        if (project == null)
            return Result.Failure("Project not found");

        _mapper.Map(request, project);
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
    public async Task<Result<PaginationResult<ProjectResponse>>> GetMyProjectPagedAsync(
        GetMyProjectsRequest request, string role)
    {
        var projectRequest = new GetProjectsRequest
        {
            Status = request.Status,
            TermId = request.TermId,
            SearchTerm = request.SearchTerm,
            OrderBy = request.OrderBy,
            IsDescending = request.IsDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        switch (role.ToLower())
        {
            case "student":
                projectRequest.StudentId = request.AccountId;
                break;
            case "mentor":
                projectRequest.MentorId = request.AccountId;
                break;
            case "lecturer":
                projectRequest.LecturerId = request.AccountId;
                break;
            default:
                return Result.Failure<PaginationResult<ProjectResponse>>("Invalid role");
        }

        return await GetPagedAsync(projectRequest);
    }
    public async Task<Result> UpdateStatusAsync(Guid id, AccountRole role, UpdateProjectStatusRequest request)
    {
        var project = await _unitOfWork.Projects.FindByIdAsync(id);
        if (project == null)
            return Result.Failure("Project not found");

        // Validate state transition and role permission
        var validation = ProjectStateManager.ValidateTransition(project.Status, request.Status, role);
        if (!validation.IsSuccess)
            return validation;

        // Use existing switch-case logic for the actual status update
        var result = project.Status switch
        {
            ProjectStatus.Pending => request.Status switch
            {
                ProjectStatus.InProgress => await UpdateToInProgress(project),
                ProjectStatus.Closed => await UpdateToClosed(project),
                _ => Result.Failure("Invalid status")
            },
            ProjectStatus.InProgress => request.Status switch
            {
                ProjectStatus.RevisionRequired => await UpdateToRevisionRequired(project),
                ProjectStatus.Completed => await UpdateToCompleted(project),
                ProjectStatus.Failed => await UpdateToFailed(project),
                ProjectStatus.PendingReview => await UpdateToPendingReview(project),
                ProjectStatus.Closed => await UpdateToClosed(project),
                _ => Result.Failure("Invalid status")
            },
            ProjectStatus.PendingReview => request.Status switch
            {
                ProjectStatus.RevisionRequired => await UpdateToRevisionRequired(project),
                ProjectStatus.Completed => await UpdateToCompleted(project),
                ProjectStatus.Failed => await UpdateToFailed(project),
                ProjectStatus.Closed => await UpdateToClosed(project),
                _ => Result.Failure("Invalid status")
            },
            ProjectStatus.Completed or ProjectStatus.Failed => request.Status switch
            {
                ProjectStatus.Closed => await UpdateToClosed(project),
                _ => Result.Failure("Invalid status")
            },
            ProjectStatus.RevisionRequired => request.Status switch
            {
                ProjectStatus.Completed => await UpdateToCompleted(project),
                ProjectStatus.Failed => await UpdateToFailed(project),
                ProjectStatus.Closed => await UpdateToClosed(project),
                _ => Result.Failure("Invalid status")
            },
            ProjectStatus.Closed => Result.Failure("Project status cannot be updated"),
            _ => Result.Failure("Invalid status")
        };

        if (result.IsFailure) return result;

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    /// <summary>
    /// Update project status to revision required
    /// Not save changes yet
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private Task<Result> UpdateToRevisionRequired(Project project)
    {
        project.Status = ProjectStatus.RevisionRequired;
        _unitOfWork.Projects.Update(project);
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Update project status to failed
    /// Not save changes yet
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private Task<Result> UpdateToFailed(Project project)
    {
        project.Status = ProjectStatus.Failed;
        _unitOfWork.Projects.Update(project);
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Update project status to completed
    /// Not save changes yet
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private Task<Result> UpdateToCompleted(Project project)
    {
        project.Status = ProjectStatus.Completed;
        _unitOfWork.Projects.Update(project);
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Update project status to pending review
    /// Not save changes yet
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private Task<Result> UpdateToPendingReview(Project project)
    {
        project.Status = ProjectStatus.PendingReview;
        _unitOfWork.Projects.Update(project);
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Update project status to closed
    /// Not save changes yet
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private Task<Result> UpdateToClosed(Project project)
    {
        project.Status = ProjectStatus.Closed;
        _unitOfWork.Projects.Update(project);
        return Task.FromResult(Result.Success());
    }

    /// <summary>
    /// Update project status to in progress if there are enough students and lecturer
    /// Not save changes yet
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    private async Task<Result> UpdateToInProgress(Project project)
    {
        var studentCount = await _unitOfWork.ProjectStudents.FindAll()
            .Where(x => x.ProjectId == project.Id).CountAsync();
        if (studentCount < Constants.MinProjectStudents)
            return Result.Failure($"Project must have at least {Constants.MinProjectStudents} students");
        if (project.LecturerId == null)
            return Result.Failure("Project must have a lecturer");
        project.Status = ProjectStatus.InProgress;
        _unitOfWork.Projects.Update(project);
        return Result.Success();
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
