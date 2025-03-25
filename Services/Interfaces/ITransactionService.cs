using Services.Models.Request.Mentor;
using Services.Models.Request.Transaction;
using Services.Models.Response.Transaction;
using Services.Utils;

namespace Services.Interfaces;

public interface ITransactionService
{
    Task<Result<PaginationResult<TransactionResponse>>> GetPagedAsync(GetTransactionsRequest request);
    Task<Result<PaginationResult<TransactionResponse>>> GetMyTransactionsAsync(GetMyTransactionsRequest request);
}