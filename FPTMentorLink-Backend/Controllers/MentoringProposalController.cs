using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Services.Interfaces;
using Services.Models.Request.MentoringProposal;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/mentoring-proposals")]
public class MentoringProposalController : ControllerBase
{
    private readonly IMentoringProposalService _mentoringProposalService;

    public MentoringProposalController(IMentoringProposalService mentoringProposalService)
    {
        _mentoringProposalService = mentoringProposalService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mentoringProposalService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] GetMentoringProposalsRequest request)
    {
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }

        var role = User.GetAccountRole();
        if (role == null)
        {
            return BadRequest(Result.Failure("Role not found"));
        }

        switch (role)
        {
            case AccountRole.Student:
                request.StudentId = userId!.Value;
                break;
            case AccountRole.Mentor:
                request.MentorId = userId!.Value;
                break;
            case AccountRole.Admin:
                break;
            default:
                return BadRequest(Result.Failure("Invalid role for this operation"));
        }

        var result = await _mentoringProposalService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMentoringProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mentoringProposalService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}/response")]
    [Authorize] // Add authorization attribute
    public async Task<IActionResult> UpdateProposal([FromRoute] Guid id,
        [FromBody] UpdateMentoringProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var role = User.GetAccountRole();
        if (role == null)
        {
            return BadRequest(Result.Failure("Role not found"));
        }

        Result result;
        switch (role)
        {
            case AccountRole.Student:
            {
                var studentUpdateRequest = new StudentUpdateMentoringProposalRequest
                {
                    StudentNote = request.Note,
                    IsClosed = request.IsClosed ?? false
                };
                result = await _mentoringProposalService.StudentUpdateAsync(id, studentUpdateRequest);
                break;
            }
            case AccountRole.Mentor:
            {
                var mentorUpdateRequest = new MentorUpdateMentoringProposalRequest
                {
                    MentorNote = request.Note,
                    IsAccepted = request.IsAccepted
                };
                result = await _mentoringProposalService.MentorUpdateAsync(id, mentorUpdateRequest);
                break;
            }
            default:
                return BadRequest(Result.Failure("Invalid role for this operation"));
        }

        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mentoringProposalService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}