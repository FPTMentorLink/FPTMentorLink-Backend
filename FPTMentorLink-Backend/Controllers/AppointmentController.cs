using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Appointment;
using Services.Models.Request.Student;
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
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] GetAppointmentsRequest request)
    {
        var result = await _appointmentService.GetPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Get leaderId from claim
        var leaderId = User.GetUserId();
        if (leaderId == null || leaderId == Guid.Empty)
        {
            return BadRequest("User not found");
        }

        request.LeaderId = leaderId.Value;

        var result = await _appointmentService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPost("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id,
        [FromBody] UpdateAppointmentStatusRequest request)
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

        var result = await _appointmentService.UpdateStatusAsync(id, role.Value, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var result = await _appointmentService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
    
    [HttpGet("my-appointments")]
    [Authorize(Roles = "Student,Mentor,Lecturer")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] GetMyAppointmentsRequest request)
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
        var result = await _appointmentService.GetMyAppointmentPagedAsync(request, role!);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}