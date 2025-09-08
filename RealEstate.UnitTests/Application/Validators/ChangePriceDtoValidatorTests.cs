using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Validators;

public class ChangePriceDtoValidatorTests
{
    private ChangePriceDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new ChangePriceDtoValidator();
    }

    [Test]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 500000m,
            TaxAmount = 60000m,
            ActorName = "Test Actor"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithZeroPrices_ShouldPassValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 0m,
            TaxAmount = 0m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithNegativeBasePrice_ShouldFailValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = -1000m,
            TaxAmount = 50000m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BasePrice" && e.ErrorMessage.Contains("0 or greater"));
    }

    [Test]
    public void Validate_WithNegativeTaxAmount_ShouldFailValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 400000m,
            TaxAmount = -5000m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TaxAmount" && e.ErrorMessage.Contains("0 or greater"));
    }

    [Test]
    public void Validate_WithExcessiveBasePrice_ShouldFailValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 1000000000000m, // Too large
            TaxAmount = 50000m
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BasePrice");
    }

    [Test]
    public void Validate_WithExcessiveTaxAmount_ShouldFailValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 400000m,
            TaxAmount = 100000000m // Too large
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TaxAmount");
    }

    [Test]
    public void Validate_WithLongActorName_ShouldFailValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 400000m,
            TaxAmount = 50000m,
            ActorName = new string('A', 201) // Too long
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ActorName" && e.ErrorMessage.Contains("180"));
    }

    [Test]
    public void Validate_WithoutActorName_ShouldPassValidation()
    {
        var dto = new ChangePriceDto
        {
            BasePrice = 400000m,
            TaxAmount = 50000m,
            ActorName = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}