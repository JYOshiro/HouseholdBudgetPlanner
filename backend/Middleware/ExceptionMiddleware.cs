using System.Net;
using System.Text.Json;

namespace HouseholdBudgetApi.Middleware;

/// <summary>
/// Middleware for handling global exceptions and returning consistent error responses.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to catch and handle exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions and returns appropriate HTTP responses.
    /// </summary>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { message = string.Empty, statusCode = HttpStatusCode.InternalServerError };

        switch (exception)
        {
            case UnauthorizedAccessException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new { message = "Unauthorized access.", statusCode = HttpStatusCode.Unauthorized };
                break;

            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new { message = exception.Message, statusCode = HttpStatusCode.BadRequest };
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new { message = "Resource not found.", statusCode = HttpStatusCode.NotFound };
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new { message = "An internal server error occurred.", statusCode = HttpStatusCode.InternalServerError };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}
