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

public class CreatePropertyCommandHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private Mock<IOwnerRepository> _mockOwnerRepository;
    private PropertyMapper _mapper;
    private CreatePropertyDtoValidator _validator;
    private CreatePropertyCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockOwnerRepository = new Mock<IOwnerRepository>();
        _mapper = new PropertyMapper();
        _validator = new CreatePropertyDtoValidator();

        _mockUnitOfWork.Setup(x => x.Properties).Returns(_mockPropertyRepository.Object);
        _mockUnitOfWork.Setup(x => x.Owners).Returns(_mockOwnerRepository.Object);

        _handler = new CreatePropertyCommandHandler(_mockUnitOfWork.Object, _mapper, _validator);
    }

    [Test]
    public async Task HandleAsync_WithValidData_ShouldCreatePropertySuccessfully()
    {
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var createDto = CreateValidPropertyDto(ownerId);
        var command = new CreatePropertyCommand { Data = createDto };

        var owner = Owner.Create("Test Owner", "owner@test.com", "+1234567890");
        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = createDto.Name,
            CodeInternal = createDto.CodeInternal,
            PropertyType = createDto.PropertyType,
            Price = createDto.BasePrice + createDto.TaxAmount,
            AddressLine = createDto.AddressLine,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode,
            Country = createDto.Country,
            Currency = createDto.Currency,
            ListingStatus = createDto.ListingStatus,
            IsFeatured = createDto.IsFeatured,
            IsPublished = createDto.IsPublished,
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 1,
            RowVersion = new byte[8]
        };

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(owner);
        _mockPropertyRepository.Setup(x => x.ExistsByCodeInternalAsync(createDto.CodeInternal, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.CodeInternal.Should().Be(createDto.CodeInternal);
        result.Price.Should().Be(createDto.BasePrice + createDto.TaxAmount);
        _mockPropertyRepository.Verify(x => x.AddAsync(It.IsAny<Property>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithInvalidData_ShouldThrowValidationException()
    {
        var createDto = new CreatePropertyDto
        {
            OwnerId = Guid.Empty,
            CodeInternal = "",
            Name = "",
            PropertyType = "INVALID",
            BasePrice = -100,
            TaxAmount = -50,
            AddressLine = "",
            City = "",
            State = "INVALID",
            PostalCode = "INVALID"
        };
        var command = new CreatePropertyCommand { Data = createDto };

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HandleAsync_WithNonExistentOwner_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var createDto = CreateValidPropertyDto(ownerId);
        var command = new CreatePropertyCommand { Data = createDto };

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Owner)null);

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"Owner with ID {ownerId} does not exist.");
    }

    [Test]
    public async Task HandleAsync_WithDuplicateCodeInternal_ShouldThrowValidationException()
    {
        var ownerId = Guid.NewGuid();
        var createDto = CreateValidPropertyDto(ownerId);
        var command = new CreatePropertyCommand { Data = createDto };
        var owner = Owner.Create("Test Owner", "owner@test.com", "+1234567890");

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(owner);
        _mockPropertyRepository.Setup(x => x.ExistsByCodeInternalAsync(createDto.CodeInternal, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = async () => await _handler.HandleAsync(command);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage($"Property with CodeInternal '{createDto.CodeInternal}' already exists.");
    }

    [Test]
    public async Task HandleAsync_WithOptionalProperties_ShouldSetAllProperties()
    {
        var ownerId = Guid.NewGuid();
        var createDto = CreateValidPropertyDto(ownerId);
        createDto.Description = "Test Description";
        createDto.YearBuilt = 2020;
        createDto.Bedrooms = 3;
        createDto.Bathrooms = 2;
        createDto.ParkingSpaces = 2;
        createDto.AreaSqft = 2500;
        createDto.Lat = 34.0522m;
        createDto.Lng = -118.2437m;
        createDto.ListingDate = DateOnly.FromDateTime(DateTime.Today);
        createDto.IsFeatured = true;
        createDto.IsPublished = false;

        var command = new CreatePropertyCommand { Data = createDto };
        var owner = Owner.Create("Test Owner", "owner@test.com", "+1234567890");

        var expectedResult = new PropertyDetailDto
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            CodeInternal = createDto.CodeInternal,
            Description = createDto.Description,
            PropertyType = createDto.PropertyType,
            YearBuilt = createDto.YearBuilt,
            Bedrooms = createDto.Bedrooms,
            Bathrooms = createDto.Bathrooms,
            ParkingSpaces = createDto.ParkingSpaces,
            AreaSqft = createDto.AreaSqft,
            Price = createDto.BasePrice + createDto.TaxAmount,
            Currency = createDto.Currency,
            AddressLine = createDto.AddressLine,
            City = createDto.City,
            State = createDto.State,
            PostalCode = createDto.PostalCode,
            Country = createDto.Country,
            Lat = createDto.Lat,
            Lng = createDto.Lng,
            ListingStatus = createDto.ListingStatus,
            ListingDate = createDto.ListingDate ?? DateOnly.FromDateTime(DateTime.Today),
            IsFeatured = createDto.IsFeatured,
            IsPublished = createDto.IsPublished,
            OwnerFullName = "Test Owner",
            Images = new List<PropertyImageDto>(),
            TracesCount = 1,
            RowVersion = new byte[8]
        };

        _mockOwnerRepository.Setup(x => x.GetByIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(owner);
        _mockPropertyRepository.Setup(x => x.ExistsByCodeInternalAsync(createDto.CodeInternal, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Description.Should().Be(createDto.Description);
        result.YearBuilt.Should().Be(createDto.YearBuilt);
        result.Bedrooms.Should().Be(createDto.Bedrooms);
        result.Bathrooms.Should().Be(createDto.Bathrooms);
        result.ParkingSpaces.Should().Be(createDto.ParkingSpaces);
        result.AreaSqft.Should().Be(createDto.AreaSqft);
        result.Lat.Should().Be(createDto.Lat);
        result.Lng.Should().Be(createDto.Lng);
        result.IsFeatured.Should().Be(createDto.IsFeatured);
        result.IsPublished.Should().Be(createDto.IsPublished);
    }

    private static CreatePropertyDto CreateValidPropertyDto(Guid ownerId)
    {
        return new CreatePropertyDto
        {
            OwnerId = ownerId,
            CodeInternal = "PROP001",
            Name = "Test Property",
            PropertyType = "HOUSE",
            BasePrice = 400000m,
            TaxAmount = 50000m,
            Currency = "USD",
            AddressLine = "123 Main St",
            City = "Los Angeles",
            State = "CA",
            PostalCode = "90210",
            Country = "US",
            ListingStatus = "ACTIVE",
            IsFeatured = false,
            IsPublished = true
        };
    }
}