using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Lecturer;
using Services.Models.Response.Lecturer;
using Services.Utils;

namespace Services.Services;

public class LecturerService : ILecturerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LecturerService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PaginationResult<GetLecturerResponse>>> GetLecturersPagedAsync(GetLecturersRequest request)
    {
        try
        {
            var query = _unitOfWork.Lecturers.GetQueryable()
                .Include(l => l.Account)
                .AsNoTracking();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(l =>
                    l.Code.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                query = query.Where(l => l.Code.Contains(request.Code, StringComparison.CurrentCultureIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.FacultyId.ToString()))
            {
                query = query.Where(l => l.FacultyId == request.FacultyId);
            }

            // Apply ordering
            query = ApplySorting(query, request);
            return await query.ProjectToPaginatedListAsync<Lecturer, GetLecturerResponse>(request);
        }
        catch (Exception ex)
        {
            return Result.Failure<PaginationResult<GetLecturerResponse>>($"Error retrieving lecturers: {ex.Message}");
        }
    }

    private static IQueryable<Lecturer> ApplySorting(IQueryable<Lecturer> query, PaginationParams paginationParams)
    {
        return paginationParams.OrderBy.ToLower().Replace(" ", "") switch
        {
            "code" => paginationParams.IsDescending
                ? query.OrderByDescending(l => l.Code)
                : query.OrderBy(l => l.Code),
            _ => paginationParams.IsDescending
                ? query.OrderByDescending(l => l.CreatedAt)
                : query.OrderByDescending(l => l.CreatedAt)
        };
    }
}