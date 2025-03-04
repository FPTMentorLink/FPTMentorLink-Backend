using System.Linq.Expressions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.ProjectStudent;
using Services.Models.Response.ProjectStudent;
using Services.Settings;
using Services.Utils;

namespace Services.Services;

public class ProjectStudentService : IProjectStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;

    public ProjectStudentService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService,
        IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<ProjectStudentResponse>> GetByIdAsync(Guid id)
    {
        var projectStudent = await _unitOfWork.ProjectStudents.FindAll()
            .Include(x => x.Student).ThenInclude(x => x.Account)
            .ProjectToType<ProjectStudentResponse>(_mapper.Config)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (projectStudent == null)
            return Result.Failure<ProjectStudentResponse>("Project student not found");

        return Result.Success(projectStudent);
    }

    public async Task<Result<PaginationResult<ProjectStudentResponse>>> GetPagedAsync(GetProjectStudentsRequest request)
    {
        var query = _unitOfWork.ProjectStudents.FindAll()
            .Include(x => x.Student).ThenInclude(x => x.Account).AsQueryable();

        Expression<Func<ProjectStudent, bool>> condition = x => true;

        if (request.StudentId.HasValue && request.StudentId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.StudentId == request.StudentId);

        if (request.ProjectId.HasValue && request.ProjectId != Guid.Empty)
            condition = Helper.CombineAndAlsoExpressions(condition, x => x.ProjectId == request.ProjectId);

        query = query.Where(condition);

        var result = await query.ProjectToPaginatedListAsync<ProjectStudent, ProjectStudentResponse>(request);

        return Result.Success(result);
    }

    // public async Task<Result> CreateAsync(CreateProjectStudentRequest request)
    // {
    //     var projectStudent = _mapper.Map<ProjectStudent>(request);
    //
    //     _unitOfWork.ProjectStudents.Add(projectStudent);
    //
    //     await _unitOfWork.SaveChangesAsync();
    //
    //     try
    //     {
    //         await _unitOfWork.SaveChangesAsync();
    //         return Result.Success();
    //     }
    //     catch (Exception ex)
    //     {
    //         return Result.Failure($"Failed to create project student: {ex.Message}");
    //     }
    // }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var projectStudent = await _unitOfWork.ProjectStudents.FindByIdAsync(id);
        if (projectStudent == null)
            return Result.Failure("Project student not found");

        _unitOfWork.ProjectStudents.Delete(projectStudent);
        await _unitOfWork.SaveChangesAsync();

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete project student: {ex.Message}");
        }
    }

    public async Task<Result> SendProjectInvitationAsync(SendProjectInvitationRequest request)
    {
        var student = await _unitOfWork.Students.FindAll()
            .Include(x => x.Account)
            .Include(x => x.ProjectStudent)
            .Where(x => x.Id == request.StudentId)
            .Select(x => new
            {
                x.Account.Email,
                x.IsGraduated,
                IsProjectStudent = x.ProjectStudent != null
            }).FirstOrDefaultAsync();

        // Check if student is valid
        if (student == null)
            return Result.Failure("Student not found");
        if (student.IsGraduated)
            return Result.Failure("Student is graduated");
        if (student.IsProjectStudent)
            return Result.Failure("Student is already a member of a project");
        var projectName = await _unitOfWork.Projects.FindAll()
            .Where(x => x.Id == request.ProjectId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync();
        if (projectName == null)
            return Result.Failure("Project not found");

        // Generate invitation token and send email
        var invitationToken =
            TokenGenerator.GenerateInvitationToken(_jwtSettings, request.StudentId, request.ProjectId);
        var emailContent = EmailPresets.ProjectInvitationEmail(invitationToken, projectName);
        await _emailService.SendEmailAsync(student.Email, emailContent);

        return Result.Success();
    }

    public async Task<Result> AcceptProjectInvitationAsync(AcceptProjectInvitationRequest request)
    {
        var principal = TokenGenerator.GetPrincipalFromToken(_jwtSettings, request.Token);
        if (principal == null)
            return Result.Failure("Invalid token");
        var studentId = principal.GetGuid("StudentId");
        var projectId = principal.GetGuid("ProjectId");
        if (studentId == null || projectId == null)
            return Result.Failure("Invalid token");

        var student = await _unitOfWork.Students.FindAll()
            .Include(x => x.Account)
            .Include(x => x.ProjectStudent)
            .Where(x => x.Id == studentId)
            .Select(x => new
            {
                x.Account.Email,
                x.IsGraduated,
                IsProjectStudent = x.ProjectStudent != null
            }).FirstOrDefaultAsync();

        // Check if student is valid
        if (student == null)
            return Result.Failure("Student not found");
        if (student.IsGraduated)
            return Result.Failure("Student is graduated");
        if (student.IsProjectStudent)
            return Result.Failure("Student is already a member of a project");

        // Check if project is valid
        var project = await _unitOfWork.Projects.FindAll()
            .Include(x => x.ProjectStudents)
            .Where(x => x.Id == projectId)
            .Select(x => new
            {
                StudentCount = x.ProjectStudents.Count,
                x.Status
            }).FirstOrDefaultAsync();

        if (project == null)
            return Result.Failure("Project not found");
        if (project.Status > ProjectStatus.Pending)
            return Result.Failure("Project is not available");
        if (project.StudentCount >= Constants.MaxProjectStudents)
            return Result.Failure("Project is full");

        // Create project student
        var projectStudent = new ProjectStudent
        {
            ProjectId = projectId.Value,
            StudentId = studentId.Value,
            IsLeader = false
        };
        _unitOfWork.ProjectStudents.Add(projectStudent);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }
}