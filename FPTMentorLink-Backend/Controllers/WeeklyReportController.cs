using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.WeeklyReport;

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
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _weeklyReportService.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetWeeklyReportRequest request)
    {
        var result = await _weeklyReportService.GetPagedAsync(request);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWeeklyReportRequest request)
    {
        var result = await _weeklyReportService.CreateAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWeeklyReportRequest request)
    {
        var result = await _weeklyReportService.UpdateAsync(id, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _weeklyReportService.DeleteAsync(id);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}