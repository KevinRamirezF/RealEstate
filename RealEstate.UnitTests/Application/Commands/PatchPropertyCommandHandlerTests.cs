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

public class PatchPropertyCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private PropertyMapper _mapper;
    private PatchPropertyDtoValidator _validator;
    private PatchPropertyCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mapper = new PropertyMapper();
        _validator = new PatchPropertyDtoValidator();

        _mockUnitOfWork.Setup(x => x.Properties).Returns(_mockPropertyRepository.Object);

        _handler = new PatchPropertyCommandHandler(_mockUnitOfWork.Object, _mapper, _validator);
    }

    [Test]
    public async Task HandleAsync_WithValidData_ShouldUpdatePropertySuccessfully()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            Name = "Updated Property",
            Description = "Updated description",
            YearBuilt = 2022,
            Bedrooms = 4,
            Bathrooms = 3,
            ParkingSpaces = 2,
            AreaSqft = 2500,
            AddressLine = "Updated Address",
            City = "Updated City",
            State = "CA",
            PostalCode = "90210",
            Lat = 34.0522m,
            Lng = -118.2437m,
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            IsFeatured = true,
            IsPublished = true
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Old Name", 
            PropertyType.HOUSE, 300000m, 30000m, "Old Address", "Old City", "NY", "10001");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = patchDto.Name,
            Description = patchDto.Description,
            YearBuilt = patchDto.YearBuilt.Value,
            Bedrooms = patchDto.Bedrooms.Value,
            Bathrooms = patchDto.Bathrooms.Value,
            ParkingSpaces = patchDto.ParkingSpaces.Value,
            AreaSqft = patchDto.AreaSqft,
            AddressLine = patchDto.AddressLine,
            City = patchDto.City,
            State = patchDto.State,
            PostalCode = patchDto.PostalCode,
            Lat = patchDto.Lat,
            Lng = patchDto.Lng,
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 330000m,
            Currency = "USD",
            Country = "US",
            ListingStatus = patchDto.ListingStatus,
            ListingDate = patchDto.ListingDate.Value,
            IsFeatured = patchDto.IsFeatured.Value,
            IsPublished = patchDto.IsPublished.Value,
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
        result.Name.Should().Be(patchDto.Name);
        result.Description.Should().Be(patchDto.Description);
        result.YearBuilt.Should().Be(patchDto.YearBuilt);
        result.Bedrooms.Should().Be(patchDto.Bedrooms);
        result.Bathrooms.Should().Be(patchDto.Bathrooms);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithNonExistentProperty_ShouldReturnNull()
    {
        var propertyId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            Name = "Updated Property"
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        _mockPropertyRepository.Setup(x => x.GetByIdAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property)null);

        var result = await _handler.HandleAsync(command);

        result.Should().BeNull();
    }

    [Test]
    public async Task HandleAsync_WithBothPrices_ShouldUseDomainMethodForPriceUpdate()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            BasePrice = 400000m,
            TaxAmount = 50000m
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 300000m, 30000m, "123 Main St", "Test City", "NY", "10001");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 450000m, // BasePrice + TaxAmount
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
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
        result.Price.Should().Be(450000m);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithOnlyBasePrice_ShouldUpdatePartialPriceAndRecalculate()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            BasePrice = 500000m
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 300000m, 30000m, "123 Main St", "Test City", "NY", "10001");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 530000m, // New BasePrice + existing TaxAmount
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US",
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 1,
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
        result.Price.Should().Be(530000m);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithOnlyTaxAmount_ShouldUpdatePartialPriceAndRecalculate()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            TaxAmount = 60000m
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 300000m, 30000m, "123 Main St", "Test City", "NY", "10001");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 360000m, // Existing BasePrice + new TaxAmount
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US",
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 1,
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
        result.Price.Should().Be(360000m);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithInvalidListingStatus_ShouldThrowValidationException()
    {
        var propertyId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            ListingStatus = "INVALID_STATUS"
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HandleAsync_WithValidListingStatus_ShouldUpdateStatus()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            ListingStatus = "PENDING"
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 300000m, 30000m, "123 Main St", "Test City", "NY", "10001");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 330000m,
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US",
            ListingStatus = "PENDING",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 1,
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
        result.ListingStatus.Should().Be("PENDING");
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithCoordinates_ShouldUpdateLatLng()
    {
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            Lat = 40.7128m,
            Lng = -74.0060m
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var existingProperty = Property.Create(ownerId, "PROP001", "Test Property", 
            PropertyType.HOUSE, 300000m, 30000m, "123 Main St", "Test City", "NY", "10001");

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 330000m,
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Test City",
            State = "NY",
            PostalCode = "10001",
            Country = "US",
            Lat = 40.7128m,
            Lng = -74.0060m,
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today),
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 1,
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
        result.Lat.Should().Be(40.7128m);
        result.Lng.Should().Be(-74.0060m);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var propertyId = Guid.NewGuid();
        var patchDto = new PatchPropertyDto
        {
            Name = "", // Empty name
            Bedrooms = -1 // Invalid bedrooms
        };
        var command = new PatchPropertyCommand { Id = propertyId, Data = patchDto };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>();
    }
}