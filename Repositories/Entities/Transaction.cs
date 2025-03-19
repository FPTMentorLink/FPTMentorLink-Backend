using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Transaction : AuditableEntity
{
    [MaxLength(255)] public required string Code { get; set; }
    public long VnPayTransactionId { get; set; } // Store VnPay transaction Id
    [MaxLength(2000)] public string Description { get; set; } = "";
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    [ForeignKey(nameof(Account))] public Guid AccountId { get; set; }
    [MaxLength(255)] public string TransactionMethod { get; set; } = "VnPay";
    public TransactionStatus Status { get; set; }

    public virtual Account Account { get; set; } = null!;
    
    public static Expression<Func<Transaction, object>> GetSortValue(string sortColumn) =>
        sortColumn.ToLower().Replace(" ", "") switch
        {
            "code" => transaction => transaction.Code,
            "amount" => transaction => transaction.Amount,
            "status" => transaction => transaction.Status,
            "type" => transaction => transaction.Type,
            "createdat" => transaction => transaction.CreatedAt,
            "updatedat" => transaction => transaction.UpdatedAt ?? transaction.CreatedAt,
            _ => transaction => transaction.UpdatedAt ?? transaction.CreatedAt
        };
}

public enum TransactionType
{
    Deposit = 1,
    Withdraw = 2
}

public enum TransactionStatus
{
    Pending = 1,
    Success = 2,
    Failed = 3
}