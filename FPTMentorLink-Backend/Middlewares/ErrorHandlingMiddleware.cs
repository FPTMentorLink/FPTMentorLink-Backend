﻿using System.Net;
using System.Text.Json;

namespace FPTMentorLink_Backend.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;


    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception? exception = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new
        {
            context.Response.StatusCode,
            Message = "An internal server error occurred.",
            DetailedMessage = exception?.Message ?? "An internal server error occurred! Please try again later.",
            StackTrace = exception?.StackTrace,
            InnerException = exception?.InnerException?.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}