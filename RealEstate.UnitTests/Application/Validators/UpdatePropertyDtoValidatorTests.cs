using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Validators;

public class UpdatePropertyDtoValidatorTests
{
    private UpdatePropertyDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new UpdatePropertyDtoValidator();
    }

    [Test]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Updated Property",
            Description = "Updated description",
            YearBuilt = 2020,
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            AreaSqft = 2000,
            AddressLine = "123 Updated St",
            City = "Updated City",
            State = "NY",
            PostalCode = "10001",
            Lat = 40.7128m,
            Lng = -74.0060m,
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            IsFeatured = true,
            IsPublished = true,
            BasePrice = 400000m,
            TaxAmount = 50000m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithEmptyName_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("required"));
    }

    [Test]
    public void Validate_WithLongName_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = new string('A', 201), // 201 characters
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("200"));
    }

    [Test]
    public void Validate_WithNegativeBedrooms_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = -1,
            Bathrooms = 2,
            ParkingSpaces = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bedrooms" && e.ErrorMessage.Contains("0 or greater"));
    }

    [Test]
    public void Validate_WithTooManyBedrooms_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 51,
            Bathrooms = 2,
            ParkingSpaces = 1,
            RowVersion = new byte[8]
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bedrooms" && e.ErrorMessage.Contains("50"));
    }

    [Test]
    public void Validate_WithNegativeBathrooms_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = -1,
            ParkingSpaces = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bathrooms" && e.ErrorMessage.Contains("0 or greater"));
    }

    [Test]
    public void Validate_WithTooManyBathrooms_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 51,
            ParkingSpaces = 1,
            RowVersion = new byte[8]
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Bathrooms" && e.ErrorMessage.Contains("50"));
    }

    [Test]
    public void Validate_WithNegativeBasePrice_ShouldPassValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            BasePrice = -1000m,
            RowVersion = new byte[8]
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithNegativeTaxAmount_ShouldPassValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            TaxAmount = -500m,
            RowVersion = new byte[8]
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithInvalidListingStatus_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            ListingStatus = "INVALID_STATUS"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ListingStatus");
    }

    [Test]
    public void Validate_WithValidListingStatuses_ShouldPassValidation()
    {
        var validStatuses = new[] { "ACTIVE", "DRAFT", "PENDING", "SOLD", "OFF_MARKET" };

        foreach (var status in validStatuses)
        {
            var dto = new UpdatePropertyDto
            {
                Name = "Test Property",
                Bedrooms = 3,
                Bathrooms = 2,
                ParkingSpaces = 1,
                ListingStatus = status
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"Status '{status}' should be valid");
        }
    }

    [Test]
    public void Validate_WithInvalidLatitude_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            Lat = 91.0m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lat" && e.ErrorMessage.Contains("-90 and 90"));
    }

    [Test]
    public void Validate_WithInvalidLongitude_ShouldFailValidation()
    {
        var dto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            Lng = 181.0m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lng" && e.ErrorMessage.Contains("-180 and 180"));
    }
}