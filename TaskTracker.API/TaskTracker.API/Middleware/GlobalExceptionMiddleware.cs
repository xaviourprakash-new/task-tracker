using Microsoft.AspNetCore.Mvc;
using TaskTracker.API.Application.Common.Exceptions;

namespace TaskTracker.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        var problemDetails = exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Resource Not Found",
                Detail = notFound.Message
            },

            BadRequestException badRequest => CreateBadRequestProblemDetails(badRequest),

            ConflictException conflict => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = conflict.Message
            },

            UnauthorizedException unauthorized => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Authentication Failed",
                Detail = unauthorized.Message
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred. Please try again later."
            }
        };

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "An unhandled exception occurred.");

        problemDetails.Instance = context.Request.Path;

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static ValidationProblemDetails CreateBadRequestProblemDetails(BadRequestException exception)
    {
        var problemDetails = new ValidationProblemDetails(exception.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed",
            Detail = exception.Message
        };

        return problemDetails;
    }
}
