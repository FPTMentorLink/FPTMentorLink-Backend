using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Request.Student;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/student")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost("{id:guid}/deposit")]
    public async Task<IActionResult> Deposit([FromRoute] Guid id, StudentDepositRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _studentService.DepositAsync(id, request, HttpContext);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("IPN")]
    public async Task<IActionResult> Ipn()
    {
        var query = Request.Query;
        var result = await _studentService.HandleVnPayIpn(query);
        return Ok(result);
    }
}