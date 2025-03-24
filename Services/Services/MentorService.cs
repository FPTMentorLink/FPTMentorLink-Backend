using MapsterMapper;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Request.Mentor;
using Services.Models.Response.Appointment;
using Services.Models.Response.Mentor;
using Services.Utils;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;

namespace Services.Services;

public class MentorService : IMentorService
{
    private readonly IAppointmentService _appointmentService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MentorService(
        IAppointmentService appointmentService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _appointmentService = appointmentService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<AppointmentResponse>>> GetMyAppointmentPagedAsync(
        GetMentorAppointmentsRequest request)
    {
        return await _appointmentService.GetPagedAsync(new GetAppointmentsRequest
        {
            StudentId = request.MentorId,
            Status = request.Status,
            SearchTerm = request.SearchTerm,
            OrderBy = request.OrderBy,
            IsDescending = request.IsDescending,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        });
    }

    public async Task<Result<PaginationResult<GetMentorResponse>>> GetMentorsPagedAsync(GetMentorsRequest request)
    {
        try
        {
            var query = _unitOfWork.Mentors.GetQueryable()
                .Include(m => m.Account)
                .Include(m => m.Availabilities)
                .AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(m =>
                    m.Code.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim().ToLower();
                query = query.Where(m => m.Code.ToLower().Contains(code));
            }

            if (request.MinBaseSalaryPerHour.HasValue)
            {
                query = query.Where(m => m.BaseSalaryPerHour >= request.MinBaseSalaryPerHour.Value);
            }

            if (request.MaxBaseSalaryPerHour.HasValue)
            {
                query = query.Where(m => m.BaseSalaryPerHour <= request.MaxBaseSalaryPerHour.Value);
            }

            // Apply ordering
            query = ApplySorting(query, request);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var mentorResponses = _mapper.Map<List<GetMentorResponse>>(items);

            return Result.Success(new PaginationResult<GetMentorResponse>(
                mentorResponses,
                request.PageNumber,
                request.PageSize,
                totalItems
            ));
        }
        catch (Exception ex)
        {
            return Result.Failure<PaginationResult<GetMentorResponse>>($"Error retrieving mentors: {ex.Message}");
        }
    }

    /// <summary>
    /// Apply sorting to query
    /// (Custom sorting & default sorting)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="paginationParams"></param>
    /// <returns></returns>
    private static IQueryable<Mentor> ApplySorting(IQueryable<Mentor> query, PaginationParams paginationParams)
    {
        return paginationParams.OrderBy.ToLower().Replace(" ", "") switch
        {
            "code" => paginationParams.IsDescending
                ? query.OrderByDescending(m => m.Code)
                : query.OrderBy(m => m.Code),
            "salary" => paginationParams.IsDescending
                ? query.OrderByDescending(m => m.BaseSalaryPerHour)
                : query.OrderBy(m => m.BaseSalaryPerHour),
            _ => paginationParams.IsDescending
                ? query.OrderByDescending(m => m.CreatedAt)
                : query.OrderByDescending(m => m.CreatedAt)
        };
    }
}