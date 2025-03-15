using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.WeeklyReport;
using Services.Models.Response.WeeklyReport;
using Services.Utils;
using System.Linq.Expressions;

namespace Services.Services;

public class WeeklyReportService : IWeeklyReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WeeklyReportService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WeeklyReportResponse>> GetByIdAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.FindByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure<WeeklyReportResponse>("Weekly report not found");

        return Result.Success(_mapper.Map<WeeklyReportResponse>(weeklyReport));
    }

    public async Task<Result<PaginationResult<WeeklyReportResponse>>> GetPagedAsync(GetWeeklyReportRequest request)
    {
        Expression<Func<WeeklyReport, bool>> filter = report => true;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<WeeklyReport, bool>> searchFilter = report =>
                report.Title.Contains(request.SearchTerm) || report.Content.Contains(request.SearchTerm);
            filter = Helper.CombineAndAlsoExpressions(filter, searchFilter);
        }

        if (request.ProjectId.HasValue)
        {
            Expression<Func<WeeklyReport, bool>> projectFilter = report => report.ProjectId == request.ProjectId;
            filter = Helper.CombineAndAlsoExpressions(filter, projectFilter);
        }

        var query = _unitOfWork.WeeklyReports.FindAll(filter);
        if (!string.IsNullOrEmpty(request.OrderBy))
        {
            var sortProperty = GetSortProperty(request.OrderBy);
            query = query.ApplySorting(request.IsDescending, sortProperty);
        }

        var result = await query.ProjectToPaginatedListAsync<WeeklyReport, WeeklyReportResponse>(request);
        return Result.Success(result);
    }

    private Expression<Func<WeeklyReport, object>> GetSortProperty(string sortBy) =>
        sortBy.ToLower().Replace(" ", "") switch
        {
            "createdat" => report => report.CreatedAt,
            "updatedat" => report => report.UpdatedAt!,
            _ => report => report.Id
        };

    public async Task<Result<WeeklyReportResponse>> CreateAsync(CreateWeeklyReportRequest request)
    {
        var project = await _unitOfWork.Projects.FindByIdAsync(request.ProjectId);
        if (project == null)
            return Result.Failure<WeeklyReportResponse>("Project not found");
        var weeklyReport = _mapper.Map<WeeklyReport>(request);
        _unitOfWork.WeeklyReports.Add(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<WeeklyReportResponse>(weeklyReport));
        }
        catch (Exception ex)
        {
            return Result.Failure<WeeklyReportResponse>($"Failed to create weekly report: {ex.Message}");
        }
    }

    public async Task<Result<WeeklyReportResponse>> UpdateAsync(Guid id, UpdateWeeklyReportRequest request)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.FindByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure<WeeklyReportResponse>("Weekly report not found");

        weeklyReport.Title = request.Title ?? weeklyReport.Title;
        weeklyReport.Content = request.Content ?? weeklyReport.Content;
        _unitOfWork.WeeklyReports.Update(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<WeeklyReportResponse>(weeklyReport));
        }
        catch (Exception ex)
        {
            return Result.Failure<WeeklyReportResponse>($"Failed to update weekly report: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.FindByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure("Weekly report not found");

        _unitOfWork.WeeklyReports.Delete(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete weekly report: {ex.Message}");
        }
    }
}