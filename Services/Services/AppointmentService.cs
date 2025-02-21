using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Response.Appointment;
using Services.Utils;

namespace Services.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AppointmentResponse>> GetByIdAsync(Guid id)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentResponse>("Appointment not found");

        return Result.Success(_mapper.Map<AppointmentResponse>(appointment));
    }

    public async Task<Result<PaginationResult<AppointmentResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Appointments.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Appointment, AppointmentResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateAppointmentRequest request)
    {
        var appointment = _mapper.Map<Appointment>(request);
        _unitOfWork.Appointments.Add(appointment);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create appointment: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateAppointmentRequest request)
    {
        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Result.Failure<AppointmentResponse>("Appointment not found");

        _mapper.Map(request, appointment);
        _unitOfWork.Appointments.Update(appointment);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update appointment: {ex.Message}");
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