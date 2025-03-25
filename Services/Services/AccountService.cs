using System.Globalization;
using System.Linq.Expressions;
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
using Services.Models.Request.Lecturer;
using Services.Models.Request.Mentor;
using Services.Models.Request.Student;
using Services.Models.Response.Account;
using Services.Models.Response.Lecturer;
using Services.Models.Response.Mentor;
using Services.Models.Response.Student;
using Services.Settings;
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

    public async Task<Result<object>> GetByIdAsync(Guid id, AccountRole accountRole,
        CancellationToken cancellationToken) =>
        accountRole switch
        {
            AccountRole.Admin => await _unitOfWork.Accounts
                    .FindSingleAsync(a => a.Id == id && a.Role == AccountRole.Admin, cancellationToken)
                is { } account
                ? Result.Success<object>(_mapper.Map<AdminResponse>(account))
                : Result.Failure<object>(DomainError.Account.AccountNotFound),

            AccountRole.Lecturer => await _unitOfWork.Lecturers
                    .FindSingleAsync(l => l.Id == id, cancellationToken, x => x.Account, x => x.Faculty)
                is { } lecturer
                ? Result.Success<object>(_mapper.Map<LecturerResponse>(lecturer))
                : Result.Failure<object>(DomainError.Lecturer.LecturerNotFound),

            AccountRole.Mentor => await _unitOfWork.Mentors
                    .FindSingleAsync(m => m.Id == id, cancellationToken, x => x.Account)
                is { } mentor
                ? Result.Success<object>(_mapper.Map<MentorResponse>(mentor))
                : Result.Failure<object>(DomainError.Mentor.MentorNotFound),

            AccountRole.Student => await _unitOfWork.Students
                    .FindSingleAsync(s => s.Id == id, cancellationToken, x => x.Account, x => x.Faculty)
                is { } student
                ? Result.Success<object>(_mapper.Map<StudentResponse>(student))
                : Result.Failure<object>(DomainError.Student.StudentNotFound),

            _ => Result.Failure<object>(DomainError.Account.AccountNotFound)
        };

    /// <summary>
    /// Base validation for creating an account (default role is Admin)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<Result> BaseValidateAccountAsync(BaseCreateAccountRequest request,
        CancellationToken cancellationToken)
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

        return Result.Success();
    }

    public async Task<Result> CreateAdminAsync(CreateAdminRequest request, CancellationToken cancellationToken)
    {
        var validateAccount = await BaseValidateAccountAsync(request, cancellationToken);
        if (!validateAccount.IsSuccess)
            return validateAccount;
        request.Password = _passwordHasher.HashPassword(request.Password);
        var account = _mapper.Map<Account>(request);
        account.Role = AccountRole.Admin;
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

    public async Task<Result> CreateLecturerAsync(CreateLecturerRequest request, CancellationToken cancellationToken)
    {
        var baseValidation = await BaseValidateAccountAsync(request, cancellationToken);
        if (!baseValidation.IsSuccess)
            return baseValidation;
        var validateCode = await _unitOfWork.Lecturers.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (validateCode)
            return Result.Failure(DomainError.Lecturer.LecturerCodeExists);
        var faculty =
            await _unitOfWork.Faculties.AnyAsync(x => x.Id == request.FacultyId, cancellationToken);
        if (!faculty)
            return Result.Failure(DomainError.Faculty.FacultyNotFound);

        request.Password = _passwordHasher.HashPassword(request.Password);
        var account = _mapper.Map<Account>(request);
        account.Role = AccountRole.Lecturer;
        var lecturer = _mapper.Map<Lecturer>(request);
        lecturer.Id = account.Id;
        lecturer.Account = account; // Navigation property
        _unitOfWork.Accounts.Add(account);
        _unitOfWork.Lecturers.Add(lecturer);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> CreateMentorAsync(CreateMentorRequest request, CancellationToken cancellationToken)
    {
        var baseValidation = await BaseValidateAccountAsync(request, cancellationToken);
        if (!baseValidation.IsSuccess)
            return baseValidation;
        var validateCode = await _unitOfWork.Mentors.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (validateCode)
            return Result.Failure(DomainError.Lecturer.LecturerCodeExists);

        request.Password = _passwordHasher.HashPassword(request.Password);
        var account = _mapper.Map<Account>(request);
        account.Role = AccountRole.Mentor;
        var mentor = _mapper.Map<Mentor>(request);
        mentor.Id = account.Id;
        mentor.Account = account; // Navigation property
        _unitOfWork.Accounts.Add(account);
        _unitOfWork.Mentors.Add(mentor);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> CreateStudentAsync(CreateStudentRequest request, CancellationToken cancellationToken)
    {
        var baseValidation = await BaseValidateAccountAsync(request, cancellationToken);
        if (!baseValidation.IsSuccess)
            return baseValidation;
        var validateCode = await _unitOfWork.Students.AnyAsync(x => x.Code == request.Code, cancellationToken);
        if (validateCode)
            return Result.Failure(DomainError.Lecturer.LecturerCodeExists);

        request.Password = _passwordHasher.HashPassword(request.Password);
        var account = _mapper.Map<Account>(request);
        account.Role = AccountRole.Student;
        var student = _mapper.Map<Student>(request);
        student.Id = account.Id;
        student.Account = account; // Navigation property
        _unitOfWork.Accounts.Add(account);
        _unitOfWork.Students.Add(student);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> UpdateAdminAsync(Guid id, UpdateAdminRequest request,
        CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.FindByIdAsync(id, cancellationToken);

        if (account == null)
            return Result.Failure(DomainError.Account.AccountNotFound);

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

    public async Task<Result<PaginationResult<AccountResponse>>> GetPagedAsync(GetAccountsRequest request)
    {
        var query = _unitOfWork.Accounts.FindAll();
        Expression<Func<Account, bool>> condition = account => true;
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            Expression<Func<Account, bool>> searchTermFilter = account =>
                account.Email.Contains(request.SearchTerm) ||
                account.Username.Contains(request.SearchTerm) ||
                account.FirstName.Contains(request.SearchTerm);
            condition = condition.CombineAndAlsoExpressions(searchTermFilter);
        }

        if (request.Roles != null)
        {
            Expression<Func<Account, bool>> roleFilter = account =>
                request.Roles.Contains(account.Role);
            condition = condition.CombineAndAlsoExpressions(roleFilter);
        }

        query = query.Where(condition);

        query = ApplySorting(query, request);

        var result = await query.ProjectToPaginatedListAsync<Account, AccountResponse>(request);

        return Result.Success(result);
    }

    private static IQueryable<Account> ApplySorting(IQueryable<Account> query,
        PaginationParams paginationParams)
    {
        var orderBy = paginationParams.OrderBy;
        var isDescending = paginationParams.IsDescending;
        return orderBy.ToLower().Replace(" ", "") switch
        {
            _ => query.ApplySorting(isDescending, Account.GetSortValue(orderBy))
        };
    }

    public async Task<Result<int>> GetTotalAsync(AccountRole[] roles)
    {
        var query = _unitOfWork.Accounts.FindAll();
        if (roles.Length > 0) query = query.Where(a => roles.Contains(a.Role));

        var total = await query.CountAsync();
        return Result.Success(total);
    }

    public async Task<Result> ImportAccountsAsync(IFormFile file, AccountRole role, CancellationToken cancellationToken)
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
            switch (role)
            {
                case AccountRole.Admin:
                    var importAdminResult = await ImportAdminsAsync(csv, cancellationToken);
                    if (importAdminResult.IsFailure) return importAdminResult;
                    break;
                case AccountRole.Student:
                    var importStudentResult = await ImportStudentAsync(csv, cancellationToken);
                    if (importStudentResult.IsFailure) return importStudentResult;
                    break;
                case AccountRole.Mentor:
                    var importMentorResult = await ImportMentorAsync(csv, cancellationToken);
                    if (importMentorResult.IsFailure) return importMentorResult;
                    break;
                case AccountRole.Lecturer:
                    var importLecturerResult = await ImportLecturerAsync(csv, cancellationToken);
                    if (importLecturerResult.IsFailure) return importLecturerResult;
                    break;
            }

            // var records = csv.GetRecords<CsvAccount>().ToList();
            // if (records.Count() > Constants.MaxImportAccounts)
            //     return Result.Failure(DomainError.Account.MaxImportAccountsExceeded);
            //
            // // Get all emails and usernames from the CSV
            // var csvEmails = records.Select(r => r.Email).ToList();
            // var csvUsernames = records.Select(r => r.Username).ToList();
            //
            // // Check for duplicates within the CSV file itself
            // var duplicateEmails = csvEmails.GroupBy(e => e)
            //     .Where(g => g.Count() > 1)
            //     .Select(g => g.Key)
            //     .ToList();
            //
            // if (duplicateEmails.Any())
            //     return Result.Failure($"{DomainError.Account.DuplicateEmailCsv} {string.Join(", ", duplicateEmails)}");
            //
            // var duplicateUsernames = csvUsernames.GroupBy(u => u)
            //     .Where(g => g.Count() > 1)
            //     .Select(g => g.Key)
            //     .ToList();
            //
            // if (duplicateUsernames.Any())
            //     return Result.Failure(
            //         $"{DomainError.Account.DuplicateUsernameCsv} {string.Join(", ", duplicateUsernames)}");
            //
            // // Check against existing database records
            // var isExistingAccounts = await _unitOfWork.Accounts
            //     .FindAll()
            //     .AnyAsync(a => csvEmails.Contains(a.Email) || csvUsernames.Contains(a.Username), cancellationToken);
            // // .Select(a => new { a.Email, a.Username })
            // // .ToListAsync(
            // //     cancellationToken); // With 75000 records in the database, this query takes 10-30 seconds to execute
            //
            // if (isExistingAccounts)
            // {
            //     return Result.Failure(DomainError.Account.CsvAccountExist);
            // }
            //
            // // Map CSV records to Account entities
            //
            // // var accounts = records.Select(r =>
            // // {
            // //     var account = _mapper.Map<Account>(r);
            // //     account.PasswordHash = _passwordHasher.HashPassword(r.Password); // Moi lan hash ton 0.3s -> 1000 records -> 300s?
            // //     return account;
            // // }).ToList();
            //
            // var accounts = await HashPasswordsInParallelAsync(records, cancellationToken);
            // _unitOfWork.Accounts.AddRange(accounts);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to import accounts: {ex.Message}");
        }
    }

    private async Task<Result> ImportAdminsAsync(CsvReader csv, CancellationToken cancellationToken)
    {
        var adminRecords = csv.GetRecords<CsvAccount>().ToList();
        var adminResult = await BaseValidateImportRecords(adminRecords, cancellationToken);
        if (adminResult.IsFailure)
        {
            return adminResult;
        }

        var adminAccounts =
            await HashPasswordsInParallelAsync(adminRecords, AccountRole.Admin, cancellationToken);
        _unitOfWork.Accounts.AddRange(adminAccounts);
        return Result.Success();
    }

    private async Task<Result> ImportStudentAsync(CsvReader csv, CancellationToken cancellationToken)
    {
        var studentRecords = csv.GetRecords<CsvStudent>().ToList();
        var studentResult = await BaseValidateImportRecords(studentRecords, cancellationToken);
        if (studentResult.IsFailure)
        {
            return studentResult;
        }

        var code = studentRecords.Select(x => x.Code).ToList();
        var duplicateCode = code.GroupBy(e => e)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateCode.Any())
            return Result.Failure(
                $"{DomainError.Student.DuplicateStudentCodeCsv} {string.Join(", ", duplicateCode)}");

        var existCode = await _unitOfWork.Students.AnyAsync(x => code.Contains(x.Code), cancellationToken);
        if (existCode)
            return Result.Failure(DomainError.Student.StudentCodeExists);

        // Get all unique faculty codes from the records
        var studentFacultyCodes = studentRecords.Select(r => r.FacultyCode).Distinct().ToList();

        // Fetch all required faculties in a single query
        var facultyDictionary = await _unitOfWork.Faculties
            .FindAll(f => studentFacultyCodes.Contains(f.Code))
            .ToDictionaryAsync(f => f.Code, f => f.Id, cancellationToken);

        // Validate all faculty codes exist
        var missingFacultyCodes = studentFacultyCodes
            .Except(facultyDictionary.Keys)
            .ToList();
        if (missingFacultyCodes.Any())
        {
            return Result.Failure(
                $"Faculties with codes not found: {string.Join(", ", missingFacultyCodes)}");
        }

        var studentAccounts =
            await HashPasswordsInParallelAsync(studentRecords, AccountRole.Student, cancellationToken);
        // Create corresponding student entities
        var students = studentRecords.Select((r, index) =>
        {
            var student = _mapper.Map<Student>(r);
            // Use the same ID as the account
            student.Id = studentAccounts[index].Id;
            student.Account = studentAccounts[index];
            student.FacultyId = facultyDictionary[r.FacultyCode];
            return student;
        }).ToList();
        // Add both accounts and students to the database
        _unitOfWork.Accounts.AddRange(studentAccounts);
        _unitOfWork.Students.AddRange(students);
        return Result.Success();
    }

    private async Task<Result> ImportLecturerAsync(CsvReader csv, CancellationToken cancellationToken)
    {
        var lecturerRecords = csv.GetRecords<CsvLecturer>().ToList();
        var lecturerResult = await BaseValidateImportRecords(lecturerRecords, cancellationToken);
        if (lecturerResult.IsFailure)
        {
            return lecturerResult;
        }

        var code = lecturerRecords.Select(x => x.Code).ToList();
        var duplicateCode = code.GroupBy(e => e)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateCode.Any())
            return Result.Failure(
                $"{DomainError.Mentor.DuplicateLecturerCodeCsv} {string.Join(", ", duplicateCode)}");

        var existCode = await _unitOfWork.Lecturers.AnyAsync(x => code.Contains(x.Code), cancellationToken);
        if (existCode)
            return Result.Failure(DomainError.Lecturer.LecturerCodeExists);

        // Get all unique faculty codes from the records
        var facultyCodes = lecturerRecords.Select(r => r.FacultyCode).Distinct().ToList();

        // Fetch all required faculties in a single query
        var dictionary = await _unitOfWork.Faculties
            .FindAll(f => facultyCodes.Contains(f.Code))
            .ToDictionaryAsync(f => f.Code, f => f.Id, cancellationToken);

        // Validate all faculty codes exist
        var missingFaculty = facultyCodes
            .Except(dictionary.Keys)
            .ToList();
        if (missingFaculty.Any())
        {
            return Result.Failure(
                $"Faculties with codes not found: {string.Join(", ", missingFaculty)}");
        }

        var lecturerAccounts =
            await HashPasswordsInParallelAsync(lecturerRecords, AccountRole.Lecturer, cancellationToken);
        var lecturers = lecturerRecords.Select((r, index) =>
        {
            var lecturer = _mapper.Map<Lecturer>(r);
            lecturer.Id = lecturerAccounts[index].Id;
            lecturer.Account = lecturerAccounts[index];
            lecturer.FacultyId = dictionary[r.FacultyCode];
            return lecturer;
        }).ToList();
        _unitOfWork.Accounts.AddRange(lecturerAccounts);
        _unitOfWork.Lecturers.AddRange(lecturers);
        return Result.Success();
    }

    private async Task<Result> ImportMentorAsync(CsvReader csv, CancellationToken cancellationToken)
    {
        var mentorRecords = csv.GetRecords<CsvMentor>().ToList();
        var mentorResult = await BaseValidateImportRecords(mentorRecords, cancellationToken);
        if (mentorResult.IsFailure)
        {
            return mentorResult;
        }

        var code = mentorRecords.Select(x => x.Code).ToList();
        var duplicateCode = code.GroupBy(e => e)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateCode.Any())
            return Result.Failure(
                $"{DomainError.Lecturer.DuplicateLecturerCodeCsv} {string.Join(", ", duplicateCode)}");

        var existCode = await _unitOfWork.Mentors.AnyAsync(x => code.Contains(x.Code), cancellationToken);
        if (existCode)
            return Result.Failure(DomainError.Mentor.MentorCodeExists);

        var mentorAccounts =
            await HashPasswordsInParallelAsync(mentorRecords, AccountRole.Mentor, cancellationToken);
        var mentors = mentorRecords.Select((r, index) =>
        {
            var mentor = _mapper.Map<Mentor>(r);
            mentor.Id = mentorAccounts[index].Id;
            mentor.Account = mentorAccounts[index];
            return mentor;
        }).ToList();
        _unitOfWork.Accounts.AddRange(mentorAccounts);
        _unitOfWork.Mentors.AddRange(mentors);
        return Result.Success();
    }


    private async Task<Result> BaseValidateImportRecords<T>(IEnumerable<T> records, CancellationToken cancellationToken)
        where T : CsvAccount
    {
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
            return Result.Failure(
                $"{DomainError.Account.DuplicateUsernameCsv} {string.Join(", ", duplicateUsernames)}");

        // Check against existing database records
        var isExistingAccounts = await _unitOfWork.Accounts
            .FindAll()
            .AnyAsync(a => csvEmails.Contains(a.Email) || csvUsernames.Contains(a.Username), cancellationToken);
        return isExistingAccounts ? Result.Failure(DomainError.Account.CsvAccountExist) : Result.Success();
    }

    private async Task<List<Account>> HashPasswordsInParallelAsync<T>(List<T> records,
        AccountRole role, CancellationToken cancellationToken) where T : CsvAccount
    {
        // If you have 4 CPU cores, this will allow 8 concurrent operations
        // var maxConcurrent = Environment.ProcessorCount * 2;
        var maxConcurrent = Environment.ProcessorCount / 2;
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
                    account.Role = role;
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


    public async Task<Result> UpdateLecturerAsync(Guid id, UpdateLecturerRequest request,
        CancellationToken cancellationToken)
    {
        var lecturer = await _unitOfWork.Lecturers.FindSingleAsync(
            l => l.Id == id,
            cancellationToken, x => x.Account);
        if (lecturer == null)
            return Result.Failure(DomainError.Lecturer.LecturerNotFound);
        var existingCode =
            await _unitOfWork.Lecturers.AnyAsync(x => x.Code == request.Code && x.Id != id, cancellationToken);
        if (existingCode)
            return Result.Failure(DomainError.Lecturer.LecturerCodeExists);

        if (request.FacultyId.HasValue)
        {
            var faculty = await _unitOfWork.Faculties.AnyAsync(x => x.Id == request.FacultyId.Value, cancellationToken);
            if (!faculty)
                return Result.Failure(DomainError.Faculty.FacultyNotFound);
        }

        // Update base account properties only
        lecturer.Account.FirstName = request.FirstName ?? lecturer.Account.FirstName;
        lecturer.Account.LastName = request.LastName ?? lecturer.Account.LastName;
        lecturer.Account.ImageUrl = request.ImageUrl ?? lecturer.Account.ImageUrl;
        lecturer.Account.IsSuspended = request.IsSuspended ?? lecturer.Account.IsSuspended;

        // Update lecturer specific properties
        lecturer.FacultyId = request.FacultyId ?? lecturer.FacultyId;
        lecturer.Code = request.Code ?? lecturer.Code;
        lecturer.Description = request.Description ?? lecturer.Description;

        _unitOfWork.Lecturers.Update(lecturer);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateMentorAsync(Guid id, UpdateMentorRequest request,
        CancellationToken cancellationToken)
    {
        var mentor = await _unitOfWork.Mentors.FindSingleAsync(
            m => m.Id == id,
            cancellationToken, x => x.Account);
        if (mentor == null)
            return Result.Failure(DomainError.Mentor.MentorNotFound);
        var existingCode =
            await _unitOfWork.Mentors.AnyAsync(x => x.Code == request.Code && x.Id != id, cancellationToken);
        if (existingCode)
            return Result.Failure(DomainError.Mentor.MentorCodeExists);


        // Update base account properties only
        mentor.Account.FirstName = request.FirstName ?? mentor.Account.FirstName;
        mentor.Account.LastName = request.LastName ?? mentor.Account.LastName;
        mentor.Account.ImageUrl = request.ImageUrl ?? mentor.Account.ImageUrl;
        mentor.Account.IsSuspended = request.IsSuspended ?? mentor.Account.IsSuspended;

        // Update mentor specific properties
        mentor.Code = request.Code ?? mentor.Code;
        mentor.BankName = request.BankName ?? mentor.BankName;
        mentor.BankCode = request.BankCode ?? mentor.BankCode;
        mentor.BaseSalaryPerHour = request.BaseSalaryPerHour ?? mentor.BaseSalaryPerHour;

        _unitOfWork.Mentors.Update(mentor);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> UpdateStudentAsync(Guid id, UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Students.FindSingleAsync(
            s => s.Id == id,
            cancellationToken, x => x.Account);
        if (student == null)
            return Result.Failure(DomainError.Student.StudentNotFound);
        var existingCode =
            await _unitOfWork.Students.AnyAsync(x => x.Code == request.Code && x.Id != id, cancellationToken);
        if (existingCode)
            return Result.Failure(DomainError.Student.StudentCodeExists);
        if (request.FacultyId.HasValue)
        {
            var faculty = await _unitOfWork.Faculties.FindByIdAsync(request.FacultyId.Value, cancellationToken);
            if (faculty == null)
                return Result.Failure(DomainError.Faculty.FacultyNotFound);
        }

        // Update base account properties only
        student.Account.FirstName = request.FirstName ?? student.Account.FirstName;
        student.Account.LastName = request.LastName ?? student.Account.LastName;
        student.Account.ImageUrl = request.ImageUrl ?? student.Account.ImageUrl;
        student.Account.IsSuspended = request.IsSuspended ?? student.Account.IsSuspended;

        // Update student specific properties
        student.Code = request.Code ?? student.Code;

        student.FacultyId = request.FacultyId ?? student.FacultyId;

        _unitOfWork.Students.Update(student);
        await _unitOfWork.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result<object>> GetProfileAsync(Guid userId, string role, CancellationToken cancellationToken)
        =>
            role switch
            {
                "Admin" => await _unitOfWork.Accounts
                        .FindSingleAsync(a => a.Id == userId && a.Role == AccountRole.Admin, cancellationToken)
                    is { } account
                    ? Result.Success<object>(_mapper.Map<AdminResponse>(account))
                    : Result.Failure<object>(DomainError.Account.AccountNotFound),

                "Lecturer" => await _unitOfWork.Lecturers
                        .FindSingleAsync(l => l.Id == userId, cancellationToken, x => x.Account, x => x.Faculty)
                    is { } lecturer
                    ? Result.Success<object>(_mapper.Map<LecturerResponse>(lecturer))
                    : Result.Failure<object>(DomainError.Lecturer.LecturerNotFound),

                "Mentor" => await _unitOfWork.Mentors
                        .FindSingleAsync(m => m.Id == userId, cancellationToken, x => x.Account)
                    is { } mentor
                    ? Result.Success<object>(_mapper.Map<MentorResponse>(mentor))
                    : Result.Failure<object>(DomainError.Mentor.MentorNotFound),

                "Student" => await _unitOfWork.Students
                        .FindSingleAsync(s => s.Id == userId, cancellationToken, x => x.Account, x => x.Faculty)
                    is { } student
                    ? Result.Success<object>(_mapper.Map<StudentResponse>(student))
                    : Result.Failure<object>(DomainError.Student.StudentNotFound),

                _ => Result.Failure<object>(DomainError.Account.AccountNotFound)
            };
    
    public async Task<Result<object>> GetPublicProfileAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts
            .FindAll()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (account == null)
            return Result.Failure<object>("Account not found");

        if (account.Role != AccountRole.Mentor && account.Role != AccountRole.Lecturer)
            return Result.Failure<object>("Profile not available");

        // Return mentor profile
        if (account.Role == AccountRole.Mentor)
        {
            var mentor = await _unitOfWork.Mentors
                .FindAll()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Account.Id == id, cancellationToken);

            if (mentor == null)
                return Result.Failure<object>("Mentor profile not found");

            var mentorProfile = _mapper.Map<MentorResponse>(mentor);
            return Result.Success<object>(mentorProfile);
        }

        // Return lecturer profile
        if (account.Role == AccountRole.Lecturer)
        {
            var lecturer = await _unitOfWork.Lecturers
                .FindAll()
                .Include(x => x.Account)
                .FirstOrDefaultAsync(x => x.Account.Id == id, cancellationToken);

            if (lecturer == null)
                return Result.Failure<object>("Lecturer profile not found");

            var lecturerProfile = _mapper.Map<LecturerResponse>(lecturer);
            return Result.Success<object>(lecturerProfile);
        }

        return Result.Failure<object>("Invalid account type");
    }
}