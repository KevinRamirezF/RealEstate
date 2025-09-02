using FluentValidation;
using RealEstate.Application.DTOs.Input;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Validators;

public class CreatePropertyDtoValidator : AbstractValidator<CreatePropertyDto>
{
    public CreatePropertyDtoValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID is required.");

        RuleFor(x => x.CodeInternal)
            .NotEmpty()
            .WithMessage("Code Internal is required.")
            .MaximumLength(40)
            .WithMessage("Code Internal cannot exceed 40 characters.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.PropertyType)
            .NotEmpty()
            .WithMessage("Property Type is required.")
            .Must(BeValidPropertyType)
            .WithMessage("Invalid Property Type. Valid values: HOUSE, CONDO, TOWNHOUSE, MULTI_FAMILY, LAND, APARTMENT, OTHER");

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
            .GreaterThanOrEqualTo(0.0m)
            .WithMessage("Bathrooms must be 0 or greater.")
            .LessThanOrEqualTo(50.0m)
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

        RuleFor(x => x.LotSizeSqft)
            .GreaterThan(0)
            .When(x => x.LotSizeSqft.HasValue)
            .WithMessage("Lot Size must be greater than 0.")
            .LessThanOrEqualTo(100000000)
            .When(x => x.LotSizeSqft.HasValue)
            .WithMessage("Lot Size cannot exceed 100,000,000 sqft.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be 0 or greater.")
            .LessThanOrEqualTo(999999999999.99m)
            .WithMessage("Price cannot exceed $999,999,999,999.99.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency is required.")
            .Length(3)
            .WithMessage("Currency must be exactly 3 characters (ISO code).")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Currency must be uppercase ISO code (e.g., USD, EUR).");

        RuleFor(x => x.HoaFee)
            .GreaterThanOrEqualTo(0)
            .When(x => x.HoaFee.HasValue)
            .WithMessage("HOA Fee must be 0 or greater.")
            .LessThanOrEqualTo(999999.99m)
            .When(x => x.HoaFee.HasValue)
            .WithMessage("HOA Fee cannot exceed $999,999.99.");

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage("Address Line 1 is required.")
            .MaximumLength(200)
            .WithMessage("Address Line 1 cannot exceed 200 characters.");

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200)
            .WithMessage("Address Line 2 cannot exceed 200 characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required.")
            .MaximumLength(120)
            .WithMessage("City cannot exceed 120 characters.");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required.")
            .Length(2)
            .WithMessage("State must be exactly 2 characters.")
            .Matches("^[A-Z]{2}$")
            .WithMessage("State must be uppercase 2-letter code (e.g., CA, NY).");

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .WithMessage("Postal Code is required.")
            .MaximumLength(10)
            .WithMessage("Postal Code cannot exceed 10 characters.")
            .Matches(@"^\d{5}(-\d{4})?$")
            .WithMessage("Postal Code must be in format 12345 or 12345-6789.");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required.")
            .Length(2)
            .WithMessage("Country must be exactly 2 characters.")
            .Matches("^[A-Z]{2}$")
            .WithMessage("Country must be uppercase 2-letter ISO code (e.g., US, CA).");

        RuleFor(x => x.Lat)
            .InclusiveBetween(-90.0m, 90.0m)
            .When(x => x.Lat.HasValue)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Lng)
            .InclusiveBetween(-180.0m, 180.0m)
            .When(x => x.Lng.HasValue)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(x => x.ListingStatus)
            .NotEmpty()
            .WithMessage("Listing Status is required.")
            .Must(BeValidListingStatus)
            .WithMessage("Invalid Listing Status. Valid values: DRAFT, ACTIVE, PENDING, SOLD, OFF_MARKET");

        RuleFor(x => x.ListingDate)
            .GreaterThanOrEqualTo(new DateOnly(2000, 1, 1))
            .When(x => x.ListingDate.HasValue)
            .WithMessage("Listing Date cannot be before year 2000.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddYears(1)))
            .When(x => x.ListingDate.HasValue)
            .WithMessage("Listing Date cannot be more than 1 year in the future.");

        RuleFor(x => x.LastSoldPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LastSoldPrice.HasValue)
            .WithMessage("Last Sold Price must be 0 or greater.");

        // Conditional validation: If ListingStatus is SOLD, LastSoldPrice should be provided
        When(x => x.ListingStatus == "SOLD", () =>
        {
            RuleFor(x => x.LastSoldPrice)
                .NotNull()
                .WithMessage("Last Sold Price is required when Listing Status is SOLD.")
                .GreaterThan(0)
                .WithMessage("Last Sold Price must be greater than 0 when property is sold.");
        });
    }

    private static bool BeValidPropertyType(string propertyType)
    {
        return Enum.TryParse<PropertyType>(propertyType, out _);
    }

    private static bool BeValidListingStatus(string listingStatus)
    {
        return Enum.TryParse<ListingStatus>(listingStatus, out _);
    }
}