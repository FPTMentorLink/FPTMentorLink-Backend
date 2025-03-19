using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Checkpoint;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/checkpoints")]
public class CheckpointController : ControllerBase
{
    private readonly ICheckpointService _checkpointService;

    public CheckpointController(ICheckpointService checkpointService)
    {
        _checkpointService = checkpointService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _checkpointService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetCheckpointsRequest request)
    {
        var result = await _checkpointService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCheckpointRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointService.CreateCheckpointAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCheckpointRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointService.UpdateCheckpointAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _checkpointService.DeleteCheckpointAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}