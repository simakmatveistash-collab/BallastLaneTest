using System.Net;
using System.Text.Json;

namespace BallastLaneTest.Server.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception has occurred: {Message}", exception.Message);
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            StatusCode = context.Response.StatusCode
        };

        return exception switch
        {
            InvalidOperationException => HandleInvalidOperationException(context, (InvalidOperationException)exception, response),
            ArgumentException => HandleArgumentException(context, (ArgumentException)exception, response),
            _ => HandleGenericException(context, exception, response)
        };
    }

    private static Task HandleInvalidOperationException(HttpContext context, InvalidOperationException exception, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.StatusCode = StatusCodes.Status400BadRequest;
        response.Message = exception.Message;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleArgumentException(HttpContext context, ArgumentException exception, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        response.StatusCode = StatusCodes.Status400BadRequest;
        response.Message = exception.Message;
        return context.Response.WriteAsJsonAsync(response);
    }

    private static Task HandleGenericException(HttpContext context, Exception exception, ErrorResponse response)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        response.StatusCode = StatusCodes.Status500InternalServerError;
        response.Message = "An internal server error has occurred. Please try again later.";
        return context.Response.WriteAsJsonAsync(response);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}
