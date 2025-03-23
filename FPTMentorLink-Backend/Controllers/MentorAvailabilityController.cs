using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Services.Interfaces;
using Services.Models.Request.MentorAvailability;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/mentor-availability")]
public class MentorAvailabilityController : ControllerBase
{
    private readonly IMentorAvailabilityService _mentorAvailabilityService;

    public MentorAvailabilityController(IMentorAvailabilityService mentorAvailabilityService)
    {
        _mentorAvailabilityService = mentorAvailabilityService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mentorAvailabilityService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetMentorAvailabilitiesRequest request)
    {
        var result = await _mentorAvailabilityService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMentorAvailabilityRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.GetUserId();
        if (userId == null)
        {
            return BadRequest(Result.Failure("User not found"));
        }

        var role = User.GetAccountRole();
        if (role is not AccountRole.Mentor)
        {
            return BadRequest(Result.Failure("Invalid role for this operation"));
        }

        request.MentorId = userId.Value;

        var result = await _mentorAvailabilityService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMentorAvailabilityRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = User.GetUserId();
        if (userId == null)
        {
            return BadRequest(Result.Failure("User not found"));
        }

        var role = User.GetAccountRole();
        if (role is not AccountRole.Mentor)
        {
            return BadRequest(Result.Failure("Invalid role for this operation"));
        }

        request.MentorId = userId.Value;  

        var result = await _mentorAvailabilityService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return BadRequest(Result.Failure("User not found"));
        }
        var role = User.GetAccountRole();
        if (role is not AccountRole.Mentor)
        {
            return BadRequest(Result.Failure("Invalid role for this operation"));
        }
        var result = await _mentorAvailabilityService.DeleteAsync(id, userId.Value);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}