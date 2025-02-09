using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IWeeklyReportService
{
    Task<Result<WeeklyReportsDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<WeeklyReportsDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<WeeklyReportsDto>> CreateAsync(CreateWeeklyReportsDto dto);
    Task<Result<WeeklyReportsDto>> UpdateAsync(Guid id, UpdateWeeklyReportsDto dto);
    Task<Result> DeleteAsync(Guid id);
} 