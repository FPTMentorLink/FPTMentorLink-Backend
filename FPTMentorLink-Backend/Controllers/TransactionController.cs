using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Mentor;
using Services.Models.Request.Transaction;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/transactions")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService  _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPaged([FromQuery] GetTransactionsRequest request)
    {
        var result = await _transactionService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
    
    [HttpGet("my-transactions")]
    [Authorize(Roles = "Mentor,Student")]
    public async Task<IActionResult> GetMyTransactions([FromQuery] GetMyTransactionsRequest request)
    {
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }
        request.AccountId = userId!.Value;
        var result = await _transactionService.GetMyTransactionsAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
    
}