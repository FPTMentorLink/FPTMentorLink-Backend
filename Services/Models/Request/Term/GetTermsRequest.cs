using Repositories.Entities;
using Services.Utils;

namespace Services.Models.Request.Term;

public class GetTermsRequest : PaginationParams
{
    public TermStatus? Status { get; set; }
}