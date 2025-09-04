using FluentValidation;
using RealEstate.Application.DTOs.Input;
using System.Text.RegularExpressions;

namespace RealEstate.Application.Validators;

public class PatchOwnerDtoValidator : AbstractValidator<PatchOwnerDto>
{
    public PatchOwnerDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .When(x => x.FirstName != null)
            .WithMessage("First Name cannot be empty when provided.")
            .MaximumLength(100)
            .When(x => x.FirstName != null)
            .WithMessage("First Name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .When(x => x.LastName != null)
            .WithMessage("Last Name cannot be empty when provided.")
            .MaximumLength(100)
            .When(x => x.LastName != null)
            .WithMessage("Last Name cannot exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .When(x => x.Email != null)
            .WithMessage("Email cannot be empty when provided.")
            .EmailAddress()
            .When(x => x.Email != null)
            .WithMessage("Please provide a valid email address.")
            .MaximumLength(255)
            .When(x => x.Email != null)
            .WithMessage("Email cannot exceed 255 characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20)
            .When(x => x.PhoneNumber != null)
            .WithMessage("Phone Number cannot exceed 20 characters.")
            .Must(BeValidPhoneNumber)
            .When(x => x.PhoneNumber != null)
            .WithMessage("Phone Number format is invalid.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.Today))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of Birth must be in the past.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Today.AddYears(-150)))
            .When(x => x.DateOfBirth.HasValue)
            .WithMessage("Date of Birth cannot be more than 150 years ago.");

        RuleFor(x => x.AddressLine)
            .NotEmpty()
            .When(x => x.AddressLine != null)
            .WithMessage("Address Line cannot be empty when provided.")
            .MaximumLength(255)
            .When(x => x.AddressLine != null)
            .WithMessage("Address Line cannot exceed 255 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.City != null)
            .WithMessage("City cannot be empty when provided.")
            .MaximumLength(100)
            .When(x => x.City != null)
            .WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .When(x => x.State != null)
            .WithMessage("State cannot be empty when provided.")
            .MaximumLength(100)
            .When(x => x.State != null)
            .WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .When(x => x.PostalCode != null)
            .WithMessage("Postal Code cannot be empty when provided.")
            .MaximumLength(20)
            .When(x => x.PostalCode != null)
            .WithMessage("Postal Code cannot exceed 20 characters.")
            .Matches(@"^\d{5}(-\d{4})?$")
            .When(x => x.PostalCode != null)
            .WithMessage("Postal Code must be in format 12345 or 12345-6789.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .When(x => x.Country != null)
            .WithMessage("Country cannot be empty when provided.")
            .MaximumLength(100)
            .When(x => x.Country != null)
            .WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.ExternalCode)
            .MaximumLength(50)
            .When(x => x.ExternalCode != null)
            .WithMessage("External Code cannot exceed 50 characters.");
    }

    private static bool BeValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return true;

        var phoneRegex = new Regex(@"^(\+\d{1,3}[- ]?)?\(?\d{3}\)?[- ]?\d{3}[- ]?\d{4}$");
        return phoneRegex.IsMatch(phoneNumber);
    }
}