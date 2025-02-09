using AutoMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentDto>> GetByIdAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentDto>("Appointment not found");

        return Result.Success(_mapper.Map<AppointmentDto>(appointment));
    }

    public async Task<Result<PaginationResult<AppointmentDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Appointments.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Appointment, AppointmentDto>(paginationParams, _mapper.ConfigurationProvider);
            
        return Result.Success(result);
    }

    public async Task<Result<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
    {
        var appointment = _mapper.Map<Appointment>(dto);
        _unitOfWork.Appointments.Add(appointment);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Failed to create appointment: {ex.Message}");
        }
    }

    public async Task<Result<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentDto>("Appointment not found");

        _mapper.Map(dto, appointment);
        _unitOfWork.Appointments.Update(appointment);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (Exception ex)
        {
            return Result.Failure<AppointmentDto>($"Failed to update appointment: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Result.Failure("Appointment not found");

        _unitOfWork.Appointments.Delete(appointment);
        
        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete appointment: {ex.Message}");
        }
    }
}