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
        return Result.Success(_mapper.Map<ProjectDetailResponse>(project));
    }

    public async Task<Result<PaginationResult<ProjectResponse>>> GetPagedAsync(GetProjectsRequest request)
    {
        var query = _unitOfWork.Projects.GetQueryable()
            .Include(x => x.Term)
            .Include(x => x.Faculty)
            // Include project students to prevent N+1 problem on condition checking
            .Include(x => x.ProjectStudents)
            .AsQueryable();

        Expression<Func<Project, bool>> condition = x => true;

        if (request.TermId.HasValue && request.TermId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.TermId == request.TermId);

        if (request.FacultyId.HasValue && request.FacultyId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.FacultyId == request.FacultyId);

        if (request.MentorId.HasValue && request.MentorId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.MentorId == request.MentorId);

        if (request.LecturerId.HasValue && request.LecturerId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.LecturerId == request.LecturerId);

        if (request.StudentId.HasValue && request.StudentId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition,
                x => x.ProjectStudents.Any(y => y.StudentId == request.StudentId));


        if (request.Status.HasValue)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.Status == request.Status);

        query = query.Where(condition);

        var result =
            await query.ProjectToPaginatedListAsync<Project, ProjectResponse>(request);

        return Result.Success(result);
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