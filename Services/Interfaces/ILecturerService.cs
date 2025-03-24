using Services.Models.Request.Lecturer;
using Services.Models.Response.Lecturer;
using Services.Utils;

namespace Services.Interfaces;

public interface ILecturerService
{
    Task<Result<PaginationResult<GetLecturerResponse>>> GetLecturersPagedAsync(GetLecturersRequest request);
}