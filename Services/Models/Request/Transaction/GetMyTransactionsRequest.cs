using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Utils;

namespace Services.Models.Request.Transaction;

public class GetMyTransactionsRequest : PaginationParams
{
    [BindNever] 
    public Guid? AccountId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}