using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Queries.Properties;
using RealEstate.Application.Validators;

namespace RealEstate.UnitTests.Application.Queries;

public class GetPropertiesQueryHandlerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private PropertyFiltersValidator _validator;
    private GetPropertiesQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _validator = new PropertyFiltersValidator();

        _mockUnitOfWork.Setup(x => x.Properties).Returns(_mockPropertyRepository.Object);
        _handler = new GetPropertiesQueryHandler(_mockUnitOfWork.Object, _validator);
    }

    [Test]
    public async Task HandleAsync_WithValidFilters_ShouldReturnPagedResults()
    {
        var filters = new PropertyFilters
        {
            Page = 1,
            PageSize = 10,
            City = "Los Angeles",
            State = "CA",
            MinPrice = 300000m,
            MaxPrice = 800000m
        };
        var query = new GetPropertiesQuery { Filters = filters };

        var expectedResult = new PagedResult<PropertyListDto>
        {
            Items = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Property 1",
                    CodeInternal = "PROP001",
                    City = "Los Angeles",
                    State = "CA",
                    PostalCode = "90210",
                    Price = 500000m,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    AreaSqft = 2500,
                    ListingStatus = "ACTIVE",
                    OwnerFullName = "John Doe",
                    IsFeatured = false,
                    IsPublished = true
                },
                new PropertyListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Property 2",
                    CodeInternal = "PROP002",
                    City = "Los Angeles",
                    State = "CA",
                    PostalCode = "90211",
                    Price = 750000m,
                    YearBuilt = 2020,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    AreaSqft = 3000,
                    ListingStatus = "ACTIVE",
                    OwnerFullName = "Jane Smith",
                    IsFeatured = true,
                    IsPublished = true
                }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 2,
        };

        _mockPropertyRepository.Setup(x => x.GetPagedPropertiesAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(2);
        result.TotalPages.Should().Be(1);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Test]
    public async Task HandleAsync_WithInvalidFilters_ShouldThrowValidationException()
    {
        var filters = new PropertyFilters
        {
            Page = 0, // Invalid page number
            PageSize = -1,  // Invalid page size
            MinPrice = -100m, // Invalid negative price
            MaxPrice = -50m   // Invalid negative price
        };
        var query = new GetPropertiesQuery { Filters = filters };

        var act = async () => await _handler.HandleAsync(query);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task HandleAsync_WithEmptyFilters_ShouldReturnAllProperties()
    {
        var filters = new PropertyFilters();
        var query = new GetPropertiesQuery { Filters = filters };

        var expectedResult = new PagedResult<PropertyListDto>
        {
            Items = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "All Properties Result",
                    CodeInternal = "PROP999",
                    City = "Any City",
                    State = "NY",
                    PostalCode = "10001",
                    Price = 400000m,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    ListingStatus = "ACTIVE",
                    OwnerFullName = "Test Owner",
                    IsFeatured = false,
                    IsPublished = true
                }
            },
            Page = 1,
            PageSize = 20,
            TotalCount = 1,
        };

        _mockPropertyRepository.Setup(x => x.GetPagedPropertiesAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        _mockPropertyRepository.Verify(x => x.GetPagedPropertiesAsync(filters, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WithPriceRangeFilter_ShouldFilterCorrectly()
    {
        var filters = new PropertyFilters
        {
            MinPrice = 500000m,
            MaxPrice = 1000000m,
            Page = 1,
            PageSize = 5
        };
        var query = new GetPropertiesQuery { Filters = filters };

        var expectedResult = new PagedResult<PropertyListDto>
        {
            Items = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Expensive Property",
                    CodeInternal = "PROP-EXP",
                    City = "Beverly Hills",
                    State = "CA",
                    PostalCode = "90210",
                    Price = 750000m,
                    Bedrooms = 5,
                    Bathrooms = 4,
                    AreaSqft = 4000,
                    ListingStatus = "ACTIVE",
                    OwnerFullName = "Rich Owner",
                    IsFeatured = true,
                    IsPublished = true
                }
            },
            Page = 1,
            PageSize = 5,
            TotalCount = 1,
        };

        _mockPropertyRepository.Setup(x => x.GetPagedPropertiesAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().Price.Should().Be(750000m);
        result.Items.First().Name.Should().Be("Expensive Property");
    }

    [Test]
    public async Task HandleAsync_WithLocationFilter_ShouldFilterByLocation()
    {
        var filters = new PropertyFilters
        {
            City = "Miami",
            State = "FL",
            Page = 1,
            PageSize = 10
        };
        var query = new GetPropertiesQuery { Filters = filters };

        var expectedResult = new PagedResult<PropertyListDto>
        {
            Items = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Miami Beach House",
                    CodeInternal = "PROP-MIA",
                    City = "Miami",
                    State = "FL",
                    PostalCode = "33101",
                    Price = 900000m,
                    Bedrooms = 3,
                    Bathrooms = 3,
                    AreaSqft = 2800,
                    ListingStatus = "ACTIVE",
                    OwnerFullName = "Florida Owner",
                    IsFeatured = true,
                    IsPublished = true
                }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 1,
        };

        _mockPropertyRepository.Setup(x => x.GetPagedPropertiesAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().City.Should().Be("Miami");
        result.Items.First().State.Should().Be("FL");
    }

    [Test]
    public async Task HandleAsync_WithPagination_ShouldReturnCorrectPage()
    {
        var filters = new PropertyFilters
        {
            Page = 2,
            PageSize = 2
        };
        var query = new GetPropertiesQuery { Filters = filters };

        var expectedResult = new PagedResult<PropertyListDto>
        {
            Items = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Property on Page 2",
                    CodeInternal = "PROP-P2",
                    City = "Chicago",
                    State = "IL",
                    PostalCode = "60601",
                    Price = 350000m,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    ListingStatus = "ACTIVE",
                    OwnerFullName = "Page 2 Owner",
                    IsFeatured = false,
                    IsPublished = true
                }
            },
            Page = 2,
            PageSize = 2,
            TotalCount = 5,
        };

        _mockPropertyRepository.Setup(x => x.GetPagedPropertiesAsync(filters, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.HasPreviousPage.Should().BeTrue();
        result.HasNextPage.Should().BeTrue();
    }
}