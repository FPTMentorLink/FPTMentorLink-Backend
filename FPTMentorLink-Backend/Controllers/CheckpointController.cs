using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Checkpoint;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/checkpoints")]
public class CheckpointController : ControllerBase
{
    private readonly ICheckpointService _checkpointService;

    public CheckpointController(ICheckpointService checkpointService)
    {
        _checkpointService = checkpointService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _checkpointService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedAsync([FromQuery] GetCheckpointsRequest paginationParams)
    {
        var result = await _checkpointService.GetPagedAsync(paginationParams);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckpointAsync([FromBody] CreateCheckpointRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointService.CreateCheckpointAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCheckpointRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointService.UpdateCheckpointAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _checkpointService.DeleteCheckpointAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}