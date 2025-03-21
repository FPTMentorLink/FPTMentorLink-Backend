﻿using Microsoft.AspNetCore.Mvc;
using Repositories.Entities;
using Services.Interfaces;
using Services.Models.Request.Student;
using Services.Utils;

namespace FPTMentorLink_Backend.Controllers;

[Route("api/students")]
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

    [HttpGet("my-projects")]
    public async Task<IActionResult> GetMyProjects([FromQuery] GetStudentProjectsRequest request)
    {
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }

        request.StudentId = userId!.Value;
        var result = await _studentService.GetMyProjectPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }

    [HttpGet("my-appointments")]
    public async Task<IActionResult> GetMyAppointments([FromQuery] GetStudentAppointmentsRequest request)
    {
        var userId = User.GetUserId();
        if (userId.IsNullOrGuidEmpty())
        {
            return BadRequest(Result.Failure("User not found"));
        }

        request.StudentId = userId!.Value;
        var result = await _studentService.GetMyAppointmentPagedAsync(request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result);
    }
}