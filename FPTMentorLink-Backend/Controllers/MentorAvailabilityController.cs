using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.MentorAvailability;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/mentor-availability")]
public class MentorAvailabilityController : ControllerBase
{
    private readonly IMentorAvailabilityService _mentorAvailabilityService;

    public MentorAvailabilityController(IMentorAvailabilityService mentorAvailabilityService)
    {
        _mentorAvailabilityService = mentorAvailabilityService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _mentorAvailabilityService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetMentorAvailabilitiesRequest request){
        var result = await _mentorAvailabilityService.GetPagedAsync(request);
        return result.IsSuccess? Ok(result.Value) : BadRequest(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMentorAvailabilityRequest request){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mentorAvailabilityService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMentorAvailabilityRequest request){
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mentorAvailabilityService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id){
        var result = await _mentorAvailabilityService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}