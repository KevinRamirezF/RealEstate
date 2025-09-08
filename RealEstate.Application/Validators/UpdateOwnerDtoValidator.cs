using FluentValidation;
using RealEstate.Application.DTOs.Input;
using System.Text.RegularExpressions;

namespace RealEstate.Application.Validators;

public class UpdateOwnerDtoValidator : AbstractValidator<UpdateOwnerDto>
{
    public UpdateOwnerDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .MaximumLength(100).WithMessage("First Name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .MaximumLength(100).WithMessage("Last Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Please provide a valid email address.")
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone Number cannot exceed 20 characters.")
            .Must(BeValidPhoneNumber)
            .WithMessage("Phone Number format is invalid.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of Birth must be in the past.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-150)))
            .WithMessage("Date of Birth cannot be more than 150 years ago.")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address Line is required.")
            .MaximumLength(255).WithMessage("Address Line cannot exceed 255 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal Code is required.")
            .MaximumLength(20).WithMessage("Postal Code cannot exceed 20 characters.")
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Postal Code must be in format 12345 or 12345-6789.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.ExternalCode)
            .MaximumLength(50).WithMessage("External Code cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.ExternalCode));
    }

    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return true;

        var phoneRegex = new Regex(@"^(\+\d{1,3}[- ]?)?\(?\d{3}\)?[- ]?\d{3}[- ]?\d{4}$");
        return phoneRegex.IsMatch(phoneNumber);
    }
}