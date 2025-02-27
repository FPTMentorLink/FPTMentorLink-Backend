using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Account;
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


    [HttpGet]
    public async Task<IActionResult> GetAccounts([FromQuery] PaginationParams request)
    {
        var result = await _accountService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _accountService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.CreateAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:Guid}")]
    public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _accountService.UpdateAsync(id, request,cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteAccount(Guid id,CancellationToken cancellationToken)
    {
        var result = await _accountService.DeleteAsync(id,cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}