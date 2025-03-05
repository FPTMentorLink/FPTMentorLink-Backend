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

    public async Task<Result<PaginationResult<WeeklyReportResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.WeeklyReports.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<WeeklyReport, WeeklyReportResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<WeeklyReportResponse>> CreateAsync(CreateWeeklyReportRequest request)
    {
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

        _mapper.Map(request, weeklyReport);
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