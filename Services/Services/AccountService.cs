using System.Security.Claims;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.Interfaces;
using Services.Models.Request.Account;
using Services.Models.Request.Authorization;
using Services.Models.Response.Account;
using Services.Models.Response.Authentication;
using Services.Models.Response.Authorization;
using Services.Utils;

namespace Services.Services;

public class AccountService : IAccountService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AccountResponse>> GetByIdAsync(Guid id)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        if (account == null)
            return Result.Failure<AccountResponse>("Account not found");

        return Result.Success(account.Adapt<AccountResponse>());
    }

    public async Task<Result> CreateAsync(CreateAccountRequest request)
    {
        var account = request.Adapt<Account>();
        _unitOfWork.Accounts.Add(account);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to create account: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateAccountRequest request)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        if (account == null)
            return Result.Failure<AccountResponse>("Account not found");

        request.Adapt(account);
        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        if (account == null)
            return Result.Failure("Account not found");

        _unitOfWork.Accounts.Delete(account);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        var account = await _unitOfWork.Accounts.FindSingleAsync(a => a.Email == email);
        if (account == null || !_passwordHasher.VerifyPassword(password, account.PasswordHash))
            return Result.Failure<LoginResponse>("Invalid email or password");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Email, account.Email),
            new(ClaimTypes.Role, account.Role.ToString())
        };
        var accessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, claims);
        var refreshToken = TokenGenerator.GenerateRefreshToken();

        var response = account.Adapt<LoginResponse>();
        response.AccessToken = accessToken;
        response.RefreshToken = refreshToken;

        return Result.Success(response);
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = TokenGenerator.GetPrincipalFromExpiredToken(_jwtSettings, request.AccessToken);
        if (principal == null)
            return Result.Failure<RefreshTokenResponse>("Invalid token");

        var userId = principal.GetUserId();
        if (userId == null)
            return Result.Failure<RefreshTokenResponse>("Invalid token");

        var claims = principal.Claims.ToList();
        var newAccessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, claims);
        var newRefreshToken = TokenGenerator.GenerateRefreshToken();

        // TODO: implement refresh token invalidation and database update logic
        await Task.Delay(100);

        return Result.Success(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    public async Task<Result> IsEmailUniqueAsync(string email)
    {
        var exists = await _unitOfWork.Accounts.AnyAsync(a => a.Email == email);
        return exists ? Result.Failure("Email already exists") : Result.Success();
    }

    public async Task<Result<PaginationResult<AccountResponse>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Accounts.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Account, AccountResponse>(paginationParams);

        return Result.Success(result);
    }

    public async Task<Result<int>> GetTotalAsync(AccountRole[] roles)
    {
        var query = _unitOfWork.Accounts.GetQueryable();
        if (roles.Length > 0) query = query.Where(a => roles.Contains(a.Role));

        var total = await query.CountAsync();
        return Result.Success(total);
    }
}