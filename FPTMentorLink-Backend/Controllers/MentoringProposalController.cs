using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.MentoringProposal;

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
    public async Task<IActionResult> GetPaged([FromQuery] GetMentoringProposalsRequest request)
    {
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

    [HttpPatch("{id}/student")]
    public async Task<IActionResult> StudentUpdate([FromRoute] Guid id, [FromBody] StudentUpdateMentoringProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mentoringProposalService.StudentUpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}/mentor")]
    public async Task<IActionResult> MentorUpdate([FromRoute] Guid id, [FromBody] MentorUpdateMentoringProposalRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mentoringProposalService.MentorUpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _mentoringProposalService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}