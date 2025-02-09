using AutoMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class TaskLogService : ITaskLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TaskLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TaskLogDto>> GetByIdAsync(Guid id)
    {
        var taskLog = await _unitOfWork.TaskLogs.GetByIdAsync(id);
        if (taskLog == null)
            return Result.Failure<TaskLogDto>("Task log not found");

        return Result.Success(_mapper.Map<TaskLogDto>(taskLog));
    }

    public async Task<Result<PaginationResult<TaskLogDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.TaskLogs.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<TaskLog, TaskLogDto>(paginationParams, _mapper.ConfigurationProvider);
            
        return Result.Success(result);
    }

    public async Task<Result<TaskLogDto>> CreateAsync(CreateTaskLogDto dto)
    {
        var taskLog = _mapper.Map<TaskLog>(dto);
        _unitOfWork.TaskLogs.Add(taskLog);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<TaskLogDto>(taskLog));
        }
        catch (Exception ex)
        {
            return Result.Failure<TaskLogDto>($"Failed to create task log: {ex.Message}");
        }
    }

    public async Task<Result<TaskLogDto>> UpdateAsync(Guid id, UpdateTaskLogDto dto)
    {
        var taskLog = await _unitOfWork.TaskLogs.GetByIdAsync(id);
        if (taskLog == null)
            return Result.Failure<TaskLogDto>("Task log not found");

        _mapper.Map(dto, taskLog);
        _unitOfWork.TaskLogs.Update(taskLog);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<TaskLogDto>(taskLog));
        }
        catch (Exception ex)
        {
            return Result.Failure<TaskLogDto>($"Failed to update task log: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var taskLog = await _unitOfWork.TaskLogs.GetByIdAsync(id);
        if (taskLog == null)
            return Result.Failure("Task log not found");

        _unitOfWork.TaskLogs.Delete(taskLog);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete task log: {ex.Message}");
        }
    }
} 