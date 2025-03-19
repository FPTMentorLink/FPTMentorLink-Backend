using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Transaction;

public class GetTransactionsRequest : PaginationQuery
{
    public Guid? AccountId { get; set; }
    public TransactionStatus? Status { get; set; }
    public TransactionType? Type { get; set; }
}