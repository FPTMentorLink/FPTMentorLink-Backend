using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.WeeklyReport;
using Services.Models.Request.WeeklyReportFeedback;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/weekly-reports")]
public class WeeklyReportController : ControllerBase
{
    private readonly IWeeklyReportService _weeklyReportService;

    public WeeklyReportController(IWeeklyReportService weeklyReportService)
    {
        _weeklyReportService = weeklyReportService;
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _weeklyReportService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaged([FromQuery] GetWeeklyReportRequest request)
    {
        var result = await _weeklyReportService.GetPagedAsync(request);
        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create([FromBody] CreateWeeklyReportRequest request)
    {
        var result = await _weeklyReportService.CreateAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWeeklyReportRequest request)
    {
        var result = await _weeklyReportService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _weeklyReportService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }

    [HttpPost("{id:guid}/feedback")]
    [Authorize(Roles = "Lecturer")]
    public async Task<IActionResult> CreateFeedback(Guid id, [FromBody] CreateWeeklyReportFeedback request)
    {
        var lecturerId = User.GetUserId();
        if (lecturerId == null)
        {
            return BadRequest("Invalid lecturer ID");
        }

        request.LecturerId = lecturerId.Value;
        var result = await _weeklyReportService.CreateFeedbackAsync(id, request);
        return result.IsSuccess ? Ok() : BadRequest(result);
    }
}