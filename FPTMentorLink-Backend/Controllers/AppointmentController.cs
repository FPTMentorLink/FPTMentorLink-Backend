using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[ApiController]
[Route("api/appointment")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments([FromQuery] PaginationParams request)
    {
        var result = await _appointmentService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get leaderId from claim
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest("Invalid user ID");
        }

        request.LeaderId = userId!.Value;

        var result = await _appointmentService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] CancelAppointmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get userId from claim
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest("Invalid user ID");
        }

        request.UserId = userId!.Value;
        request.AppointmentId = id;

        var result = await _appointmentService.CancelAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}