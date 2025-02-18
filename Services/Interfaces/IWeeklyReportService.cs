using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IWeeklyReportService
{
    Task<Result<WeeklyReportDto>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<WeeklyReportDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<WeeklyReportDto>> CreateAsync(CreateWeeklyReportDto dto);
    Task<Result<WeeklyReportDto>> UpdateAsync(Guid id, UpdateWeeklyReportDto dto);
    Task<Result> DeleteAsync(Guid id);
} 