using System.Net;
using System.Text.Json;
using Academia.Application.Common.Exceptions;

namespace Academia.WebAPI.Middleware;

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, response) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.UnprocessableEntity,
                new ProblemResponse("Validation failed", (int)HttpStatusCode.UnprocessableEntity, ve.Errors)),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                new ProblemResponse(nfe.Message, (int)HttpStatusCode.NotFound)),

            UnauthorizedException ue => (
                HttpStatusCode.Unauthorized,
                new ProblemResponse(ue.Message, (int)HttpStatusCode.Unauthorized)),

            _ => (
                HttpStatusCode.InternalServerError,
                new ProblemResponse("An unexpected error occurred.", (int)HttpStatusCode.InternalServerError))
        };

        if ((int)statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public record ProblemResponse(
    string Message,
    int StatusCode,
    object? Errors = null
);
