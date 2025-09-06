using FluentValidation;
using RealEstate.Application.DTOs.Input;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Validators;

public class UpdatePropertyDtoValidator : AbstractValidator<UpdatePropertyDto>
{
    public UpdatePropertyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.YearBuilt)
            .GreaterThan((short)1800)
            .When(x => x.YearBuilt.HasValue)
            .WithMessage("Year Built must be greater than 1800.")
            .LessThanOrEqualTo((short)(DateTime.Now.Year + 5))
            .When(x => x.YearBuilt.HasValue)
            .WithMessage("Year Built cannot be more than 5 years in the future.");

        RuleFor(x => x.Bedrooms)
            .GreaterThanOrEqualTo((short)0)
            .WithMessage("Bedrooms must be 0 or greater.")
            .LessThanOrEqualTo((short)50)
            .WithMessage("Bedrooms cannot exceed 50.");

        RuleFor(x => x.Bathrooms)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Bathrooms must be 0 or greater.")
            .LessThanOrEqualTo(50)
            .WithMessage("Bathrooms cannot exceed 50.");

        RuleFor(x => x.ParkingSpaces)
            .GreaterThanOrEqualTo((short)0)
            .WithMessage("Parking Spaces must be 0 or greater.")
            .LessThanOrEqualTo((short)20)
            .WithMessage("Parking Spaces cannot exceed 20.");

        RuleFor(x => x.AreaSqft)
            .GreaterThan(0)
            .When(x => x.AreaSqft.HasValue)
            .WithMessage("Area must be greater than 0.")
            .LessThanOrEqualTo(1000000)
            .When(x => x.AreaSqft.HasValue)
            .WithMessage("Area cannot exceed 1,000,000 sqft.");



        RuleFor(x => x.AddressLine)
            .MaximumLength(200)
            .WithMessage("Address Line cannot exceed 200 characters.");

        RuleFor(x => x.City)
            .MaximumLength(120)
            .WithMessage("City cannot exceed 120 characters.");

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

        RuleFor(x => x.Lat)
            .InclusiveBetween(-90.0m, 90.0m)
            .When(x => x.Lat.HasValue)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Lng)
            .InclusiveBetween(-180.0m, 180.0m)
            .When(x => x.Lng.HasValue)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(x => x.ListingStatus)
            .Must(BeValidListingStatus)
            .When(x => !string.IsNullOrEmpty(x.ListingStatus))
            .WithMessage("Invalid Listing Status. Valid values: DRAFT, ACTIVE, PENDING, SOLD, OFF_MARKET");

        RuleFor(x => x.ListingDate)
            .GreaterThanOrEqualTo(new DateOnly(2000, 1, 1))
            .When(x => x.ListingDate.HasValue)
            .WithMessage("Listing Date cannot be before year 2000.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddYears(1)))
            .When(x => x.ListingDate.HasValue)
            .WithMessage("Listing Date cannot be more than 1 year in the future.");

        RuleFor(x => x.RowVersion)
            .NotNull()
            .WithMessage("Row Version is required for concurrency control.")
            .Must(rv => rv != null && rv.Length == 8)
            .WithMessage("Row Version must be an 8-byte array.");
    }

    private static bool BeValidListingStatus(string? listingStatus)
    {
        return Enum.TryParse<ListingStatus>(listingStatus, out _);
    }
}