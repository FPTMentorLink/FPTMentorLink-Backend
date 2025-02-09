using Repositories.Entities;
using Services.DTOs;
using Services.Utils;

namespace Services.Interfaces;

public interface IAccountService
{
    Task<Result<AccountDto>> GetByIdAsync(Guid id);
    Task<Result<AccountDto>> CreateAsync(CreateAccountDto dto);
    Task<Result<AccountDto>> UpdateAsync(Guid id, UpdateAccountDto dto);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<LoginResponse>> LoginAsync(string email, string password);
    Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result<AccountDto>> RegisterAsync(RegisterRequest request);
    Task<Result> IsEmailUniqueAsync(string email);
    Task<Result<PaginationResult<AccountDto>>> GetPagedAsync(PaginationParams paginationParams);
    Task<Result<int>> GetTotalAsync(AccountRole[] roles);
}