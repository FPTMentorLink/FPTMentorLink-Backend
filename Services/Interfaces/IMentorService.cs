using Services.Models.Request.Mentor;
using Services.Models.Response.Mentor;
using Services.Utils;

namespace Services.Interfaces;

public interface IMentorService
{

    Task<Result<PaginationResult<GetMentorResponse>>> GetMentorsPagedAsync(GetMentorsRequest request);
}