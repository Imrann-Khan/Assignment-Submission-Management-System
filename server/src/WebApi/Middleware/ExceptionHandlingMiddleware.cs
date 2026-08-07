using System.Net;
using System.Text.Json;
using Application.Common.Exceptions;

namespace WebApi.Middleware;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation Failed"),
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            ForbiddenAccessException => (HttpStatusCode.Forbidden, "Forbidden"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        if(statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred while processing {Path}", context.Request.Path);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = new Dictionary<string, object?>
        {
            ["status"] = (int)statusCode,
            ["title"] = title,
            ["detail"] = exception.Message
        };

        if(exception is ValidationException validationException)
        {
            payload["errors"] = validationException.Errors;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}