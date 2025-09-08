using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Validators;

public class AddImageDtoValidatorTests
{
    private AddImageDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new AddImageDtoValidator();
    }

    [Test]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "S3",
            AltText = "Test Image",
            IsPrimary = true,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithEmptyUrl_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "",
            StorageProvider = "S3",
            IsPrimary = false,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Url" && e.ErrorMessage.Contains("required"));
    }

    [Test]
    public void Validate_WithTooLongUrl_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/" + new string('a', 1000), // Too long
            StorageProvider = "S3",
            IsPrimary = false,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Url" && e.ErrorMessage.Contains("1000"));
    }

    [Test]
    public void Validate_WithInvalidStorageProvider_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "INVALID_PROVIDER",
            IsPrimary = false,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StorageProvider");
    }

    [Test]
    public void Validate_WithValidStorageProviders_ShouldPassValidation()
    {
        var validProviders = new[] { "S3", "GCS", "AZURE", "LOCAL", "EXTERNAL" };

        foreach (var provider in validProviders)
        {
            var dto = new AddImageDto
            {
                Url = "https://example.com/image.jpg",
                StorageProvider = provider,
                IsPrimary = false,
                SortOrder = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"StorageProvider '{provider}' should be valid");
        }
    }

    [Test]
    public void Validate_WithNegativeSortOrder_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "S3",
            IsPrimary = false,
            SortOrder = -1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SortOrder" && e.ErrorMessage.Contains("0"));
    }

    [Test]
    public void Validate_WithExcessiveSortOrder_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "S3",
            IsPrimary = false,
            SortOrder = 10000
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SortOrder" && e.ErrorMessage.Contains("9999"));
    }

    [Test]
    public void Validate_WithLongAltText_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "S3",
            AltText = new string('A', 501), // Too long
            IsPrimary = false,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AltText" && e.ErrorMessage.Contains("200"));
    }

    [Test]
    public void Validate_WithNullAltText_ShouldPassValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "S3",
            AltText = null,
            IsPrimary = false,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithEmptyStorageProvider_ShouldFailValidation()
    {
        var dto = new AddImageDto
        {
            Url = "https://example.com/image.jpg",
            StorageProvider = "",
            IsPrimary = false,
            SortOrder = 1
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StorageProvider" && e.ErrorMessage.Contains("required"));
    }

    [Test]
    public void Validate_WithValidUrlFormats_ShouldPassValidation()
    {
        var validUrls = new[]
        {
            "https://example.com/image.jpg",
            "http://localhost/image.png",
            "https://cdn.example.com/path/to/image.gif",
            "http://example.com/local/path/image.jpg",
            "https://example.com/data/image.jpeg"
        };

        foreach (var url in validUrls)
        {
            var dto = new AddImageDto
            {
                Url = url,
                StorageProvider = "S3",
                IsPrimary = false,
                SortOrder = 1
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"URL '{url}' should be valid");
        }
    }
}