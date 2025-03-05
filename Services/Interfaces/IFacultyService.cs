using Services.Models.Request.Faculty;
using Services.Models.Response.Faculty;
using Services.Utils;

namespace Services.Interfaces;

public interface IFacultyService
{
    Task<Result<FacultyResponse>> GetByIdAsync(Guid id);
    Task<Result<PaginationResult<FacultyResponse>>> GetPagedAsync(GetFacultiesRequest request);
    Task<Result> CreateAsync(CreateFacultyRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateFacultyRequest request);
    Task<Result> DeleteAsync(Guid id);
}