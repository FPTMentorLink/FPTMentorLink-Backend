using Repositories.Entities;

namespace Services.Models.Request.Transaction;

public class CreateTransactionRequest
{
    public required string Code { get; set; }
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    public Guid AccountId { get; set; }
    public required string TransactionMethod { get; set; }
    public TransactionStatus Status { get; set; }
}