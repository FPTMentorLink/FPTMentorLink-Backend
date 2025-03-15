using Services.Models.Request.WeeklyReport;
using Services.Models.Response.WeeklyReport;
using Services.Utils;

namespace Services.Interfaces;

public interface IWeeklyReportService
{
    Task<Result<WeeklyReportResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<WeeklyReportResponse>>> GetPagedAsync(GetWeeklyReportRequest paginationParams);
    Task<Result<WeeklyReportResponse>> CreateAsync(CreateWeeklyReportRequest request);
    Task<Result<WeeklyReportResponse>> UpdateAsync(Guid id, UpdateWeeklyReportRequest request);
    Task<Result> DeleteAsync(Guid id);
} 