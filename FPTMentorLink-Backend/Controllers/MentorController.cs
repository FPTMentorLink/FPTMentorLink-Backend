using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Mentor;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/mentors")]
public class MentorController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMentors([FromQuery] GetMentorsRequest request)
    {
        var result = await _mentorService.GetMentorsPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("my-appointments")]
    [Authorize(Roles = "Mentor")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] GetMentorAppointmentsRequest request)
    {
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }

        request.MentorId = userId!.Value;
        var result = await _mentorService.GetMyAppointmentPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("my-transactions")]
    [Authorize(Roles = "Mentor")]
    public async Task<IActionResult> GetMyTransactions([FromQuery] GetMentorTransactionsRequest request)
    {
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }

        request.MentorId = userId!.Value;
        var result = await _mentorService.GetMyTransactionPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}