namespace InventoryService.Api.Middleware;

using System.Text.Json;
using FluentValidation;
using InventoryService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
        var problemDetails = exception switch
        {
            ValidationException validationEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Extensions =
                {
                    ["errors"] = validationEx.Errors
                        .Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
                        .ToArray()
                }
            },
            InventoryDomainException domainEx => new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Domain Error",
                Detail = domainEx.Message,
                Extensions = { ["code"] = domainEx.Code }
            },
            InvalidOperationException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = exception.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            }
        };

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred");
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception: {Message}", exception.Message);
        }

        context.Response.StatusCode = problemDetails.Status ?? 500;
        context.Response.ContentType = "application/problem+json";

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
