using System.Linq.Expressions;
using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.WeeklyReport;
using Services.Models.Response.WeeklyReport;
using Services.Utils;

namespace Services.Services;

public class WeeklyReportService : IWeeklyReportService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public WeeklyReportService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<WeeklyReportResponse>> GetByIdAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.FindByIdAsync(id);
        if (weeklyReport == null)
        {
            return Result.Failure<WeeklyReportResponse>(DomainError.WeeklyReport.WeeklyReportNotFound);
        }

        return Result.Success(_mapper.Map<WeeklyReportResponse>(weeklyReport));
    }

    public async Task<Result> CreateWeeklyReportAsync(CreateWeeklyReportRequest request)
    {
        var weeklyReport = _mapper.Map<WeeklyReport>(request);
        _unitOfWork.WeeklyReports.Add(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create weekly report: {ex.Message}");
        }
    }

    public async Task<Result> UpdateWeeklyReportAsync(Guid id, UpdateWeeklyReportRequest request)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.FindByIdAsync(id);
        if (weeklyReport == null)
        {
            return Result.Failure(DomainError.WeeklyReport.WeeklyReportNotFound);
        }

        weeklyReport.Title = request.Title ?? weeklyReport.Title;
        weeklyReport.Content = request.Content ?? weeklyReport.Content;
        _unitOfWork.WeeklyReports.Update(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update weekly report: {ex.Message}");
        }
    }

    public async Task<Result> DeleteWeeklyReportAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.FindByIdAsync(id);
        if (weeklyReport == null)
        {
            return Result.Failure(DomainError.WeeklyReport.WeeklyReportNotFound);
        }

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

    public async Task<Result<PaginationResult<WeeklyReportResponse>>> GetPagedAsync(GetWeeklyReportsRequest request)
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
            Expression<Func<WeeklyReport, bool>> projectFilter = report =>
                report.ProjectId == request.ProjectId.Value;
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
            _ => report => report.Id
        };
}