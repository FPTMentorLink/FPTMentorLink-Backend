﻿using Microsoft.AspNetCore.Http;
using Services.Models.Request.Student;
using Services.Models.Response.Appointment;
using Services.Models.Response.Project;
using Services.Models.Response.Student;
using Services.Models.VnPay;
using Services.Utils;

namespace Services.Interfaces;

public interface IStudentService
{
    Task<Result<StudentDepositResponse>> DepositAsync(Guid id, StudentDepositRequest request, HttpContext httpContext);
    Task<VnPayIpnResponse> HandleVnPayIpn(IQueryCollection request);
    Task<Result<PaginationResult<ProjectResponse>>> GetMyProjectPagedAsync(GetStudentProjectsRequest request);
    Task<Result<PaginationResult<AppointmentResponse>>> GetMyAppointmentPagedAsync(GetStudentAppointmentsRequest request);
}