using System.Text.Json;

namespace TicketingSLA.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var payload = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
            await context.Response.WriteAsync(payload);
        }
    }
}
