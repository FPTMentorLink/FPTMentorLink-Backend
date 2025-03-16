using System.Linq.Expressions;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Transaction;
using Services.Models.Response.Transaction;
using Services.Utils;

namespace Services.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;

    public TransactionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginationResult<TransactionResponse>>> GetPagedAsync(GetTransactionsRequest paginationParams)
    {
        Expression<Func<Transaction, bool>> searchTermFilter = transaction => true;
        if(!string.IsNullOrEmpty(paginationParams.SearchTerm))
        {
            searchTermFilter = transaction =>
                transaction.Code.Contains(paginationParams.SearchTerm) ||
                transaction.AccountId.ToString().Contains(paginationParams.SearchTerm)||
                transaction.Account.Email.Contains(paginationParams.SearchTerm); 
        }
        var query = _unitOfWork.Transactions.FindAll(searchTermFilter);
        if (!string.IsNullOrEmpty(paginationParams.OrderBy))
        {
            var sortProperty = GetSortProperty(paginationParams.OrderBy);
            query = query.ApplySorting(paginationParams.IsDescending, sortProperty);
        }
        var result = await query.ProjectToPaginatedListAsync<Transaction , TransactionResponse>(paginationParams);
        return Result.Success(result);
    }
    
    private Expression<Func<Transaction, object>> GetSortProperty(string sortBy) =>
        sortBy.ToLower().Replace(" ", "") switch
        {
            "code" => transaction => transaction.Code,
            "type" => transaction => transaction.Type,
            "amount" => transaction => transaction.Amount,
            "status" => transaction => transaction.Status,
            _ => transaction => transaction.Id
        };
    
}