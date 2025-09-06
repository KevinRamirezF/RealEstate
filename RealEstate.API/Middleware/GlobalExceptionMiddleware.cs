using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace RealEstate.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. Request: {Method} {Path} {QueryString}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString);

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = exception switch
        {
            FluentValidation.ValidationException validationEx => CreateValidationProblemDetails(context, validationEx),
            System.ComponentModel.DataAnnotations.ValidationException validationEx => CreateBusinessValidationProblemDetails(context, validationEx),
            KeyNotFoundException notFoundEx => CreateNotFoundProblemDetails(context, notFoundEx),
            UnauthorizedAccessException unauthorizedEx => CreateUnauthorizedProblemDetails(context, unauthorizedEx),
            InvalidOperationException invalidOpEx => CreateBadRequestProblemDetails(context, invalidOpEx),
            ArgumentException argumentEx => CreateBadRequestProblemDetails(context, argumentEx),
            DbUpdateConcurrencyException concurrencyEx => CreateConcurrencyProblemDetails(context, concurrencyEx),
            DbUpdateException dbUpdateEx => CreateDatabaseProblemDetails(context, dbUpdateEx),
            TimeoutException timeoutEx => CreateTimeoutProblemDetails(context, timeoutEx),
            _ => CreateInternalServerErrorProblemDetails(context, exception)
        };

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, jsonOptions));
    }

    private static ProblemDetails CreateValidationProblemDetails(HttpContext context, FluentValidation.ValidationException exception)
    {
        var validationProblemDetails = new ValidationProblemDetails();
        
        foreach (var error in exception.Errors)
        {
            if (validationProblemDetails.Errors.ContainsKey(error.PropertyName))
            {
                var existingErrors = validationProblemDetails.Errors[error.PropertyName].ToList();
                existingErrors.Add(error.ErrorMessage);
                validationProblemDetails.Errors[error.PropertyName] = existingErrors.ToArray();
            }
            else
            {
                validationProblemDetails.Errors.Add(error.PropertyName, new[] { error.ErrorMessage });
            }
        }

        validationProblemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        validationProblemDetails.Title = "One or more validation errors occurred.";
        validationProblemDetails.Status = (int)HttpStatusCode.BadRequest;
        validationProblemDetails.Detail = "Please refer to the errors property for additional details.";
        validationProblemDetails.Instance = context.Request.Path;

        return validationProblemDetails;
    }

    private ProblemDetails CreateBusinessValidationProblemDetails(HttpContext context, System.ComponentModel.DataAnnotations.ValidationException exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Business rule validation failed",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message,
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateNotFoundProblemDetails(HttpContext context, KeyNotFoundException exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Resource not found",
            Status = (int)HttpStatusCode.NotFound,
            Detail = exception.Message,
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateUnauthorizedProblemDetails(HttpContext context, UnauthorizedAccessException exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = (int)HttpStatusCode.Unauthorized,
            Detail = "Access to this resource is not authorized.",
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateBadRequestProblemDetails(HttpContext context, Exception exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = exception.Message,
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateConcurrencyProblemDetails(HttpContext context, DbUpdateConcurrencyException exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
            Title = "Concurrency Conflict",
            Status = (int)HttpStatusCode.Conflict,
            Detail = "The resource has been modified by another request. Please refresh and try again.",
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateDatabaseProblemDetails(HttpContext context, DbUpdateException exception)
    {
        var detail = _environment.IsDevelopment() 
            ? exception.InnerException?.Message ?? exception.Message
            : "A database error occurred while processing your request.";

        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Database Error",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = detail,
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateTimeoutProblemDetails(HttpContext context, TimeoutException exception)
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.5",
            Title = "Request Timeout",
            Status = (int)HttpStatusCode.RequestTimeout,
            Detail = "The request took too long to process.",
            Instance = context.Request.Path
        };
    }

    private ProblemDetails CreateInternalServerErrorProblemDetails(HttpContext context, Exception exception)
    {
        var detail = _environment.IsDevelopment()
            ? $"{exception.Message}\n{exception.StackTrace}"
            : "An internal server error occurred. Please try again later.";

        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = detail,
            Instance = context.Request.Path
        };
    }
}