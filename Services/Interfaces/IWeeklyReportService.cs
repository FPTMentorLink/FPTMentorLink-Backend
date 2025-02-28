using Services.Models.Request.WeeklyReport;
using Services.Models.Response.WeeklyReport;
using Services.Utils;

namespace Services.Interfaces;

public interface IWeeklyReportService
{
    Task<Result<WeeklyReportResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<WeeklyReportResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<WeeklyReportResponse>> CreateAsync(CreateWeeklyReportRequest dto);
    Task<Result<WeeklyReportResponse>> UpdateAsync(Guid id, UpdateWeeklyReportRequest dto);
    Task<Result> DeleteAsync(Guid id);
} 