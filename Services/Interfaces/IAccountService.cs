using Microsoft.AspNetCore.Http;
using Repositories.Entities;
using Services.Models.Request.Account;
using Services.Models.Request.Authorization;
using Services.Models.Response.Account;
using Services.Models.Response.Authentication;
using Services.Models.Response.Authorization;
using Services.Utils;

namespace Services.Interfaces;

public interface IAccountService
{
    Task<Result<AccountResponse>> GetByIdAsync(Guid id,CancellationToken cancellationToken);
    Task<Result> CreateAsync(CreateAccountRequest request,CancellationToken cancellationToken);
    Task<Result> UpdateAsync(Guid id, UpdateAccountRequest request,CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id,CancellationToken cancellationToken);
    Task<Result<LoginResponse>> LoginAsync(string email, string password);
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result> IsEmailUniqueAsync(string email);
    Task<Result<PaginationResult<AccountResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<int>> GetTotalAsync(AccountRole[] roles);
    Task<Result> ImportAccountsAsync(IFormFile formFile, CancellationToken cancellationToken);
}