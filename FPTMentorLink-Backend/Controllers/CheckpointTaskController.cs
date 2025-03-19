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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _checkpointTaskService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetCheckpointTasksRequest request)
    {
        var result = await _checkpointTaskService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCheckpointTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointTaskService.CreateCheckpointTaskAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCheckpointTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointTaskService.UpdateCheckpointTaskAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id,
        [FromBody] UpdateCheckpointTaskStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _checkpointTaskService.UpdateCheckpointTaskStatusAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _checkpointTaskService.DeleteCheckpointTaskAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}