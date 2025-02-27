using Services.Models.Request.Base;

namespace Services.Models.Request.ProjectStudent;

public class GetProjectStudentsRequest : PaginationQuery
{
    public Guid? ProjectId { get; set; }
    public Guid? StudentId { get; set; }
}