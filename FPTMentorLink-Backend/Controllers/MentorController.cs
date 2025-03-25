using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Mentor;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/mentors")]
public class MentorController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMentors([FromQuery] GetMentorsRequest request)
    {
        var result = await _mentorService.GetMentorsPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
    
}