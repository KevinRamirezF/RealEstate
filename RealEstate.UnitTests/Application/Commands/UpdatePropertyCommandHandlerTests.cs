using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Commands.Properties;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.UnitTests.Application.Commands;

public class UpdatePropertyCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private UpdatePropertyDtoValidator _validator;
    private UpdatePropertyCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _validator = new UpdatePropertyDtoValidator();

        _mockUnitOfWork.Setup(x => x.Properties).Returns(_mockPropertyRepository.Object);

        _handler = new UpdatePropertyCommandHandler(_mockUnitOfWork.Object, new PropertyMapper(), _validator);
    }

    [Test]
    public async Task HandleAsync_WithValidData_ShouldUpdatePropertySuccessfully()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var updateDto = CreateValidUpdateDto();
        var command = new UpdatePropertyCommand { Id = propertyId, Data = updateDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Old Name", 
            PropertyType.HOUSE, 300000m, 30000m, "Old Address", "Old City", "CA", "90210");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = updateDto.Name,
            Description = updateDto.Description,
            Bedrooms = updateDto.Bedrooms,
            Bathrooms = updateDto.Bathrooms,
            ParkingSpaces = updateDto.ParkingSpaces,
            AreaSqft = updateDto.AreaSqft,
            AddressLine = updateDto.AddressLine,
            City = updateDto.City,
            State = updateDto.State,
            PostalCode = updateDto.PostalCode,
            Lat = updateDto.Lat,
            Lng = updateDto.Lng,
            IsFeatured = updateDto.IsFeatured,
            IsPublished = updateDto.IsPublished,
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 330000m,
            Currency = "USD",
            Country = "US",
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 2,
            RowVersion = new byte[8]
        };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Name.Should().Be(updateDto.Name);
        result.Description.Should().Be(updateDto.Description);
        result.Bedrooms.Should().Be(updateDto.Bedrooms);
        result.Bathrooms.Should().Be(updateDto.Bathrooms);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var propertyId = Guid.NewGuid();
        var updateDto = new UpdatePropertyDto
        {
            Name = "", // Invalid empty name
            Bedrooms = -1, // Invalid negative bedrooms
            Bathrooms = -1 // Invalid negative bathrooms
        };
        var command = new UpdatePropertyCommand { Id = propertyId, Data = updateDto };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HandleAsync_WithNonExistentProperty_ShouldReturnNull()
    {
        var propertyId = Guid.NewGuid();
        var updateDto = CreateValidUpdateDto();
        var command = new UpdatePropertyCommand { Id = propertyId, Data = updateDto };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property)null);

        var result = await _handler.HandleAsync(command);

        result.Should().BeNull();
    }

    [Test]
    public async Task HandleAsync_WithPartialUpdate_ShouldUpdateOnlyProvidedFields()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdatePropertyDto
        {
            Name = "Updated Name Only",
            Bedrooms = 4,
            Bathrooms = 2,
            ParkingSpaces = 1,
            RowVersion = new byte[8]
        };
        var command = new UpdatePropertyCommand { Id = propertyId, Data = updateDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Old Name", 
            PropertyType.HOUSE, 300000m, 30000m, "Old Address", "Old City", "CA", "90210");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = updateDto.Name,
            Bedrooms = updateDto.Bedrooms,
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 330000m,
            Currency = "USD",
            AddressLine = "Old Address",
            City = "Old City",
            State = "CA",
            PostalCode = "90210",
            Country = "US",
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 2,
            RowVersion = new byte[8]
        };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Name.Should().Be(updateDto.Name);
        result.Bedrooms.Should().Be(updateDto.Bedrooms);
        result.AddressLine.Should().Be("Old Address"); // Should remain unchanged
        result.City.Should().Be("Old City"); // Should remain unchanged
    }

    [Test]
    public async Task HandleAsync_WithPriceUpdate_ShouldUpdatePriceCorrectly()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var updateDto = new UpdatePropertyDto
        {
            Name = "Test Property",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 1,
            BasePrice = 500000m,
            TaxAmount = 60000m,
            RowVersion = new byte[8]
        };
        var command = new UpdatePropertyCommand { Id = propertyId, Data = updateDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 300000m, 30000m, "123 Main St", "Los Angeles", "CA", "90210");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 560000m, // BasePrice + TaxAmount
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Los Angeles",
            State = "CA",
            PostalCode = "90210",
            Country = "US",
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 2,
            RowVersion = new byte[8]
        };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProperty);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Price.Should().Be(560000m);
    }

    private static UpdatePropertyDto CreateValidUpdateDto()
    {
        return new UpdatePropertyDto
        {
            Name = "Updated Property Name",
            Description = "Updated description",
            Bedrooms = 3,
            Bathrooms = 2,
            ParkingSpaces = 2,
            AreaSqft = 2500,
            AddressLine = "Updated Address",
            City = "Updated City",
            State = "NY",
            PostalCode = "10001",
            Lat = 40.7128m,
            Lng = -74.0060m,
            IsFeatured = true,
            IsPublished = false,
            RowVersion = new byte[8]
        };
    }
}