using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Transaction;

public class TransactionResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string TransactionCode { get; set; } = null!;
    public TransactionType Type { get; set; }
    public long VnPayTransactionId { get; set; }
    public int Amount { get; set; }
    public Guid AccountId { get; set; }
    public string Description { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string TransactionMethod { get; set; } = null!;
    public TransactionStatus Status { get; set; }
}