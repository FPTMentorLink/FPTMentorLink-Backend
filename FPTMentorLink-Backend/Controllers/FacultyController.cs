using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Faculty;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/faculties")]
public class FacultyController : ControllerBase
{
    private readonly IFacultyService _facultyService;

    public FacultyController(IFacultyService facultyService)
    {
        _facultyService = facultyService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _facultyService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetFacultiesRequest request)
    {
        var result = await _facultyService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateFacultyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _facultyService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateFacultyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _facultyService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var result = await _facultyService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}