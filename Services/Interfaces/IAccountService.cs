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
    Task<Result<AccountResponse>> GetByIdAsync(Guid id);
    Task<Result> CreateAsync(CreateAccountRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateAccountRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<LoginResponse>> LoginAsync(string email, string password);
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result> IsEmailUniqueAsync(string email);
    Task<Result<PaginationResult<AccountResponse>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<int>> GetTotalAsync(AccountRole[] roles);
}