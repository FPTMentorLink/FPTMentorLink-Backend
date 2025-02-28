using System.Diagnostics;

namespace FPTMentorLink_Backend.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    private static string GetClientIp(HttpContext context)
    {
        // Add more common proxy headers
        var headerOrder = new[]
        {
            "X-Real-IP",
            "X-Forwarded-For",
            "X-Client-IP"
        };

        foreach (var header in headerOrder)
        {
            var ip = context.Request.Headers[header].FirstOrDefault();
            // _logger.LogInformation("Header {Header}: {Ip}", header, ip);
            if (string.IsNullOrEmpty(ip)) continue;
            // Handle X-Forwarded-For chain
            if (header == "X-Forwarded-For" && ip.Contains(','))
            {
                ip = ip.Split(',')[0].Trim();
            }

            return ip;
        }

        return context.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "Unknown";
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = Stopwatch.GetTimestamp();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Request failed: {Path}", context.Request.Path);
            if (ex.InnerException != null)
            {
                _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
            }

            throw;
        }
        finally
        {
            // Only log if status code indicates an issue
            var shouldLog = context.Response.StatusCode >= 400;

            if (shouldLog)
            {
                var elapsedMs = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
                var user = context.User.Identity?.Name ?? "Anonymous";
                // Get current timestamp in Vietnam time
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                // format: [timestamp] [method] [path] [status] [elapsed]ms | [user] | [ip]
                var logMessage = $"[{timestamp}] {context.Request.Method} {context.Request.Path} " +
                                 $"{context.Response.StatusCode} {elapsedMs:F0}ms " +
                                 $"| {user} | {GetClientIp(context)}";

                if (context.Response.StatusCode >= 400)
                {
                    _logger.LogWarning(logMessage);
                }
                else
                {
                    _logger.LogInformation(logMessage);
                }
            }
        }
    }
}