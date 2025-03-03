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
    public async Task<IActionResult> GetFacultyByIdAsync([FromRoute] Guid id)
    {
        var result = await _facultyService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetFacultyPagedAsync([FromQuery] GetFacultiesRequest request)
    {
        var result = await _facultyService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFacultyAsync([FromBody] CreateFacultyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _facultyService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateFacultyRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _facultyService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var result = await _facultyService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}