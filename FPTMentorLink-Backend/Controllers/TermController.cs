using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Term;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/terms")]
public class TermController : ControllerBase
{
    private readonly ITermService _termService;

    public TermController(ITermService termService)
    {
        _termService = termService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTermByIdAsync(Guid id)
    {
        var result = await _termService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTermPagedAsync([FromQuery] GetTermsRequest request)
    {
        var result = await _termService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTermAsync([FromBody] CreateTermRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _termService.CreateTermAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTermAsync(Guid id, [FromBody] UpdateTermRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _termService.UpdateTermAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateTermStatusAsync(Guid id, [FromBody] UpdateTermStatusRequest status)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _termService.UpdateTermStatusAsync(id, status);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTermAsync(Guid id)
    {
        var result = await _termService.DeleteTermAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}