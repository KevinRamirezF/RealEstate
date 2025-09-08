using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.Mappers;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Application.Mappers;

public class OwnerMapperTests
{
    private OwnerMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new OwnerMapper();
    }

    [Test]
    public void ToListDto_ShouldMapOwnerToListDto()
    {
        var owner = Owner.Create("John Doe", "john@example.com", "+1234567890", null);

        var result = _mapper.ToListDto(owner);

        result.Should().NotBeNull();
        result.Id.Should().Be(owner.Id);
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
        result.PhoneNumber.Should().Be("+1234567890");
    }

    [Test]
    public void ToDetailDto_ShouldMapOwnerToDetailDto()
    {
        var owner = Owner.Create("Jane Smith", "jane@example.com", null, "EXT001");

        var result = _mapper.ToDetailDto(owner);

        result.Should().NotBeNull();
        result.Id.Should().Be(owner.Id);
        result.FullName.Should().Be("Jane Smith");
        result.Email.Should().Be("jane@example.com");
        result.PhoneNumber.Should().BeNull();
        result.ExternalCode.Should().Be("EXT001");
    }
}