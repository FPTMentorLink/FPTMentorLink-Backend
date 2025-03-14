using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _lecturingProposalService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }   

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetLecturingProposalsRequest request)
    {
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
    public async Task<IActionResult> UpdateProposal([FromRoute] Guid id, [FromBody] UpdateLecturingProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = Result.Failure("Invalid role for this operation");
        if (User.IsInRole("Student"))
        {
            var studentUpdateRequest = new StudentUpdateLecturingProposalRequest{
                StudentNote = request.Note,
                IsClosed = request.IsClosed ?? false
            };
            result = await _lecturingProposalService.StudentUpdateAsync(id, studentUpdateRequest);
        }
        else if (User.IsInRole("Lecturer"))
        {
            var lecturerUpdateRequest = new LecturerUpdateLecturingProposalRequest{
                LecturerNote = request.Note,
                IsAccepted = request.IsAccepted
            };
            result = await _lecturingProposalService.LecturerUpdateAsync(id, lecturerUpdateRequest);
        }
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _lecturingProposalService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}