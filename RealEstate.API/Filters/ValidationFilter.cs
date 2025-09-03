using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RealEstate.API.Filters;

/// <summary>
/// Action filter that handles model validation and converts validation errors to ProblemDetails responses.
/// This filter runs before the action method and checks if the model state is valid.
/// </summary>
public class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var validationProblemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = 400,
                Detail = "Please refer to the errors property for additional details.",
                Instance = context.HttpContext.Request.Path
            };

            // Add correlation ID for tracking
            validationProblemDetails.Extensions.Add("correlationId", context.HttpContext.TraceIdentifier);
            validationProblemDetails.Extensions.Add("timestamp", DateTimeOffset.UtcNow);

            context.Result = new BadRequestObjectResult(validationProblemDetails);
        }

        base.OnActionExecuting(context);
    }
}

/// <summary>
/// Exception filter specifically for handling FluentValidation exceptions.
/// Converts FluentValidation.ValidationException to standardized ValidationProblemDetails.
/// </summary>
public class FluentValidationExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is FluentValidation.ValidationException validationException)
        {
            var validationProblemDetails = new ValidationProblemDetails();

            foreach (var error in validationException.Errors)
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
            validationProblemDetails.Status = 400;
            validationProblemDetails.Detail = "Please refer to the errors property for additional details.";
            validationProblemDetails.Instance = context.HttpContext.Request.Path;

            // Add correlation ID for tracking
            validationProblemDetails.Extensions.Add("correlationId", context.HttpContext.TraceIdentifier);
            validationProblemDetails.Extensions.Add("timestamp", DateTimeOffset.UtcNow);

            context.Result = new BadRequestObjectResult(validationProblemDetails);
            context.ExceptionHandled = true;
        }

        base.OnException(context);
    }
}