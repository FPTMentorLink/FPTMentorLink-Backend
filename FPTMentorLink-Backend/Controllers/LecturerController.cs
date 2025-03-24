using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Lecturer;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/lecturers")]
public class LecturerController : ControllerBase
{
    private readonly ILecturerService _lecturerService;

    public LecturerController(ILecturerService lecturerService)
    {
        _lecturerService = lecturerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLecturers([FromQuery] GetLecturersRequest request)
    {
        var result = await _lecturerService.GetLecturersPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}