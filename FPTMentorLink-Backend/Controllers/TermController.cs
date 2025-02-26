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
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _termService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedAsync([FromQuery] GetTermsRequest paginationParams)
    {
        var result = await _termService.GetPagedAsync(paginationParams);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTermRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _termService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTermRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _termService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _termService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}