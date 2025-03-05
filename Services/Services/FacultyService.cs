using MapsterMapper;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Faculty;
using Services.Models.Response.Faculty;
using Services.Utils;

namespace Services.Services;

public class FacultyService : IFacultyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FacultyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<FacultyResponse>> GetByIdAsync(Guid id)
    {
        var faculty = await _unitOfWork.Faculties.FindByIdAsync(id);
        if (faculty == null)
            return Result.Failure<FacultyResponse>("Faculty not found");

        return Result.Success(_mapper.Map<FacultyResponse>(faculty));
    }

    public async Task<Result<PaginationResult<FacultyResponse>>> GetPagedAsync(GetFacultiesRequest request)
    {
        var query = _unitOfWork.Faculties.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Faculty, FacultyResponse>(request);

        return Result.Success(result);
    }

    public async Task<Result> CreateAsync(CreateFacultyRequest request)
    {
        var faculty = _mapper.Map<Faculty>(request);
        _unitOfWork.Faculties.Add(faculty);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create faculty: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateFacultyRequest request)
    {
        var faculty = await _unitOfWork.Faculties.FindByIdAsync(id);
        if (faculty == null)
            return Result.Failure("Faculty not found");

        _mapper.Map(request, faculty);
        _unitOfWork.Faculties.Update(faculty);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update faculty: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var faculty = await _unitOfWork.Faculties.FindByIdAsync(id);
        if (faculty == null)
            return Result.Failure("Faculty not found");

        _unitOfWork.Faculties.Delete(faculty);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete faculty: {ex.Message}");
        }
    }
}