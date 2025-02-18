using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
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
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}