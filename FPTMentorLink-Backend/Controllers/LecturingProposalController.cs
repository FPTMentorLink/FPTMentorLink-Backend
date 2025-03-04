using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.LecturingProposal;

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

    [HttpPatch("{id}/student")]
    public async Task<IActionResult> StudentUpdate([FromRoute] Guid id, [FromBody] StudentUpdateLecturingProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _lecturingProposalService.StudentUpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}/lecturer")]
    public async Task<IActionResult> LecturerUpdate([FromRoute] Guid id, [FromBody] LecturerUpdateLecturingProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _lecturingProposalService.LecturerUpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _lecturingProposalService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}