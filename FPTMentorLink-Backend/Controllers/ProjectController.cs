using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Project;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectByIdAsync([FromRoute] Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectPagedAsync([FromQuery] GetProjectsRequest request)
    {
        var result = await _projectService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var leaderId = User.GetUserId();
        if (leaderId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }

        request.LeaderId = leaderId!.Value;
        var result = await _projectService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _projectService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatusAsync([FromRoute] Guid id,
        [FromBody] UpdateProjectStatusRequest request)
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

        var result = await _projectService.UpdateStatusAsync(id, role.Value, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _projectService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}