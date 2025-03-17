using Microsoft.AspNetCore.Http;
using Repositories.Entities;
using Services.Models.Request.Account;
using Services.Models.Request.Authorization;
using Services.Models.Request.Base;
using Services.Models.Request.Lecturer;
using Services.Models.Request.Mentor;
using Services.Models.Request.Student;
using Services.Models.Response.Account;
using Services.Models.Response.Authentication;
using Services.Models.Response.Authorization;
using Services.Utils;

namespace Services.Interfaces;

public interface IAccountService
{
    Task<Result<object>> GetByIdAsync(Guid id, AccountRole accountRole, CancellationToken cancellationToken);
    Task<Result> CreateAdminAsync(CreateAdminRequest request, CancellationToken cancellationToken);
    Task<Result> CreateLecturerAsync(CreateLecturerRequest request, CancellationToken cancellationToken);
    Task<Result> CreateMentorAsync(CreateMentorRequest request, CancellationToken cancellationToken);
    Task<Result> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateAdminAsync(Guid id, UpdateAdminRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<LoginResponse>> LoginAsync(string email, string password);
    Task<Result<RefreshTokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<Result> IsEmailUniqueAsync(string email);
    Task<Result<PaginationResult<AccountResponse>>> GetPagedAsync(GetAccountsRequest paginationParams);
    Task<Result<int>> GetTotalAsync(AccountRole[] roles);
    Task<Result> ImportAccountsAsync(IFormFile formFile, AccountRole role, CancellationToken cancellationToken);
    Task<Result> UpdateLecturerAsync(Guid id, UpdateLecturerRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateMentorAsync(Guid id, UpdateMentorRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateStudentAsync(Guid id, UpdateStudentRequest request, CancellationToken cancellationToken);
    Task<Result<object>> GetProfileAsync(Guid userId, string role, CancellationToken cancellationToken);
}