using FluentValidation;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Validators;

public class PropertyFiltersValidator : AbstractValidator<PropertyFilters>
{
    private static readonly string[] ValidSortFields = { "name", "price", "listing_date", "year_built", "area_sqft" };
    private static readonly string[] ValidDirections = { "asc", "desc" };

    public PropertyFiltersValidator()
    {
        RuleFor(x => x.Q)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Q))
            .WithMessage("Search query cannot exceed 200 characters.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Minimum Price must be 0 or greater.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Maximum Price must be 0 or greater.")
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .When(x => x.MaxPrice.HasValue && x.MinPrice.HasValue)
            .WithMessage("Maximum Price must be greater than or equal to Minimum Price.");

        RuleFor(x => x.YearFrom)
            .GreaterThan((short)1800)
            .When(x => x.YearFrom.HasValue)
            .WithMessage("Year From must be greater than 1800.")
            .LessThanOrEqualTo((short)(DateTime.Now.Year + 5))
            .When(x => x.YearFrom.HasValue)
            .WithMessage("Year From cannot be more than 5 years in the future.");

        RuleFor(x => x.YearTo)
            .GreaterThan((short)1800)
            .When(x => x.YearTo.HasValue)
            .WithMessage("Year To must be greater than 1800.")
            .LessThanOrEqualTo((short)(DateTime.Now.Year + 5))
            .When(x => x.YearTo.HasValue)
            .WithMessage("Year To cannot be more than 5 years in the future.")
            .GreaterThanOrEqualTo(x => x.YearFrom)
            .When(x => x.YearTo.HasValue && x.YearFrom.HasValue)
            .WithMessage("Year To must be greater than or equal to Year From.");

        RuleFor(x => x.BedroomsFrom)
            .GreaterThanOrEqualTo((short)0)
            .When(x => x.BedroomsFrom.HasValue)
            .WithMessage("Bedrooms From must be 0 or greater.")
            .LessThanOrEqualTo((short)50)
            .When(x => x.BedroomsFrom.HasValue)
            .WithMessage("Bedrooms From cannot exceed 50.");

        RuleFor(x => x.BedroomsTo)
            .GreaterThanOrEqualTo((short)0)
            .When(x => x.BedroomsTo.HasValue)
            .WithMessage("Bedrooms To must be 0 or greater.")
            .LessThanOrEqualTo((short)50)
            .When(x => x.BedroomsTo.HasValue)
            .WithMessage("Bedrooms To cannot exceed 50.")
            .GreaterThanOrEqualTo(x => x.BedroomsFrom)
            .When(x => x.BedroomsTo.HasValue && x.BedroomsFrom.HasValue)
            .WithMessage("Bedrooms To must be greater than or equal to Bedrooms From.");

        RuleFor(x => x.BathroomsFrom)
            .GreaterThanOrEqualTo(0.0m)
            .When(x => x.BathroomsFrom.HasValue)
            .WithMessage("Bathrooms From must be 0 or greater.")
            .LessThanOrEqualTo(50.0m)
            .When(x => x.BathroomsFrom.HasValue)
            .WithMessage("Bathrooms From cannot exceed 50.");

        RuleFor(x => x.BathroomsTo)
            .GreaterThanOrEqualTo(0.0m)
            .When(x => x.BathroomsTo.HasValue)
            .WithMessage("Bathrooms To must be 0 or greater.")
            .LessThanOrEqualTo(50.0m)
            .When(x => x.BathroomsTo.HasValue)
            .WithMessage("Bathrooms To cannot exceed 50.")
            .GreaterThanOrEqualTo(x => x.BathroomsFrom)
            .When(x => x.BathroomsTo.HasValue && x.BathroomsFrom.HasValue)
            .WithMessage("Bathrooms To must be greater than or equal to Bathrooms From.");

        RuleFor(x => x.AreaFrom)
            .GreaterThan(0)
            .When(x => x.AreaFrom.HasValue)
            .WithMessage("Area From must be greater than 0.");

        RuleFor(x => x.AreaTo)
            .GreaterThan(0)
            .When(x => x.AreaTo.HasValue)
            .WithMessage("Area To must be greater than 0.")
            .GreaterThanOrEqualTo(x => x.AreaFrom)
            .When(x => x.AreaTo.HasValue && x.AreaFrom.HasValue)
            .WithMessage("Area To must be greater than or equal to Area From.");

        RuleFor(x => x.LotFrom)
            .GreaterThan(0)
            .When(x => x.LotFrom.HasValue)
            .WithMessage("Lot From must be greater than 0.");

        RuleFor(x => x.LotTo)
            .GreaterThan(0)
            .When(x => x.LotTo.HasValue)
            .WithMessage("Lot To must be greater than 0.")
            .GreaterThanOrEqualTo(x => x.LotFrom)
            .When(x => x.LotTo.HasValue && x.LotFrom.HasValue)
            .WithMessage("Lot To must be greater than or equal to Lot From.");

        RuleFor(x => x.HoaFrom)
            .GreaterThanOrEqualTo(0)
            .When(x => x.HoaFrom.HasValue)
            .WithMessage("HOA From must be 0 or greater.");

        RuleFor(x => x.HoaTo)
            .GreaterThanOrEqualTo(0)
            .When(x => x.HoaTo.HasValue)
            .WithMessage("HOA To must be 0 or greater.")
            .GreaterThanOrEqualTo(x => x.HoaFrom)
            .When(x => x.HoaTo.HasValue && x.HoaFrom.HasValue)
            .WithMessage("HOA To must be greater than or equal to HOA From.");

        RuleForEach(x => x.PropertyType)
            .Must(BeValidPropertyType)
            .WithMessage("Invalid Property Type. Valid values: HOUSE, CONDO, TOWNHOUSE, MULTI_FAMILY, LAND, APARTMENT, OTHER");

        RuleForEach(x => x.ListingStatus)
            .Must(BeValidListingStatus)
            .WithMessage("Invalid Listing Status. Valid values: DRAFT, ACTIVE, PENDING, SOLD, OFF_MARKET");

        RuleFor(x => x.State)
            .Length(2)
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage("State must be exactly 2 characters.")
            .Matches("^[A-Z]{2}$")
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage("State must be uppercase 2-letter code (e.g., CA, NY).");

        RuleFor(x => x.City)
            .MaximumLength(120)
            .When(x => !string.IsNullOrEmpty(x.City))
            .WithMessage("City cannot exceed 120 characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(10)
            .When(x => !string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Postal Code cannot exceed 10 characters.");

        RuleFor(x => x.LatMin)
            .InclusiveBetween(-90.0m, 90.0m)
            .When(x => x.LatMin.HasValue)
            .WithMessage("Latitude Min must be between -90 and 90 degrees.");

        RuleFor(x => x.LatMax)
            .InclusiveBetween(-90.0m, 90.0m)
            .When(x => x.LatMax.HasValue)
            .WithMessage("Latitude Max must be between -90 and 90 degrees.")
            .GreaterThanOrEqualTo(x => x.LatMin)
            .When(x => x.LatMax.HasValue && x.LatMin.HasValue)
            .WithMessage("Latitude Max must be greater than or equal to Latitude Min.");

        RuleFor(x => x.LngMin)
            .InclusiveBetween(-180.0m, 180.0m)
            .When(x => x.LngMin.HasValue)
            .WithMessage("Longitude Min must be between -180 and 180 degrees.");

        RuleFor(x => x.LngMax)
            .InclusiveBetween(-180.0m, 180.0m)
            .When(x => x.LngMax.HasValue)
            .WithMessage("Longitude Max must be between -180 and 180 degrees.")
            .GreaterThanOrEqualTo(x => x.LngMin)
            .When(x => x.LngMax.HasValue && x.LngMin.HasValue)
            .WithMessage("Longitude Max must be greater than or equal to Longitude Min.");

        RuleFor(x => x.Sort)
            .NotEmpty()
            .WithMessage("Sort field is required.")
            .Must(BeValidSortField)
            .WithMessage($"Invalid sort field. Valid values: {string.Join(", ", ValidSortFields)}");

        RuleFor(x => x.Dir)
            .NotEmpty()
            .WithMessage("Sort direction is required.")
            .Must(BeValidDirection)
            .WithMessage($"Invalid sort direction. Valid values: {string.Join(", ", ValidDirections)}");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.")
            .LessThanOrEqualTo(10000)
            .WithMessage("Page cannot exceed 10000.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page Size must be greater than 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page Size cannot exceed 100.");
    }

    private static bool BeValidPropertyType(string propertyType)
    {
        return Enum.TryParse<PropertyType>(propertyType, out _);
    }

    private static bool BeValidListingStatus(string listingStatus)
    {
        return Enum.TryParse<ListingStatus>(listingStatus, out _);
    }

    private static bool BeValidSortField(string sortField)
    {
        return ValidSortFields.Contains(sortField.ToLowerInvariant());
    }

    private static bool BeValidDirection(string direction)
    {
        return ValidDirections.Contains(direction.ToLowerInvariant());
    }
}