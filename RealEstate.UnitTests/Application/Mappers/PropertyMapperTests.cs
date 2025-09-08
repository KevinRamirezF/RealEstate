using FluentAssertions;
using NUnit.Framework;
using RealEstate.Application.Mappers;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.UnitTests.Application.Mappers;

public class PropertyMapperTests
{
    private PropertyMapper _mapper;

    [SetUp]
    public void Setup()
    {
        _mapper = new PropertyMapper();
    }

    [Test]
    public void ToListDto_ShouldMapAllPropertiesCorrectly()
    {
        var ownerId = Guid.NewGuid();
        var property = Property.Create(
            ownerId, "PROP001", "Test Property", PropertyType.HOUSE,
            400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        property.Update(yearBuilt: 2020, bedrooms: 3, bathrooms: 2, areaSqft: 2500, isFeatured: true);

        var ownerFullName = "John Doe";

        var result = _mapper.ToListDto(property, ownerFullName);

        result.Should().NotBeNull();
        result.Id.Should().Be(property.Id);
        result.Name.Should().Be("Test Property");
        result.CodeInternal.Should().Be("PROP001");
        result.City.Should().Be("Los Angeles");
        result.State.Should().Be("CA");
        result.PostalCode.Should().Be("90210");
        result.Price.Should().Be(450000m); // 400000 + 50000
        result.YearBuilt.Should().Be(2020);
        result.Bedrooms.Should().Be(3);
        result.Bathrooms.Should().Be(2);
        result.AreaSqft.Should().Be(2500);
        result.ListingStatus.Should().Be("ACTIVE");
        result.OwnerFullName.Should().Be("John Doe");
        result.IsFeatured.Should().BeTrue();
        result.IsPublished.Should().BeTrue();
        result.PrimaryImageUrl.Should().BeNull(); // No images added
    }

    [Test]
    public void ToListDto_WithImages_ShouldMapPrimaryImageUrl()
    {
        var ownerId = Guid.NewGuid();
        var property = Property.Create(
            ownerId, "PROP001", "Test Property", PropertyType.HOUSE,
            400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        property.AddImage("https://example.com/image1.jpg", StorageProvider.S3, "Image 1", false, 1);
        property.AddImage("https://example.com/primary.jpg", StorageProvider.S3, "Primary Image", true, 0);

        var ownerFullName = "John Doe";

        var result = _mapper.ToListDto(property, ownerFullName);

        result.Should().NotBeNull();
        result.PrimaryImageUrl.Should().Be("https://example.com/primary.jpg");
    }

    [Test]
    public void ToDetailDto_ShouldMapAllPropertiesCorrectly()
    {
        var ownerId = Guid.NewGuid();
        var property = Property.Create(
            ownerId, "PROP001", "Test Property", PropertyType.HOUSE,
            400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        property.Update(
            description: "Beautiful family home",
            yearBuilt: 2020,
            bedrooms: 3,
            bathrooms: 2,
            parkingSpaces: 2,
            areaSqft: 2500,
            lat: 34.0522m,
            lng: -118.2437m,
            isFeatured: true,
            isPublished: false
        );

        var ownerFullName = "John Doe";

        var result = _mapper.ToDetailDto(property, ownerFullName);

        result.Should().NotBeNull();
        result.Id.Should().Be(property.Id);
        result.Name.Should().Be("Test Property");
        result.CodeInternal.Should().Be("PROP001");
        result.Description.Should().Be("Beautiful family home");
        result.PropertyType.Should().Be("HOUSE");
        result.YearBuilt.Should().Be(2020);
        result.Bedrooms.Should().Be(3);
        result.Bathrooms.Should().Be(2);
        result.ParkingSpaces.Should().Be(2);
        result.AreaSqft.Should().Be(2500);
        result.Price.Should().Be(450000m);
        result.Currency.Should().Be("USD");
        result.AddressLine.Should().Be("123 Main St");
        result.City.Should().Be("Los Angeles");
        result.State.Should().Be("CA");
        result.PostalCode.Should().Be("90210");
        result.Country.Should().Be("US");
        result.Lat.Should().Be(34.0522m);
        result.Lng.Should().Be(-118.2437m);
        result.ListingStatus.Should().Be("ACTIVE");
        result.IsFeatured.Should().BeTrue();
        result.IsPublished.Should().BeFalse();
        result.OwnerFullName.Should().Be("John Doe");
        result.TracesCount.Should().BeGreaterThan(0);
        result.RowVersion.Should().NotBeNull();
    }

    [Test]
    public void ToDetailDto_WithImages_ShouldMapImagesCorrectly()
    {
        var ownerId = Guid.NewGuid();
        var property = Property.Create(
            ownerId, "PROP001", "Test Property", PropertyType.HOUSE,
            400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        property.AddImage("https://example.com/image2.jpg", StorageProvider.S3, "Image 2", false, 2);
        property.AddImage("https://example.com/primary.jpg", StorageProvider.S3, "Primary Image", true, 0);
        property.AddImage("https://example.com/image1.jpg", StorageProvider.S3, "Image 1", false, 1);

        var ownerFullName = "John Doe";

        var result = _mapper.ToDetailDto(property, ownerFullName);

        result.Should().NotBeNull();
        result.Images.Should().HaveCount(3);
        // Images should be ordered by primary first, then by sort order
        result.Images[0].IsPrimary.Should().BeTrue();
        result.Images[0].Url.Should().Be("https://example.com/primary.jpg");
        result.Images[0].SortOrder.Should().Be(0);
        result.Images[1].Url.Should().Be("https://example.com/image1.jpg");
        result.Images[1].SortOrder.Should().Be(1);
        result.Images[2].Url.Should().Be("https://example.com/image2.jpg");
        result.Images[2].SortOrder.Should().Be(2);
    }

    [Test]
    public void ToDetailDto_WithPriceChangeTrace_ShouldMapLastPriceChange()
    {
        var ownerId = Guid.NewGuid();
        var property = Property.Create(
            ownerId, "PROP001", "Test Property", PropertyType.HOUSE,
            400000m, 50000m, "123 Main St", "Los Angeles", "CA", "90210");

        // Create a price change to generate a trace
        property.ChangePrice(500000m, 60000m, "Test Actor");

        var lastPriceTrace = property.Traces.LastOrDefault(t => t.EventType == TraceEventType.PRICE_CHANGE);
        var ownerFullName = "John Doe";

        var result = _mapper.ToDetailDto(property, ownerFullName, lastPriceTrace);

        result.Should().NotBeNull();
        result.LastPriceChange.Should().NotBeNull();
        result.LastPriceChange.OldValue.Should().Be(450000m); // Original total price
        result.LastPriceChange.ActorName.Should().Be("Test Actor");
        result.LastPriceChange.EventDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Test]
    public void ToImageDto_ShouldMapAllImageProperties()
    {
        var propertyId = Guid.NewGuid();
        var image = PropertyImage.Create(
            propertyId, 
            "https://example.com/image.jpg", 
            StorageProvider.S3, 
            "Test Alt Text", 
            true, 
            5);

        var result = _mapper.ToImageDto(image);

        result.Should().NotBeNull();
        result.Id.Should().Be(image.Id);
        result.Url.Should().Be("https://example.com/image.jpg");
        result.IsPrimary.Should().BeTrue();
        result.SortOrder.Should().Be(5);
        result.Enabled.Should().BeTrue();
        result.AltText.Should().Be("Test Alt Text");
    }

    [Test]
    public void ToLastPriceChangeDto_WithNullTrace_ShouldReturnNull()
    {
        var result = _mapper.ToLastPriceChangeDto(null);

        result.Should().BeNull();
    }

    [Test]
    public void ToLastPriceChangeDto_WithNonPriceChangeTrace_ShouldReturnNull()
    {
        var propertyId = Guid.NewGuid();
        var trace = PropertyTrace.Create(propertyId, TraceEventType.CREATED, "Property created");

        var result = _mapper.ToLastPriceChangeDto(trace);

        result.Should().BeNull();
    }

    [Test]
    public void ToLastPriceChangeDto_WithPriceChangeTrace_ShouldMapCorrectly()
    {
        var propertyId = Guid.NewGuid();
        var trace = PropertyTrace.Create(
            propertyId, 
            TraceEventType.PRICE_CHANGE, 
            "Price changed", 
            400000m,    // oldTotalPrice
            350000m,    // oldPriceBase
            50000m,     // oldTaxAmount
            "Test Actor"
        );

        var result = _mapper.ToLastPriceChangeDto(trace);

        result.Should().NotBeNull();
        result.EventDate.Should().Be(trace.EventDate);
        result.OldValue.Should().Be(400000m);
        result.TaxAmount.Should().Be(50000m);
        result.ActorName.Should().Be("Test Actor");
        result.NewValue.Should().BeNull(); // New values are in the current Property entity
    }
}