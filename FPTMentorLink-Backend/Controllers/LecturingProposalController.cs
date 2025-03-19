using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Services.Interfaces;
using Services.Models.Request.LecturingProposal;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/lecturing-proposals")]
public class LecturingProposalController : ControllerBase
{
    private readonly ILecturingProposalService _lecturingProposalService;

    public LecturingProposalController(ILecturingProposalService lecturingProposalService)
    {
        _lecturingProposalService = lecturingProposalService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _lecturingProposalService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] GetLecturingProposalsRequest request)
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
            case AccountRole.Lecturer:
                request.LecturerId = userId!.Value;
                break;
            case AccountRole.Admin:
                break;
            default:
                return BadRequest(Result.Failure("Invalid role for this operation"));
        }

        var result = await _lecturingProposalService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLecturingProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _lecturingProposalService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}/response")]
    [Authorize]
    public async Task<IActionResult> UpdateResponse([FromRoute] Guid id,
        [FromBody] UpdateLecturingProposalRequest request)
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
                var studentUpdateRequest = new StudentUpdateLecturingProposalRequest
                {
                    StudentNote = request.Note,
                    IsClosed = request.IsClosed ?? false
                };
                result = await _lecturingProposalService.StudentUpdateAsync(id, studentUpdateRequest);
                break;
            }
            case AccountRole.Lecturer:
            {
                var lecturerUpdateRequest = new LecturerUpdateLecturingProposalRequest
                {
                    LecturerNote = request.Note,
                    IsAccepted = request.IsAccepted
                };
                result = await _lecturingProposalService.LecturerUpdateAsync(id, lecturerUpdateRequest);
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
        var result = await _lecturingProposalService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}