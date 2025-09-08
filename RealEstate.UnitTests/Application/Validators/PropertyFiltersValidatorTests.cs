using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Validators;

public class PropertyFiltersValidatorTests
{
    private PropertyFiltersValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new PropertyFiltersValidator();
    }

    [Test]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        var filters = new PropertyFilters
        {
            Q = "luxury apartment",
            OwnerId = Guid.NewGuid(),
            MinPrice = 100000m,
            MaxPrice = 1000000m,
            YearBuilt = 2020,
            MinBedrooms = 2,
            MaxBedrooms = 5,
            MinBathrooms = 1,
            MaxBathrooms = 3,
            MinAreaSqft = 1000,
            MaxAreaSqft = 5000,
            PropertyType = new[] { "APARTMENT", "HOUSE" },
            ListingStatus = new[] { "ACTIVE", "PENDING" },
            IsFeatured = true,
            IsPublished = true,
            State = "CA",
            City = "Los Angeles",
            PostalCode = "90210",
            LatMin = 34.0m,
            LatMax = 34.1m,
            LngMin = -118.3m,
            LngMax = -118.2m,
            Sort = "price",
            Dir = "desc",
            Page = 1,
            PageSize = 20
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithInvalidPageNumber_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            Page = 0
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Page" && e.ErrorMessage.Contains("greater than 0"));
    }

    [Test]
    public void Validate_WithInvalidPageSize_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            PageSize = 0
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize" && e.ErrorMessage.Contains("greater than 0"));
    }

    [Test]
    public void Validate_WithExcessivePageSize_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            PageSize = 101
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize" && e.ErrorMessage.Contains("100"));
    }

    [Test]
    public void Validate_WithNegativePrices_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            MinPrice = -100m,
            MaxPrice = -50m
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinPrice" && e.ErrorMessage.Contains("0"));
        result.Errors.Should().Contain(e => e.PropertyName == "MaxPrice" && e.ErrorMessage.Contains("0"));
    }

    [Test]
    public void Validate_WithMinPriceGreaterThanMaxPrice_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            MinPrice = 500000m,
            MaxPrice = 300000m
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MaxPrice" && e.ErrorMessage.Contains("greater than or equal to Minimum Price"));
    }

    [Test]
    public void Validate_WithInvalidYearBuilt_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            YearBuilt = 1700 // Too old
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "YearBuilt" && e.ErrorMessage.Contains("1800"));
    }

    [Test]
    public void Validate_WithInvalidBedrooms_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            MinBedrooms = -1,
            MaxBedrooms = 51
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinBedrooms" && e.ErrorMessage.Contains("0"));
        result.Errors.Should().Contain(e => e.PropertyName == "MaxBedrooms" && e.ErrorMessage.Contains("50"));
    }

    [Test]
    public void Validate_WithInvalidBathrooms_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            MinBathrooms = -1,
            MaxBathrooms = 51
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinBathrooms" && e.ErrorMessage.Contains("0"));
        result.Errors.Should().Contain(e => e.PropertyName == "MaxBathrooms" && e.ErrorMessage.Contains("50"));
    }

    [Test]
    public void Validate_WithInvalidAreaSqft_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            MinAreaSqft = 0,
            MaxAreaSqft = 0
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "MinAreaSqft" && e.ErrorMessage.Contains("greater than 0"));
        result.Errors.Should().Contain(e => e.PropertyName == "MaxAreaSqft" && e.ErrorMessage.Contains("greater than 0"));
    }

    [Test]
    public void Validate_WithInvalidLatitude_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            LatMin = -91m,
            LatMax = 91m
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LatMin" && e.ErrorMessage.Contains("-90") && e.ErrorMessage.Contains("90"));
        result.Errors.Should().Contain(e => e.PropertyName == "LatMax" && e.ErrorMessage.Contains("-90") && e.ErrorMessage.Contains("90"));
    }

    [Test]
    public void Validate_WithInvalidLongitude_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            LngMin = -181m,
            LngMax = 181m
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "LngMin" && e.ErrorMessage.Contains("-180") && e.ErrorMessage.Contains("180"));
        result.Errors.Should().Contain(e => e.PropertyName == "LngMax" && e.ErrorMessage.Contains("-180") && e.ErrorMessage.Contains("180"));
    }

    [Test]
    public void Validate_WithInvalidSortField_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            Sort = "invalid_field"
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sort");
    }

    [Test]
    public void Validate_WithValidSortFields_ShouldPassValidation()
    {
        var validSortFields = new[] { "name", "price", "listing_date", "year_built", "area_sqft" };

        foreach (var sortField in validSortFields)
        {
            var filters = new PropertyFilters
            {
                Sort = sortField
            };

            var result = _validator.Validate(filters);

            result.IsValid.Should().BeTrue($"Sort field '{sortField}' should be valid");
        }
    }

    [Test]
    public void Validate_WithInvalidDirection_ShouldFailValidation()
    {
        var filters = new PropertyFilters
        {
            Dir = "invalid_direction"
        };

        var result = _validator.Validate(filters);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Dir");
    }

    [Test]
    public void Validate_WithValidDirections_ShouldPassValidation()
    {
        var validDirections = new[] { "asc", "desc" };

        foreach (var direction in validDirections)
        {
            var filters = new PropertyFilters
            {
                Dir = direction
            };

            var result = _validator.Validate(filters);

            result.IsValid.Should().BeTrue($"Direction '{direction}' should be valid");
        }
    }
}