using Services.Models.Request.Base;

namespace Services.Models.Request.Lecturer;

public class GetLecturersRequest : PaginationQuery
{
    public string? Code { get; set; }
    public Guid? FacultyId { get; set; }
}