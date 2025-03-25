using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Mentor;
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

    public async Task<Result<PaginationResult<TransactionResponse>>> GetPagedAsync(GetTransactionsRequest request)
    {
        var query = _unitOfWork.Transactions.FindAll();
        Expression<Func<Transaction, bool>> condition = x => true;

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Transaction, bool>> searchTermFilter = x =>
                x.Code.Contains(request.SearchTerm) ||
                x.AccountId.ToString().Contains(request.SearchTerm) ||
                x.Description.Contains(request.SearchTerm);
            condition = condition.CombineAndAlsoExpressions(searchTermFilter);
        }

        if (!request.AccountId.IsNullOrGuidEmpty())
        {
            condition = condition.CombineAndAlsoExpressions(x => x.AccountId == request.AccountId);
        }

        if (request.Status.HasValue)
        {
            condition = condition.CombineAndAlsoExpressions(x => x.Status == request.Status);
        }

        if (request.Type.HasValue)
        {
            condition = condition.CombineAndAlsoExpressions(x => x.Type == request.Type);
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<Transaction, TransactionResponse>(request);
        return Result.Success(result);
    }

    public async Task<Result<PaginationResult<TransactionResponse>>> GetMyTransactionsAsync(GetMyTransactionsRequest request)
    {
        var query = _unitOfWork.Transactions.FindAll()
            .Include(x => x.Account)
            .Where(x => x.AccountId == request.AccountId);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(x => 
                x.Code.Contains(request.SearchTerm) || 
                x.Description.Contains(request.SearchTerm));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate.Value);
        }

        query = query.OrderByDescending(x => x.CreatedAt);

        var result = await query.ProjectToPaginatedListAsync<Transaction, TransactionResponse>(request);
        return Result.Success(result);
    }

    private static IQueryable<Transaction> ApplySorting(IQueryable<Transaction> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, Transaction.GetSortValue(orderBy))
        };
    }
}