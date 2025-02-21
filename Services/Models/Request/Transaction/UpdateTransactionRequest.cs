using Repositories.Entities;

namespace Services.Models.Request.Transaction;

public class UpdateTransactionRequest
{
    public string? Code { get; set; }
    public TransactionType? Type { get; set; }
    public int? Amount { get; set; }
    public string? TransactionMethod { get; set; }
}