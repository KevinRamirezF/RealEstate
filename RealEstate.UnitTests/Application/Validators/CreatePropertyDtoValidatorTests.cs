using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Validators;

public class CreatePropertyDtoValidatorTests
{
    private CreatePropertyDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CreatePropertyDtoValidator();
    }

    [Test]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        var dto = CreateValidDto();

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithEmptyOwnerId_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.OwnerId = Guid.Empty;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OwnerId" && e.ErrorMessage == "Owner ID is required.");
    }

    [Test]
    public void Validate_WithEmptyCodeInternal_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.CodeInternal = "";

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CodeInternal" && e.ErrorMessage == "Code Internal is required.");
    }

    [Test]
    public void Validate_WithLongCodeInternal_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.CodeInternal = new string('A', 41); // 41 characters

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CodeInternal" && e.ErrorMessage == "Code Internal cannot exceed 40 characters.");
    }

    [Test]
    public void Validate_WithEmptyName_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Name = "";

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Test]
    public void Validate_WithLongName_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Name = new string('A', 201); // 201 characters

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_WithLongDescription_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Description = new string('A', 2001); // 2001 characters

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage == "Description cannot exceed 2000 characters.");
    }

    [Test]
    public void Validate_WithInvalidPropertyType_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.PropertyType = "INVALID_TYPE";

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PropertyType" && 
            e.ErrorMessage == "Invalid Property Type. Valid values: HOUSE, CONDO, TOWNHOUSE, MULTI_FAMILY, LAND, APARTMENT, OTHER");
    }

    [Test]
    public void Validate_WithValidPropertyTypes_ShouldPassValidation()
    {
        var validTypes = new[] { "HOUSE", "CONDO", "TOWNHOUSE", "MULTI_FAMILY", "LAND", "APARTMENT", "OTHER" };

        foreach (var type in validTypes)
        {
            var dto = CreateValidDto();
            dto.PropertyType = type;

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"PropertyType '{type}' should be valid");
        }
    }

    [Test]
    public void Validate_WithOldYearBuilt_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.YearBuilt = 1799;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "YearBuilt" && e.ErrorMessage == "Year Built must be greater than 1800.");
    }

    [Test]
    public void Validate_WithFutureYearBuilt_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.YearBuilt = (short)(DateTime.Now.Year + 6);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "YearBuilt" && e.ErrorMessage == "Year Built cannot be more than 5 years in the future.");
    }

    [Test]
    public void Validate_WithNegativeBedrooms_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Bedrooms = -1;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bedrooms" && e.ErrorMessage == "Bedrooms must be 0 or greater.");
    }

    [Test]
    public void Validate_WithTooManyBedrooms_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Bedrooms = 51;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bedrooms" && e.ErrorMessage == "Bedrooms cannot exceed 50.");
    }

    [Test]
    public void Validate_WithNegativeBathrooms_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Bathrooms = -1;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bathrooms" && e.ErrorMessage == "Bathrooms must be 0 or greater.");
    }

    [Test]
    public void Validate_WithTooManyBathrooms_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Bathrooms = 51;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bathrooms" && e.ErrorMessage == "Bathrooms cannot exceed 50.");
    }

    [Test]
    public void Validate_WithNegativeBasePrice_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.BasePrice = -1m;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BasePrice" && e.ErrorMessage == "Base Price must be 0 or greater.");
    }

    [Test]
    public void Validate_WithExcessiveBasePrice_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.BasePrice = 1000000000000m; // Too large

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BasePrice" && e.ErrorMessage == "Base Price cannot exceed $999,999,999,999.99.");
    }

    [Test]
    public void Validate_WithNegativeTaxAmount_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.TaxAmount = -1m;

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TaxAmount" && e.ErrorMessage == "Tax Amount must be 0 or greater.");
    }

    [Test]
    public void Validate_WithInvalidCurrency_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Currency = "usd"; // Should be uppercase

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency" && e.ErrorMessage == "Currency must be uppercase ISO code (e.g., USD, EUR).");
    }

    [Test]
    public void Validate_WithInvalidState_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.State = "California"; // Should be 2-letter code

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "State" && e.ErrorMessage == "State must be exactly 2 characters.");
    }

    [Test]
    public void Validate_WithInvalidPostalCode_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.PostalCode = "ABC123"; // Invalid format

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PostalCode" && e.ErrorMessage == "Postal Code must be in format 12345 or 12345-6789.");
    }

    [Test]
    public void Validate_WithValidPostalCodeFormats_ShouldPassValidation()
    {
        var validPostalCodes = new[] { "90210", "12345-6789" };

        foreach (var postalCode in validPostalCodes)
        {
            var dto = CreateValidDto();
            dto.PostalCode = postalCode;

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"PostalCode '{postalCode}' should be valid");
        }
    }

    [Test]
    public void Validate_WithInvalidLatitude_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Lat = 91.0m; // Too large

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lat" && e.ErrorMessage == "Latitude must be between -90 and 90 degrees.");
    }

    [Test]
    public void Validate_WithInvalidLongitude_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.Lng = 181.0m; // Too large

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lng" && e.ErrorMessage == "Longitude must be between -180 and 180 degrees.");
    }

    [Test]
    public void Validate_WithInvalidListingStatus_ShouldFailValidation()
    {
        var dto = CreateValidDto();
        dto.ListingStatus = "INVALID_STATUS";

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ListingStatus" && 
            e.ErrorMessage == "Invalid Listing Status. Valid values: DRAFT, ACTIVE, PENDING, SOLD, OFF_MARKET");
    }

    [Test]
    public void Validate_WithValidListingStatuses_ShouldPassValidation()
    {
        var validStatuses = new[] { "DRAFT", "ACTIVE", "PENDING", "SOLD", "OFF_MARKET" };

        foreach (var status in validStatuses)
        {
            var dto = CreateValidDto();
            dto.ListingStatus = status;

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"ListingStatus '{status}' should be valid");
        }
    }

    private static CreatePropertyDto CreateValidDto()
    {
        return new CreatePropertyDto
        {
            OwnerId = Guid.NewGuid(),
            CodeInternal = "PROP001",
            Name = "Test Property",
            PropertyType = "HOUSE",
            YearBuilt = 2020,
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 2,
            AreaSqft = 2500,
            BasePrice = 400000m,
            TaxAmount = 50000m,
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Los Angeles",
            State = "CA",
            PostalCode = "90210",
            Country = "US",
            Lat = 34.0522m,
            Lng = -118.2437m,
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            IsFeatured = false,
            IsPublished = true
        };
    }
}