using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.CheckpointTask;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/checkpoint-tasks")]
public class CheckpointTaskController : ControllerBase
{
    private readonly ICheckpointTaskService _checkpointTaskService;

    public CheckpointTaskController(ICheckpointTaskService checkpointTaskService)
    {
        _checkpointTaskService = checkpointTaskService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCheckpointTaskByIdAsync(Guid id)
    {
        var result = await _checkpointTaskService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCheckpointTaskPagedAsync([FromQuery] GetCheckpointTasksRequest request)
    {
        var result = await _checkpointTaskService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCheckpointTaskAsync([FromBody] CreateCheckpointTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointTaskService.CreateCheckpointTaskAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateCheckpointTaskAsync(Guid id, [FromBody] UpdateCheckpointTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointTaskService.UpdateCheckpointTaskAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCheckpointTaskAsync(Guid id)
    {
        var result = await _checkpointTaskService.DeleteCheckpointTaskAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}