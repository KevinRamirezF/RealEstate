using FluentValidation;
using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Validators;

public class CreateOwnerDtoValidator : AbstractValidator<CreateOwnerDto>
{
    public CreateOwnerDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full Name is required.")
            .MaximumLength(180)
            .WithMessage("Full Name cannot exceed 180 characters.")
            .Matches(@"^[a-zA-Z\s\-'\.]+$")
            .WithMessage("Full Name can only contain letters, spaces, hyphens, apostrophes, and periods.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(180)
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email cannot exceed 180 characters.");

        RuleFor(x => x.Phone)
            .MaximumLength(40)
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone cannot exceed 40 characters.")
            .Matches(@"^[\+]?[1-9][\d\-\(\)\s]*$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone must be a valid phone number format.");

        RuleFor(x => x.ExternalCode)
            .MaximumLength(40)
            .When(x => !string.IsNullOrEmpty(x.ExternalCode))
            .WithMessage("External Code cannot exceed 40 characters.")
            .Matches(@"^[a-zA-Z0-9\-_]+$")
            .When(x => !string.IsNullOrEmpty(x.ExternalCode))
            .WithMessage("External Code can only contain letters, numbers, hyphens, and underscores.");

        RuleFor(x => x.PhotoUrl)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.PhotoUrl))
            .WithMessage("Photo URL cannot exceed 500 characters.")
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.PhotoUrl))
            .WithMessage("Photo URL must be a valid HTTP or HTTPS URL.");

        RuleFor(x => x.BirthDate)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
            .When(x => x.BirthDate.HasValue)
            .WithMessage("Birth Date indicates person must be at least 18 years old.")
            .GreaterThanOrEqualTo(new DateOnly(1900, 1, 1))
            .When(x => x.BirthDate.HasValue)
            .WithMessage("Birth Date cannot be before year 1900.");

        RuleFor(x => x.AddressLine1)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.AddressLine1))
            .WithMessage("Address Line 1 cannot exceed 200 characters.");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.AddressLine2))
            .WithMessage("Address Line 2 cannot exceed 200 characters.");

        RuleFor(x => x.City)
            .MaximumLength(120)
            .When(x => !string.IsNullOrEmpty(x.City))
            .WithMessage("City cannot exceed 120 characters.")
            .Matches(@"^[a-zA-Z\s\-'\.]+$")
            .When(x => !string.IsNullOrEmpty(x.City))
            .WithMessage("City can only contain letters, spaces, hyphens, apostrophes, and periods.");

        RuleFor(x => x.State)
            .Length(2)
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage("State must be exactly 2 characters.")
            .Matches("^[A-Z]{2}$")
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage("State must be uppercase 2-letter code (e.g., CA, NY).");

        RuleFor(x => x.PostalCode)
            .MaximumLength(10)
            .When(x => !string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Postal Code cannot exceed 10 characters.")
            .Matches(@"^\d{5}(-\d{4})?$")
            .When(x => !string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Postal Code must be in format 12345 or 12345-6789.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .Length(2)
            .WithMessage("Country must be exactly 2 characters.")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country must be uppercase 2-letter ISO code (e.g., US, CA).");

        // Business rule: Email or Phone must be provided for contact
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Either Email or Phone must be provided for contact information.")
            .WithName("ContactInfo");
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}