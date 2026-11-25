
using System.Net;

namespace StaffDb.Middlewares;

public class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Purpose: Continue the HTTP request pipeline.
            await next(context);
        }
        catch (Exception exception)
        {
            // Purpose: Log the actual exception for debugging.
            logger.LogError(
                exception,
                "An unexpected error occurred.");

            // Purpose: Check whether the response has already started.
            if (context.Response.HasStarted)
            {
                throw;
            }

            // Purpose: Clear any existing response content.
            context.Response.Clear();

            // Purpose: Set HTTP status code to 500.
            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            // Purpose: Specify JSON response format.
            context.Response.ContentType = "application/json";

            // Purpose: Send a consistent error response.
            // SQL Equivalent: None (HTTP response handling).
            await context.Response.WriteAsJsonAsync(new
            {
                message = "An unexpected error occurred."
            });
        }
    }
}
