using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.ProjectStudent;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/project-students")]
public class ProjectStudentController : ControllerBase
{
    private readonly IProjectStudentService _projectStudentService;

    public ProjectStudentController(IProjectStudentService projectStudentService)
    {
        _projectStudentService = projectStudentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _projectStudentService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetProjectStudentsRequest request)
    {
        var result = await _projectStudentService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost("send-invitation")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SendProjectInvitation([FromBody] SendProjectInvitationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _projectStudentService.SendProjectInvitationAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpGet("accept-invitation")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> AcceptProjectInvitation([FromQuery] AcceptProjectInvitationRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _projectStudentService.AcceptProjectInvitationAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,Student")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _projectStudentService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}