using Services.Models.Request.WeeklyReport;
using Services.Models.Response.WeeklyReport;
using Services.Utils;

namespace Services.Interfaces;

public interface IWeeklyReportService
{
    Task<Result<WeeklyReportResponse>> GetByIdAsync(Guid id);
    Task<Result> CreateWeeklyReportAsync(CreateWeeklyReportRequest request);
    Task<Result> UpdateWeeklyReportAsync(Guid id, UpdateWeeklyReportRequest request);
    Task<Result> DeleteWeeklyReportAsync(Guid id);
    Task<Result<PaginationResult<WeeklyReportResponse>>> GetPagedAsync(GetWeeklyReportsRequest request);
}