using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Claims;
using CsvHelper;
using CsvHelper.Configuration;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
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
    private readonly IMapper _mapper;

    public AccountService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IOptions<JwtSettings> jwtSettings,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<Result<AccountResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.FindByIdAsync(id, cancellationToken);
        if (account == null)
            return Result.Failure<AccountResponse>("Account not found");

        return Result.Success(_mapper.Map<AccountResponse>(account));
    }

    public async Task<Result> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var validateAccount = await
            _unitOfWork.Accounts.FindAll().Select(x => new
            {
                IsEmailTaken = x.Email == request.Email,
                IsUsernameTaken = x.Username == request.Username
            }).FirstOrDefaultAsync(x => x.IsEmailTaken || x.IsUsernameTaken, cancellationToken);

        if (validateAccount != null)
        {
            if (validateAccount.IsEmailTaken)
            {
                return Result.Failure(DomainError.Account.EmailExists);
            }

            if (validateAccount.IsUsernameTaken)
            {
                return Result.Failure(DomainError.Account.UsernameExists);
            }
        }

        request.Password = _passwordHasher.HashPassword(request.Password);
        var account = _mapper.Map<Account>(request);
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

    public async Task<Result> UpdateAsync(Guid id, UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.FindByIdAsync(id, cancellationToken);

        if (account == null)
            return Result.Failure<AccountResponse>(DomainError.Account.AccountNotFound);

        account.FirstName = request.FirstName ?? account.FirstName;
        account.LastName = request.LastName ?? account.LastName;
        account.ImageUrl = request.ImageUrl ?? account.ImageUrl;
        account.IsSuspended = request.IsSuspended ?? account.IsSuspended;

        _unitOfWork.Accounts.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.FindByIdAsync(id, cancellationToken);
        if (account == null)
            return Result.Failure(DomainError.Account.AccountNotFound);

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

        var response = _mapper.Map<LoginResponse>(account);
        response.AccessToken = accessToken;
        response.RefreshToken = refreshToken;

        return Result.Success(response);
    }

    public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        // Check if access token is valid when it is expired
        var principal = TokenGenerator.GetPrincipalFromToken(_jwtSettings, request.AccessToken, false);
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

    public async Task<Result> ImportAccountsAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return Result.Failure("File is empty");

        if (!file.FileName.EndsWith(".csv"))
            return Result.Failure("File must be a CSV");

        if (file.Length > Constants.MaxFileSize)
            return Result.Failure("File size exceeds the limit");

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            var records = csv.GetRecords<CsvAccount>().ToList();
            if (records.Count() > Constants.MaxImportAccounts)
                return Result.Failure(DomainError.Account.MaxImportAccountsExceeded);

            // Get all emails and usernames from the CSV
            var csvEmails = records.Select(r => r.Email).ToList();
            var csvUsernames = records.Select(r => r.Username).ToList();

            // Check for duplicates within the CSV file itself
            var duplicateEmails = csvEmails.GroupBy(e => e)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateEmails.Any())
                return Result.Failure($"{DomainError.Account.DuplicateEmailCsv} {string.Join(", ", duplicateEmails)}");

            var duplicateUsernames = csvUsernames.GroupBy(u => u)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateUsernames.Any())
                return Result.Failure($"{DomainError.Account.DuplicateUsernameCsv} {string.Join(", ", duplicateUsernames)}");

            // Check against existing database records
            var isExistingAccounts = await _unitOfWork.Accounts
                .FindAll()
                .AnyAsync(a => csvEmails.Contains(a.Email) || csvUsernames.Contains(a.Username), cancellationToken);
            // .Select(a => new { a.Email, a.Username })
            // .ToListAsync(
            //     cancellationToken); // With 75000 records in the database, this query takes 10-30 seconds to execute

            if (isExistingAccounts)
            {
                return Result.Failure(DomainError.Account.CsvAccountExist);
            }

            // Map CSV records to Account entities

            // var accounts = records.Select(r =>
            // {
            //     var account = _mapper.Map<Account>(r);
            //     account.PasswordHash = _passwordHasher.HashPassword(r.Password); // Moi lan hash ton 0.3s -> 1000 records -> 300s?
            //     return account;
            // }).ToList();

            var accounts = await HashPasswordsInParallelAsync(records, cancellationToken);
            _unitOfWork.Accounts.AddRange(accounts);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to import accounts: {ex.Message}");
        }
    }

    private async Task<List<Account>> HashPasswordsInParallelAsync(List<CsvAccount> records,
        CancellationToken cancellationToken)
    {
        // If you have 4 CPU cores, this will allow 8 concurrent operations
        var maxConcurrent = Environment.ProcessorCount * 2;
        // var maxConcurrent = Environment.ProcessorCount / 2;
        using var semaphore = new SemaphoreSlim(maxConcurrent);

        // Moi record la 1 task -> 1000 records -> 1000 tasks neu co 4 core -> 8 task chay cung 1 luc
        var hashingTasks = records.Select(async record =>
        {
            try
            {
                await semaphore.WaitAsync(cancellationToken);
                return await Task.Run(() =>
                {
                    var account = _mapper.Map<Account>(record);
                    account.PasswordHash = _passwordHasher.HashPassword(record.Password);
                    return account;
                }, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        });

        var hashedAccounts = await Task.WhenAll(hashingTasks);
        return hashedAccounts.ToList();
    }
}