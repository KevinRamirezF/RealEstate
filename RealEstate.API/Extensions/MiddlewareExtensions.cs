using RealEstate.API.Middleware;

namespace RealEstate.API.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the application pipeline.
    /// This middleware catches all unhandled exceptions and converts them to standardized ProblemDetails responses.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    /// <summary>
    /// Configures comprehensive problem details support for the application.
    /// This includes custom problem details for various HTTP status codes and validation errors.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddProblemDetailsSupport(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            // Customize problem details for different status codes
            options.CustomizeProblemDetails = context =>
            {
                var problemDetails = context.ProblemDetails;

                // Add machine-readable type URI based on status code
                problemDetails.Type ??= context.HttpContext.Response.StatusCode switch
                {
                    400 => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    401 => "https://tools.ietf.org/html/rfc7235#section-3.1", 
                    403 => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    404 => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    409 => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    422 => "https://tools.ietf.org/html/rfc4918#section-11.2",
                    500 => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    _ => "https://tools.ietf.org/html/rfc7231"
                };

                // Add request path to problem details
                problemDetails.Instance ??= context.HttpContext.Request.Path;

                // Add correlation ID for tracking
                var correlationId = context.HttpContext.TraceIdentifier;
                problemDetails.Extensions.TryAdd("correlationId", correlationId);

                // Add timestamp for debugging
                problemDetails.Extensions.TryAdd("timestamp", DateTimeOffset.UtcNow);
            };
        });

        return services;
    }
}