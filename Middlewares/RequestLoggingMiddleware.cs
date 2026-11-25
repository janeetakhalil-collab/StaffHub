using System.Diagnostics;

namespace StaffDb.Middlewares;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Purpose: Start measuring request processing time.
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Purpose: Continue to the next middleware.
            await next(context);
        }
        finally
        {
            // Purpose: Stop measuring after request processing.
            stopwatch.Stop();

            // Purpose: Log HTTP method, URL, status code and duration.
            logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}