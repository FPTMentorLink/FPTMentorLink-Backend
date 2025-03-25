using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _projectService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetProjectsRequest request)
    {
        var result = await _projectService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
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
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _projectService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin, Lecturer")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id,
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
    [Authorize(Roles = "Admin,Lecturer,Student")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _projectService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
    
    
    [HttpGet("my-projects")]
    [Authorize(Roles = "Student, Mentor, Lecturer")]
    public async Task<IActionResult> GetMyProjects([FromQuery] GetMyProjectsRequest request)
    {
        var userId = User.GetUserId();
        var role = User.GetRole();
        if (role == null)
        {
            return BadRequest(Result.Failure("Role not found"));
        }
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }
        request.AccountId = userId!.Value;
        var result = await _projectService.GetMyProjectPagedAsync(request, role!);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}