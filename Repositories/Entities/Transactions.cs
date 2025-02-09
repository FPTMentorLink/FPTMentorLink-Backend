using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Transactions : AuditableEntity
{
    [MaxLength(255)] public required string Code { get; set; }
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    [ForeignKey(nameof(Account))] public Guid AccountId { get; set; }
    [MaxLength(255)] public required string TransactionMethod { get; set; }
    public TransactionStatus Status { get; set; }

    public virtual Account Account { get; set; } = null!;
}

public enum TransactionType
{
    Deposit = 1,
    Withdraw = 2,
    Transfer = 3
}

public enum TransactionStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3
}