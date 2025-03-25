using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services.Utils;

namespace Services.Models.Request.Student;

public class GetStudentTransactionsRequest : PaginationParams
{
    [BindNever] 
    public Guid? StudentId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}