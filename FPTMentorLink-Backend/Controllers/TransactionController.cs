using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Transaction;

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
    
    
}