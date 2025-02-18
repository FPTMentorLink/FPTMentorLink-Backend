using AutoMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
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

    public async Task<Result<WeeklyReportDto>> GetByIdAsync(Guid id)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.GetByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure<WeeklyReportDto>("Weekly report not found");

        return Result.Success(_mapper.Map<WeeklyReportDto>(weeklyReport));
    }

    public async Task<Result<PaginationResult<WeeklyReportDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.WeeklyReports.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<WeeklyReport, WeeklyReportDto>(paginationParams, _mapper.ConfigurationProvider);
            
        return Result.Success(result);
    }

    public async Task<Result<WeeklyReportDto>> CreateAsync(CreateWeeklyReportDto dto)
    {
        var weeklyReport = _mapper.Map<WeeklyReport>(dto);
        _unitOfWork.WeeklyReports.Add(weeklyReport);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<WeeklyReportDto>(weeklyReport));
        }
        catch (Exception ex)
        {
            return Result.Failure<WeeklyReportDto>($"Failed to create weekly report: {ex.Message}");
        }
    }

    public async Task<Result<WeeklyReportDto>> UpdateAsync(Guid id, UpdateWeeklyReportDto dto)
    {
        var weeklyReport = await _unitOfWork.WeeklyReports.GetByIdAsync(id);
        if (weeklyReport == null)
            return Result.Failure<WeeklyReportDto>("Weekly report not found");

        _mapper.Map(dto, weeklyReport);
        _unitOfWork.WeeklyReports.Update(weeklyReport);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<WeeklyReportDto>(weeklyReport));
        }
        catch (Exception ex)
        {
            return Result.Failure<WeeklyReportDto>($"Failed to update weekly report: {ex.Message}");
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