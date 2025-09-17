using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContactManager.Web.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = MapExceptionToStatusCode(exception);

        _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        var problemDetails = CreateProblemDetails(context, exception, statusCode);

        context.Response.StatusCode = problemDetails.Status.Value;

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static HttpStatusCode MapExceptionToStatusCode(Exception exception) =>

        exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            ArgumentException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

    private static ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, HttpStatusCode statusCode) =>
        new()
        {
            Status = (int)statusCode,
            Title = statusCode.ToString(),
            Detail = exception.Message,
            Instance = context.Request.Path
        };
}