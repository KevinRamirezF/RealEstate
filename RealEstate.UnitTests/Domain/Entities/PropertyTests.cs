using FluentAssertions;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.UnitTests.Domain.Entities;

[TestFixture]
public class PropertyTests
{
    private Guid _ownerId;
    private const string ValidCodeInternal = "PROP001";
    private const string ValidName = "Beautiful House";
    private const decimal ValidBasePrice = 100000m;
    private const decimal ValidTaxAmount = 5000m;
    private const string ValidAddress = "123 Main St";
    private const string ValidCity = "Miami";
    private const string ValidState = "FL";
    private const string ValidPostalCode = "33101";

    [SetUp]
    public void Setup()
    {
        _ownerId = Guid.NewGuid();
    }

    [Test]
    public void Create_WithValidData_ShouldCreateProperty()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE, 
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.Should().NotBeNull();
        property.Id.Should().NotBeEmpty();
        property.OwnerId.Should().Be(_ownerId);
        property.CodeInternal.Should().Be(ValidCodeInternal);
        property.Name.Should().Be(ValidName);
        property.PropertyType.Should().Be(PropertyType.HOUSE);
        property.BasePrice.Should().Be(ValidBasePrice);
        property.TaxAmount.Should().Be(ValidTaxAmount);
        property.Price.Should().Be(ValidBasePrice + ValidTaxAmount);
    }

    [Test]
    public void Create_WithNegativeBasePrice_ShouldThrowException()
    {
        var action = () => Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            -1000m, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        action.Should().Throw<ArgumentException>()
            .WithMessage("BasePrice cannot be negative.*");
    }

    [Test]
    public void Create_WithNegativeTaxAmount_ShouldThrowException()
    {
        var action = () => Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, -500m, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        action.Should().Throw<ArgumentException>()
            .WithMessage("TaxAmount cannot be negative.*");
    }

    [Test]
    public void Create_WithEmptyCodeInternal_ShouldThrowException()
    {
        var action = () => Property.Create(_ownerId, "", ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        action.Should().Throw<ArgumentException>()
            .WithMessage("CodeInternal is required*");
    }

    [Test]
    public void Create_WithTooLongCodeInternal_ShouldThrowException()
    {
        var longCode = new string('A', 41);

        var action = () => Property.Create(_ownerId, longCode, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 40 characters*");
    }

    [Test]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        var action = () => Property.Create(_ownerId, ValidCodeInternal, "", PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Name is required*");
    }

    [Test]
    public void Create_WithTooLongName_ShouldThrowException()
    {
        var longName = new string('A', 201);

        var action = () => Property.Create(_ownerId, ValidCodeInternal, longName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        action.Should().Throw<ArgumentException>()
            .WithMessage("*cannot exceed 200 characters*");
    }

    [Test]
    public void Create_ShouldCalculatePriceCorrectly()
    {
        var basePrice = 150000m;
        var taxAmount = 7500m;

        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            basePrice, taxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.Price.Should().Be(basePrice + taxAmount);
    }

    [Test]
    public void Create_ShouldCreateInitialTrace()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.Traces.Should().HaveCount(1);
        property.Traces.First().EventType.Should().Be(TraceEventType.CREATED);
        property.Traces.First().PropertyId.Should().Be(property.Id);
    }

    [Test]
    public void ChangePrice_WithValidPrices_ShouldUpdatePrice()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newBasePrice = 120000m;
        var newTaxAmount = 6000m;

        property.ChangePrice(newBasePrice, newTaxAmount, "TestUser");

        property.BasePrice.Should().Be(newBasePrice);
        property.TaxAmount.Should().Be(newTaxAmount);
        property.Price.Should().Be(newBasePrice + newTaxAmount);
    }

    [Test]
    public void ChangePrice_ShouldCreatePriceChangeTrace()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newBasePrice = 120000m;
        var newTaxAmount = 6000m;

        property.ChangePrice(newBasePrice, newTaxAmount, "TestUser");

        property.Traces.Should().HaveCount(2);
        var priceTrace = property.Traces.Last();
        priceTrace.EventType.Should().Be(TraceEventType.PRICE_CHANGE);
        priceTrace.ActorName.Should().Be("TestUser");
        priceTrace.OldTotalPrice.Should().Be(ValidBasePrice + ValidTaxAmount);
    }

    [Test]
    public void ChangePrice_WithNegativeBasePrice_ShouldThrowException()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        var action = () => property.ChangePrice(-1000m, ValidTaxAmount);

        action.Should().Throw<ArgumentException>()
            .WithMessage("BasePrice cannot be negative.*");
    }

    [Test]
    public void ChangePrice_WithNegativeTaxAmount_ShouldThrowException()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        var action = () => property.ChangePrice(ValidBasePrice, -500m);

        action.Should().Throw<ArgumentException>()
            .WithMessage("TaxAmount cannot be negative.*");
    }

    [Test]
    public void AddImage_WithValidData_ShouldAddImage()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.AddImage("https://example.com/image1.jpg");

        property.Images.Should().HaveCount(1);
        property.Images.First().Url.Should().Be("https://example.com/image1.jpg");
        property.Images.First().IsPrimary.Should().BeTrue();
    }

    [Test]
    public void AddImage_FirstImageShouldBePrimary()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.AddImage("https://example.com/image1.jpg", isPrimary: false);

        property.Images.First().IsPrimary.Should().BeTrue();
    }

    [Test]
    public void AddImage_WithPrimaryTrue_ShouldMakeOnlyOnePrimary()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        
        property.AddImage("https://example.com/image1.jpg");
        property.AddImage("https://example.com/image2.jpg", isPrimary: true);

        property.Images.Should().HaveCount(2);
        property.Images.Count(i => i.IsPrimary).Should().Be(1);
        property.Images.Last().IsPrimary.Should().BeTrue();
    }

    [Test]
    public void AddImage_WithEmptyUrl_ShouldThrowException()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        var action = () => property.AddImage("");

        action.Should().Throw<ArgumentException>()
            .WithMessage("URL is required.*");
    }

    [Test]
    public void SoftDelete_ShouldSetDeletedAt()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.SoftDelete();

        property.DeletedAt.Should().NotBeNull();
        property.DeletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Test]
    public void SoftDelete_ShouldCreateDeleteTrace()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.SoftDelete();

        var deleteTrace = property.Traces.Last();
        deleteTrace.EventType.Should().Be(TraceEventType.DELETED);
        deleteTrace.Notes.Should().Be("Property soft deleted");
    }

    [Test]
    public void Update_WithValidName_ShouldUpdateName()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newName = "Updated House Name";

        property.Update(name: newName);

        property.Name.Should().Be(newName);
        property.Traces.Should().HaveCount(2);
        property.Traces.Last().EventType.Should().Be(TraceEventType.UPDATED);
    }

    [Test]
    public void Update_WithNoChanges_ShouldNotCreateTrace()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var initialTraceCount = property.Traces.Count;

        property.Update(name: ValidName);

        property.Traces.Should().HaveCount(initialTraceCount);
    }

    [Test]
    public void Update_WithDescription_ShouldUpdateDescription()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newDescription = "Updated description";

        property.Update(description: newDescription);

        property.Description.Should().Be(newDescription);
        property.Traces.Should().HaveCount(2);
        property.Traces.Last().Notes.Should().Contain("Description updated");
    }

    [Test]
    public void Update_WithNullDescription_ShouldSetToNull()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        property.Update(description: "Initial description");
        
        property.Update(description: null);

        property.Description.Should().BeNull();
    }

    [Test]
    public void Update_WithBedrooms_ShouldUpdateBedrooms()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        short newBedrooms = 4;

        property.Update(bedrooms: newBedrooms);

        property.Bedrooms.Should().Be(newBedrooms);
        property.Traces.Last().Notes.Should().Contain($"Bedrooms: 0 → {newBedrooms}");
    }

    [Test]
    public void Update_WithNegativeBedrooms_ShouldNotUpdate()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var originalBedrooms = property.Bedrooms;
        var initialTraceCount = property.Traces.Count;

        property.Update(bedrooms: -1);

        property.Bedrooms.Should().Be(originalBedrooms);
        property.Traces.Should().HaveCount(initialTraceCount);
    }

    [Test]
    public void Update_WithBathrooms_ShouldUpdateBathrooms()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        int newBathrooms = 3;

        property.Update(bathrooms: newBathrooms);

        property.Bathrooms.Should().Be(newBathrooms);
        property.Traces.Last().Notes.Should().Contain($"Bathrooms: 0 → {newBathrooms}");
    }

    [Test]
    public void Update_WithNegativeBathrooms_ShouldNotUpdate()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var originalBathrooms = property.Bathrooms;
        var initialTraceCount = property.Traces.Count;

        property.Update(bathrooms: -1);

        property.Bathrooms.Should().Be(originalBathrooms);
        property.Traces.Should().HaveCount(initialTraceCount);
    }

    [Test]
    public void Update_WithParkingSpaces_ShouldUpdateParkingSpaces()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        short newParkingSpaces = 2;

        property.Update(parkingSpaces: newParkingSpaces);

        property.ParkingSpaces.Should().Be(newParkingSpaces);
        property.Traces.Last().Notes.Should().Contain($"ParkingSpaces: 0 → {newParkingSpaces}");
    }

    [Test]
    public void Update_WithNegativeParkingSpaces_ShouldNotUpdate()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var originalSpaces = property.ParkingSpaces;
        var initialTraceCount = property.Traces.Count;

        property.Update(parkingSpaces: -1);

        property.ParkingSpaces.Should().Be(originalSpaces);
        property.Traces.Should().HaveCount(initialTraceCount);
    }

    [Test]
    public void Update_WithAreaSqft_ShouldUpdateAreaSqft()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        int newArea = 2500;

        property.Update(areaSqft: newArea);

        property.AreaSqft.Should().Be(newArea);
        property.Traces.Last().Notes.Should().Contain($"AreaSqft:  → {newArea}");
    }

    [Test]
    public void Update_WithYearBuilt_ShouldUpdateYearBuilt()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        short newYear = 2023;

        property.Update(yearBuilt: newYear);

        property.YearBuilt.Should().Be(newYear);
        property.Traces.Last().Notes.Should().Contain($"YearBuilt:  → {newYear}");
    }

    [Test]
    public void Update_WithAddressLine_ShouldUpdateAddress()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newAddress = "456 Oak Street";

        property.Update(addressLine: newAddress);

        property.AddressLine.Should().Be(newAddress);
        property.Traces.Last().Notes.Should().Contain("AddressLine updated");
    }

    [Test]
    public void Update_WithCity_ShouldUpdateCity()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newCity = "Orlando";

        property.Update(city: newCity);

        property.City.Should().Be(newCity);
        property.Traces.Last().Notes.Should().Contain($"City: '{ValidCity}' → '{newCity}'");
    }

    [Test]
    public void Update_WithState_ShouldUpdateState()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newState = "CA";

        property.Update(state: newState);

        property.State.Should().Be(newState);
        property.Traces.Last().Notes.Should().Contain($"State: '{ValidState}' → '{newState}'");
    }

    [Test]
    public void Update_WithPostalCode_ShouldUpdatePostalCode()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newPostalCode = "90210";

        property.Update(postalCode: newPostalCode);

        property.PostalCode.Should().Be(newPostalCode);
        property.Traces.Last().Notes.Should().Contain($"PostalCode: '{ValidPostalCode}' → '{newPostalCode}'");
    }

    [Test]
    public void Update_WithCoordinates_ShouldUpdateLatLng()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        decimal newLat = 25.7617m;
        decimal newLng = -80.1918m;

        property.Update(lat: newLat, lng: newLng);

        property.Lat.Should().Be(newLat);
        property.Lng.Should().Be(newLng);
        property.Traces.Last().Notes.Should().Contain($"Latitude:  → {newLat}");
        property.Traces.Last().Notes.Should().Contain($"Longitude:  → {newLng}");
    }

    [Test]
    public void Update_WithIsFeatured_ShouldUpdateFeaturedFlag()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.Update(isFeatured: true);

        property.IsFeatured.Should().BeTrue();
        property.Traces.Last().Notes.Should().Contain("Featured: False → True");
    }

    [Test]
    public void Update_WithIsPublished_ShouldUpdatePublishedFlag()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.Update(isPublished: false);

        property.IsPublished.Should().BeFalse();
        property.Traces.Last().Notes.Should().Contain("Published: True → False");
    }

    [Test]
    public void Update_WithPricesInUpdateMethod_ShouldUpdatePricesAndCalculateTotal()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var newBasePrice = 150000m;
        var newTaxAmount = 7500m;

        property.Update(basePrice: newBasePrice, taxAmount: newTaxAmount);

        property.BasePrice.Should().Be(newBasePrice);
        property.TaxAmount.Should().Be(newTaxAmount);
        property.Price.Should().Be(newBasePrice + newTaxAmount);
        property.Traces.Last().Notes.Should().Contain($"Price: {ValidBasePrice + ValidTaxAmount:C} → {newBasePrice + newTaxAmount:C}");
    }

    [Test]
    public void Update_WithNegativeBasePrice_ShouldNotUpdatePrice()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var originalBasePrice = property.BasePrice;
        var originalPrice = property.Price;

        property.Update(basePrice: -1000m);

        property.BasePrice.Should().Be(originalBasePrice);
        property.Price.Should().Be(originalPrice);
        property.Traces.Should().HaveCount(2);
        property.Traces.Last().Notes.Should().Contain("Price:");
    }

    [Test]
    public void Update_WithNegativeTaxAmount_ShouldNotUpdateTax()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var originalTaxAmount = property.TaxAmount;
        var originalPrice = property.Price;

        property.Update(taxAmount: -500m);

        property.TaxAmount.Should().Be(originalTaxAmount);
        property.Price.Should().Be(originalPrice);
        property.Traces.Should().HaveCount(2);
        property.Traces.Last().Notes.Should().Contain("Price:");
    }

    [Test]
    public void Update_WithMultipleFields_ShouldUpdateAllFields()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);

        property.Update(
            name: "Updated Name",
            description: "Updated Description",
            bedrooms: 4,
            bathrooms: 3,
            areaSqft: 2500,
            isFeatured: true
        );

        property.Name.Should().Be("Updated Name");
        property.Description.Should().Be("Updated Description");
        property.Bedrooms.Should().Be(4);
        property.Bathrooms.Should().Be(3);
        property.AreaSqft.Should().Be(2500);
        property.IsFeatured.Should().BeTrue();
        
        var lastTrace = property.Traces.Last();
        lastTrace.EventType.Should().Be(TraceEventType.UPDATED);
        lastTrace.Notes.Should().Contain("Name:");
        lastTrace.Notes.Should().Contain("Description updated");
        lastTrace.Notes.Should().Contain("Bedrooms:");
        lastTrace.Notes.Should().Contain("Bathrooms:");
        lastTrace.Notes.Should().Contain("AreaSqft:");
        lastTrace.Notes.Should().Contain("Featured:");
    }

    [Test]
    public void Update_ShouldUpdateTimestamp()
    {
        var property = Property.Create(_ownerId, ValidCodeInternal, ValidName, PropertyType.HOUSE,
            ValidBasePrice, ValidTaxAmount, ValidAddress, ValidCity, ValidState, ValidPostalCode);
        var originalUpdateTime = property.UpdatedAt;

        Thread.Sleep(10);
        property.Update(name: "New Name");

        property.UpdatedAt.Should().BeAfter(originalUpdateTime);
    }
}