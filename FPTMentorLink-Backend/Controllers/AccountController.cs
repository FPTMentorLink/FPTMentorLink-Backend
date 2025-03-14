using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Services.Interfaces;
using Services.Models.Request.Account;
using Services.Models.Request.Lecturer;
using Services.Models.Request.Mentor;
using Services.Models.Request.Student;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("import-accounts/role/{role}")]
    public async Task<IActionResult> ImportAccounts(IFormFile formFile, AccountRole role,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.ImportAccountsAsync(formFile, role, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts([FromQuery] PaginationParams request)
    {
        var result = await _accountService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("{id:guid}/role/{accountRole}")]
    public async Task<IActionResult> GetAccountById(Guid id, AccountRole accountRole,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.GetByIdAsync(id, accountRole, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("profile")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null || userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure<Account>("User not found"));
        }

        var role = User.GetRole();
        if (role == null)
        {
            return BadRequest(Result.Failure<Account>("Role not found"));
        }

        var result = await _accountService.GetProfileAsync(userId.Value, role, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }


    /// <summary>
    /// Role 1: Admin
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateAdminAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    /// <summary>
    /// Role 2: Student
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("student")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateStudentAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    /// <summary>
    /// Role 3: Mentor
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("mentor")]
    public async Task<IActionResult> CreateMentor([FromBody] CreateMentorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateMentorAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    /// <summary>
    /// Role 4
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("lecturer")]
    public async Task<IActionResult> CreateLecturer([FromBody] CreateLecturerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateLecturerAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("admin/{id:Guid}")]
    public async Task<IActionResult> UpdateAdminAccount(Guid id, [FromBody] UpdateAdminRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.UpdateAdminAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _accountService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("lecturer/{id:guid}")]
    public async Task<IActionResult> UpdateLecturer(Guid id, [FromBody] UpdateLecturerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.UpdateLecturerAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("mentor/{id:guid}")]
    public async Task<IActionResult> UpdateMentor(Guid id, [FromBody] UpdateMentorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.UpdateMentorAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("student/{id:guid}")]
    public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.UpdateStudentAsync(id, request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}