using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Term;

public class GetTermsRequest : PaginationQuery
{
    public TermStatus? Status { get; set; }
}