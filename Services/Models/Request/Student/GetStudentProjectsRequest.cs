using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Student;

public class GetStudentProjectsRequest : PaginationQuery
{
    [BindNever] public Guid? StudentId { get; set; }
    public Guid? TermId { get; set; }
    public ProjectStatus? Status { get; set; }
}