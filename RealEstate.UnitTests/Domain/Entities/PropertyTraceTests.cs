using FluentAssertions;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.UnitTests.Domain.Entities;

[TestFixture]
public class PropertyTraceTests
{
    private Guid _propertyId;
    private const string ValidNotes = "Test notes";
    private const string ValidActorName = "TestUser";
    private const decimal OldPrice = 100000m;
    private const decimal OldBasePrice = 95000m;
    private const decimal OldTaxAmount = 5000m;

    [SetUp]
    public void Setup()
    {
        _propertyId = Guid.NewGuid();
    }

    [Test]
    public void Create_WithMinimalData_ShouldCreatePropertyTrace()
    {
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.CREATED);

        trace.Should().NotBeNull();
        trace.Id.Should().NotBeEmpty();
        trace.PropertyId.Should().Be(_propertyId);
        trace.EventType.Should().Be(TraceEventType.CREATED);
        trace.EventDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        trace.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        trace.ActorName.Should().BeNull();
        trace.OldTotalPrice.Should().BeNull();
        trace.OldPriceBase.Should().BeNull();
        trace.OldTaxAmount.Should().BeNull();
        trace.Notes.Should().BeNull();
    }

    [Test]
    public void Create_WithAllData_ShouldCreatePropertyTrace()
    {
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.PRICE_CHANGE, ValidNotes, 
            OldPrice, OldBasePrice, OldTaxAmount, ValidActorName);

        trace.PropertyId.Should().Be(_propertyId);
        trace.EventType.Should().Be(TraceEventType.PRICE_CHANGE);
        trace.Notes.Should().Be(ValidNotes);
        trace.ActorName.Should().Be(ValidActorName);
        trace.OldTotalPrice.Should().Be(OldPrice);
        trace.OldPriceBase.Should().Be(OldBasePrice);
        trace.OldTaxAmount.Should().Be(OldTaxAmount);
    }

    [Test]
    public void Create_WithNotesOnly_ShouldCreatePropertyTrace()
    {
        var notes = "Property was updated with new information";

        var trace = PropertyTrace.Create(_propertyId, TraceEventType.UPDATED, notes);

        trace.EventType.Should().Be(TraceEventType.UPDATED);
        trace.Notes.Should().Be(notes);
        trace.ActorName.Should().BeNull();
        trace.OldTotalPrice.Should().BeNull();
    }

    [Test]
    public void Create_WithActorNameOnly_ShouldCreatePropertyTrace()
    {
        var actorName = "AdminUser";

        var trace = PropertyTrace.Create(_propertyId, TraceEventType.DELETED, actorName: actorName);

        trace.EventType.Should().Be(TraceEventType.DELETED);
        trace.ActorName.Should().Be(actorName);
        trace.Notes.Should().BeNull();
        trace.OldTotalPrice.Should().BeNull();
    }

    [Test]
    public void Create_WithPriceChangeData_ShouldCreateTraceWithOldPrices()
    {
        var oldTotal = 150000m;
        var oldBase = 140000m;
        var oldTax = 10000m;

        var trace = PropertyTrace.Create(_propertyId, TraceEventType.PRICE_CHANGE, 
            oldTotalPrice: oldTotal, oldPriceBase: oldBase, oldTaxAmount: oldTax);

        trace.EventType.Should().Be(TraceEventType.PRICE_CHANGE);
        trace.OldTotalPrice.Should().Be(oldTotal);
        trace.OldPriceBase.Should().Be(oldBase);
        trace.OldTaxAmount.Should().Be(oldTax);
    }

    [Test]
    public void Create_ForDifferentEventTypes_ShouldCreateCorrectly()
    {
        var createdTrace = PropertyTrace.Create(_propertyId, TraceEventType.CREATED);
        var updatedTrace = PropertyTrace.Create(_propertyId, TraceEventType.UPDATED);
        var deletedTrace = PropertyTrace.Create(_propertyId, TraceEventType.DELETED);
        var priceTrace = PropertyTrace.Create(_propertyId, TraceEventType.PRICE_CHANGE);
        var listedTrace = PropertyTrace.Create(_propertyId, TraceEventType.LISTED);
        var soldTrace = PropertyTrace.Create(_propertyId, TraceEventType.SOLD);
        var taxTrace = PropertyTrace.Create(_propertyId, TraceEventType.TAX_UPDATE);
        var noteTrace = PropertyTrace.Create(_propertyId, TraceEventType.NOTE);

        createdTrace.EventType.Should().Be(TraceEventType.CREATED);
        updatedTrace.EventType.Should().Be(TraceEventType.UPDATED);
        deletedTrace.EventType.Should().Be(TraceEventType.DELETED);
        priceTrace.EventType.Should().Be(TraceEventType.PRICE_CHANGE);
        listedTrace.EventType.Should().Be(TraceEventType.LISTED);
        soldTrace.EventType.Should().Be(TraceEventType.SOLD);
        taxTrace.EventType.Should().Be(TraceEventType.TAX_UPDATE);
        noteTrace.EventType.Should().Be(TraceEventType.NOTE);
    }

    [Test]
    public void EventDate_ShouldBeSetToCurrentTime()
    {
        var beforeCreate = DateTimeOffset.UtcNow;
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.CREATED);
        var afterCreate = DateTimeOffset.UtcNow;

        trace.EventDate.Should().BeOnOrAfter(beforeCreate);
        trace.EventDate.Should().BeOnOrBefore(afterCreate);
    }

    [Test]
    public void CreatedAt_ShouldBeSetToCurrentTime()
    {
        var beforeCreate = DateTimeOffset.UtcNow;
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.CREATED);
        var afterCreate = DateTimeOffset.UtcNow;

        trace.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        trace.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Test]
    public void Create_WithZeroPrices_ShouldCreateCorrectly()
    {
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.PRICE_CHANGE, 
            oldTotalPrice: 0m, oldPriceBase: 0m, oldTaxAmount: 0m);

        trace.OldTotalPrice.Should().Be(0m);
        trace.OldPriceBase.Should().Be(0m);
        trace.OldTaxAmount.Should().Be(0m);
    }

    [Test]
    public void Create_WithNegativePrices_ShouldCreateCorrectly()
    {
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.PRICE_CHANGE, 
            oldTotalPrice: -100m, oldPriceBase: -50m, oldTaxAmount: -50m);

        trace.OldTotalPrice.Should().Be(-100m);
        trace.OldPriceBase.Should().Be(-50m);
        trace.OldTaxAmount.Should().Be(-50m);
    }

    [Test]
    public void Create_WithEmptyActorName_ShouldCreateCorrectly()
    {
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.UPDATED, actorName: "");

        trace.ActorName.Should().Be("");
    }

    [Test]
    public void Create_WithEmptyNotes_ShouldCreateCorrectly()
    {
        var trace = PropertyTrace.Create(_propertyId, TraceEventType.UPDATED, "");

        trace.Notes.Should().Be("");
    }

    [Test]
    public void Create_ShouldGenerateUniqueIds()
    {
        var trace1 = PropertyTrace.Create(_propertyId, TraceEventType.CREATED);
        var trace2 = PropertyTrace.Create(_propertyId, TraceEventType.CREATED);

        trace1.Id.Should().NotBe(trace2.Id);
    }
}