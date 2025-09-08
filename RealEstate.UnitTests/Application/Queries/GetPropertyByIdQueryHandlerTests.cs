using FluentAssertions;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Queries.Properties;
using RealEstate.Domain.Entities;

namespace RealEstate.UnitTests.Application.Queries;

public class GetPropertyByIdQueryHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private Mock<PropertyMapper> _mockMapper;
    private GetPropertyByIdQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockMapper = new Mock<PropertyMapper>();

        _mockUnitOfWork.Setup(x => x.Properties).Returns(_mockPropertyRepository.Object);
        _handler = new GetPropertyByIdQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Test]
    public async Task HandleAsync_WithExistingProperty_ShouldReturnPropertyDetail()
    {
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery { Id = propertyId };

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Test Property",
            CodeInternal = "PROP001",
            PropertyType = "HOUSE",
            Price = 500000m,
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
            TracesCount = 1,
            RowVersion = new byte[8]
        };

        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        result.Id.Should().Be(propertyId);
        result.Name.Should().Be("Test Property");
        result.CodeInternal.Should().Be("PROP001");
    }

    [Test]
    public async Task HandleAsync_WithNonExistentProperty_ShouldReturnNull()
    {
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery { Id = propertyId };

        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PropertyDetailDto)null);

        var result = await _handler.HandleAsync(query);

        result.Should().BeNull();
        _mockPropertyRepository.Verify(x => x.GetPropertyDetailAsync(propertyId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithComplexPropertyData_ShouldReturnCompleteDetail()
    {
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery { Id = propertyId };

        var expectedResult = new PropertyDetailDto
        {
            Id = propertyId,
            Name = "Luxury Penthouse",
            CodeInternal = "LUX001",
            Description = "A beautiful penthouse with ocean view",
            PropertyType = "APARTMENT",
            YearBuilt = 2020,
            Bedrooms = 4,
            Bathrooms = 3,
            ParkingSpaces = 2,
            AreaSqft = 3500,
            Price = 2500000m,
            Currency = "USD",
            AddressLine = "456 Ocean Drive",
            City = "Miami",
            State = "FL",
            PostalCode = "33139",
            Country = "US",
            Lat = 25.7907m,
            Lng = -80.1300m,
            ListingStatus = "ACTIVE",
            ListingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)),
            IsFeatured = true,
            IsPublished = true,
            OwnerFullName = "Luxury Owner",
            Images = new List<PropertyImageDto>
            {
                new PropertyImageDto
                {
                    Id = Guid.NewGuid(),
                    Url = "https://example.com/image1.jpg",
                    IsPrimary = true,
                    SortOrder = 1,
                    Enabled = true,
                    AltText = "Main view"
                },
                new PropertyImageDto
                {
                    Id = Guid.NewGuid(),
                    Url = "https://example.com/image2.jpg",
                    IsPrimary = false,
                    SortOrder = 2,
                    Enabled = true,
                    AltText = "Kitchen"
                }
            },
            LastPriceChange = new PropertyPriceChangeDto
            {
                EventDate = DateTimeOffset.UtcNow.AddDays(-7),
                OldValue = 2400000m,
                NewValue = 2500000m,
                ActorName = "Test Actor"
            },
            TracesCount = 5,
            RowVersion = new byte[8]
        };

        _mockPropertyRepository.Setup(x => x.GetPropertyDetailAsync(propertyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        result.Images.Should().HaveCount(2);
        result.Images.First().IsPrimary.Should().BeTrue();
        result.LastPriceChange.Should().NotBeNull();
        result.LastPriceChange.OldValue.Should().Be(2400000m);
        result.TracesCount.Should().Be(5);
    }
}