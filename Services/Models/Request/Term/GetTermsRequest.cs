using Repositories.Entities;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Term;

public class GetTermsRequest : PaginationQuery
{
    public TermStatus? Status { get; set; }
}