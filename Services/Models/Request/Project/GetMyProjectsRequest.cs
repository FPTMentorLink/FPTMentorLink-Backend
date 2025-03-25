using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Project;

public class GetMyProjectsRequest : PaginationQuery
{
    [BindNever] public Guid? AccountId { get; set; }
    public Guid? TermId { get; set; }
    public ProjectStatus? Status { get; set; }
}