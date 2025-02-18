using Repositories.Entities;

namespace Services.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    public Guid AccountId { get; set; }
    public required string TransactionMethod { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTransactionDto
{
    public required string Code { get; set; }
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    public Guid AccountId { get; set; }
    public required string TransactionMethod { get; set; }
    public TransactionStatus Status { get; set; }
}

public class UpdateTransactionDto
{
    public string? Code { get; set; }
    public TransactionType? Type { get; set; }
    public int? Amount { get; set; }
    public string? TransactionMethod { get; set; }
}

public class UpdateTransactionStatus
{
    public TransactionStatus? Status { get; set; }
}