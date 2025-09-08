using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Validators;

public class CreateOwnerDtoValidatorTests
{
    private CreateOwnerDtoValidator _validator;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateOwnerDtoValidator();
    }

    [Test]
    public void Validate_WithValidData_ShouldPassValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Phone = "+1234567890"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithEmptyFullName_ShouldFailValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "",
            Email = "john.doe@example.com",
            Phone = "+1234567890"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName" && e.ErrorMessage.Contains("required"));
    }

    [Test]
    public void Validate_WithLongFullName_ShouldFailValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = new string('A', 181), // 181 characters
            Email = "john.doe@example.com",
            Phone = "+1234567890"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FullName" && e.ErrorMessage.Contains("180"));
    }

    [Test]
    public void Validate_WithInvalidEmail_ShouldFailValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = "invalid-email",
            Phone = "+1234567890"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage.Contains("valid email"));
    }

    [Test]
    public void Validate_WithEmptyEmailButValidPhone_ShouldPassValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = "",
            Phone = "+1234567890"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithLongEmail_ShouldFailValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = new string('a', 170) + "@example.com", // Too long (181 chars total)
            Phone = "+1234567890"
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage.Contains("180"));
    }

    [Test]
    public void Validate_WithValidPhoneFormats_ShouldPassValidation()
    {
        var validPhones = new[] 
        { 
            "+1234567890",
            "+12345678901",
            "+123456789012",
            "+1234567890123",
            "+12345678901234"
        };

        foreach (var phone in validPhones)
        {
            var dto = new CreateOwnerDto
            {
                FullName = "John Doe",
                Email = "john.doe@example.com",
                Phone = phone
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeTrue($"Phone '{phone}' should be valid");
        }
    }

    [Test]
    public void Validate_WithInvalidPhoneFormat_ShouldFailValidation()
    {
        var invalidPhones = new[]
        {
            "012345678", // Starts with 0
            "+0123456789", // Has leading 0 after +
            "abc123", // Invalid characters
            "+", // Just plus sign
            "", // Empty should pass as it's handled by ContactInfo rule
            new string('1', 41) // Too long (over 40 chars)
        };

        foreach (var phone in invalidPhones.Where(p => p != "")) // Skip empty as it should pass
        {
            var dto = new CreateOwnerDto
            {
                FullName = "John Doe",
                Email = "", // No email to force phone validation
                Phone = phone
            };

            var result = _validator.Validate(dto);

            result.IsValid.Should().BeFalse($"Phone '{phone}' should be invalid");
            result.Errors.Should().Contain(e => e.PropertyName == "Phone");
        }
    }

    [Test]
    public void Validate_WithNullPhone_ShouldPassValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Phone = null
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_WithEmptyPhoneButValidEmail_ShouldPassValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = "john.doe@example.com",
            Phone = ""
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_WithNoEmailAndNoPhone_ShouldFailValidation()
    {
        var dto = new CreateOwnerDto
        {
            FullName = "John Doe",
            Email = "",
            Phone = ""
        };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ContactInfo" && e.ErrorMessage.Contains("Either Email or Phone must be provided"));
    }
}