using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
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

    public async Task<Result<WeeklyReportsDto>> GetByIdAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.GetByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure<WeeklyReportsDto>("Weekly report not found");

        return Result.Success(_mapper.Map<WeeklyReportsDto>(weeklyReport));
    }

    public async Task<Result<PaginationResult<WeeklyReportsDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.WeeklyReports.GetQueryable();
        var result =
            await query.ProjectToPaginatedListAsync<WeeklyReports, WeeklyReportsDto>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<WeeklyReportsDto>> CreateAsync(CreateWeeklyReportsDto dto)
    {
        var weeklyReport = _mapper.Map<WeeklyReports>(dto);
        _unitOfWork.WeeklyReports.Add(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<WeeklyReportsDto>(weeklyReport));
        }
        catch (Exception ex)
        {
            return Result.Failure<WeeklyReportsDto>($"Failed to create weekly report: {ex.Message}");
        }
    }

    public async Task<Result<WeeklyReportsDto>> UpdateAsync(Guid id, UpdateWeeklyReportsDto dto)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.GetByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure<WeeklyReportsDto>("Weekly report not found");

        _mapper.Map(dto, weeklyReport);
        _unitOfWork.WeeklyReports.Update(weeklyReport);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<WeeklyReportsDto>(weeklyReport));
        }
        catch (Exception ex)
        {
            return Result.Failure<WeeklyReportsDto>($"Failed to update weekly report: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.GetByIdAsync(id);
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