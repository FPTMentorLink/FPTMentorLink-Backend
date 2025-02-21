using Repositories.Entities;

namespace Services.Models.Request.Transaction;

public class UpdateTransactionStatusRequest
{
    public TransactionStatus? Status { get; set; }
}