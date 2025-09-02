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
    public decimal? OldValue { get; private set; }
    public decimal? NewValue { get; private set; }
    public decimal? TaxAmount { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private PropertyTrace() { }

    private PropertyTrace(Guid id, Guid propertyId, TraceEventType eventType, string? notes = null,
        decimal? oldValue = null, decimal? newValue = null, decimal? taxAmount = null, string? actorName = null)
    {
        Id = id;
        PropertyId = propertyId;
        EventType = eventType;
        EventDate = DateTimeOffset.UtcNow;
        ActorName = actorName;
        OldValue = oldValue;
        NewValue = newValue;
        TaxAmount = taxAmount;
        Notes = notes;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static PropertyTrace Create(Guid propertyId, TraceEventType eventType, string? notes = null,
        decimal? oldValue = null, decimal? newValue = null, decimal? taxAmount = null, string? actorName = null)
    {
        return new PropertyTrace(Guid.NewGuid(), propertyId, eventType, notes, oldValue, newValue, taxAmount, actorName);
    }
}
