using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Utils;

namespace Services.Models.Request.Mentor;

public class GetMentorTransactionsRequest : PaginationParams
{
    [BindNever] 
    public Guid? MentorId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}