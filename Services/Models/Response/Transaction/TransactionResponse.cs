using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Transaction;

public class TransactionResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    public Guid AccountId { get; set; }
    public required string TransactionMethod { get; set; }
    public TransactionStatus Status { get; set; }
}