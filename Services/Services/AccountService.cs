using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repositories.Entities;
using Repositories.UnitOfWork.Interfaces;
using Services.DTOs;
using Services.Interfaces;
using Services.Utils;

namespace Services.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtSettings _jwtSettings;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AccountDto>> GetByIdAsync(Guid id)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        if (account == null)
            return Result.Failure<AccountDto>("Account not found");

        return Result.Success(_mapper.Map<AccountDto>(account));
    }

    public async Task<Result<AccountDto>> CreateAsync(CreateAccountDto dto)
    {
        if (await _unitOfWork.Accounts.AnyAsync(a => a.Email == dto.Email))
            return Result.Failure<AccountDto>("Email already exists");

        var account = _mapper.Map<Account>(dto);
        account.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        account.Roles = [dto.Role];

        _unitOfWork.Accounts.Add(account);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(_mapper.Map<AccountDto>(account));
    }

    public async Task<Result<AccountDto>> UpdateAsync(Guid id, UpdateAccountDto dto)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(id);
        if (account == null)
            return Result.Failure<AccountDto>("Account not found");

        _mapper.Map(dto, account);
        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(_mapper.Map<AccountDto>(account));
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
            new(ClaimTypes.Email, account.Email)
        };

        foreach (var role in account.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        var accessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, claims);
        var refreshToken = TokenGenerator.GenerateRefreshToken();
        
        var response = _mapper.Map<LoginResponse>(account);
        response.AccessToken = accessToken;
        response.RefreshToken = refreshToken;

        return Result.Success(response);
    }

    public async Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var principal = TokenGenerator.GetPrincipalFromExpiredToken(_jwtSettings, request.AccessToken);
        if (principal == null)
            return Result.Failure<TokenResponse>("Invalid token");

        var userId = principal.GetUserId();
        if (userId == null)
            return Result.Failure<TokenResponse>("Invalid token");

        var claims = principal.Claims.ToList();
        var newAccessToken = TokenGenerator.GenerateAccessToken(_jwtSettings, claims);
        var newRefreshToken = TokenGenerator.GenerateRefreshToken();
        
        // TODO: implement refresh token invalidation and database update logic
        await Task.Delay(100);

        return Result.Success(new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    public async Task<Result<AccountDto>> RegisterAsync(RegisterRequest request)
    {
        if (await _unitOfWork.Accounts.AnyAsync(a => a.Email == request.Email))
            return Result.Failure<AccountDto>("Email already exists");

        var account = _mapper.Map<Account>(request);
        account.PasswordHash = _passwordHasher.HashPassword(request.Password);
        account.Roles = [request.Role];

        _unitOfWork.Accounts.Add(account);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(_mapper.Map<AccountDto>(account));
    }

    public async Task<Result> IsEmailUniqueAsync(string email)
    {
        var exists = await _unitOfWork.Accounts.AnyAsync(a => a.Email == email);
        return exists ? Result.Failure("Email already exists") : Result.Success();
    }

    public async Task<Result<PaginationResult<AccountDto>>> GetPagedAsync(PaginationParams paginationParams)
    {
        var query = _unitOfWork.Accounts.GetQueryable();
        var result = await query.ProjectToPaginatedListAsync<Account, AccountDto>(paginationParams, _mapper.ConfigurationProvider);
        
        return Result.Success(result);
    }

    public async Task<Result<int>> GetTotalAsync(AccountRole[] roles)
    {
        var query = _unitOfWork.Accounts.GetQueryable();
        if (roles.Length > 0)
        {
            query = query.Where(a => a.Roles.Any(roles.Contains));
        }
        
        var total = await query.CountAsync();
        return Result.Success(total);
    }
}