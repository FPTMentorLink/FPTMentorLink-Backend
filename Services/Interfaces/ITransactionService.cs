using Services.Models.Request.Transaction;
using Services.Models.Response.Transaction;
using Services.Utils;

namespace Services.Interfaces;

public interface ITransactionService
{
    Task<Result<PaginationResult<TransactionResponse>>> GetPagedAsync(GetTransactionsRequest request);
}