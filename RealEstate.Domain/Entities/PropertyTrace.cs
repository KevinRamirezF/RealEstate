using System;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class PropertyTrace
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public TraceEventType EventType { get; private set; }
    public DateTimeOffset EventDate { get; private set; }
    public string? ActorName { get; private set; }
    public decimal? OldTotalPrice { get; private set; }
    public decimal? OldPriceBase { get; private set; }
    public decimal? OldTaxAmount { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PropertyTrace() { }

    private PropertyTrace(Guid id, Guid propertyId, TraceEventType eventType, string? notes = null,
        decimal? oldTotalPrice = null, decimal? oldPriceBase = null, decimal? oldTaxAmount = null, string? actorName = null)
    {
        Id = id;
        PropertyId = propertyId;
        EventType = eventType;
        EventDate = DateTimeOffset.UtcNow;
        ActorName = actorName;
        OldTotalPrice = oldTotalPrice;
        OldPriceBase = oldPriceBase;
        OldTaxAmount = oldTaxAmount;
        Notes = notes;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static PropertyTrace Create(Guid propertyId, TraceEventType eventType, string? notes = null,
        decimal? oldTotalPrice = null, decimal? oldPriceBase = null, decimal? oldTaxAmount = null, string? actorName = null)
    {
        return new PropertyTrace(Guid.NewGuid(), propertyId, eventType, notes, oldTotalPrice, oldPriceBase, oldTaxAmount, actorName);
    }
}
