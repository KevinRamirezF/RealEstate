using System;
using System.Collections.Generic;
using System.Linq;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }
    public Guid OwnerId { get; private set; }
    public string CodeInternal { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public PropertyType PropertyType { get; private set; }
    public short? YearBuilt { get; private set; }
    public short Bedrooms { get; private set; } = 0;
    public int Bathrooms { get; private set; } = 0;
    public short ParkingSpaces { get; private set; } = 0;
    public int? AreaSqft { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string AddressLine { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string Country { get; private set; } = "US";
    public decimal? Lat { get; private set; }
    public decimal? Lng { get; private set; }
    public ListingStatus ListingStatus { get; private set; } = ListingStatus.ACTIVE;
    public DateOnly ListingDate { get; private set; }
    public bool IsFeatured { get; private set; } = false;
    public bool IsPublished { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public int RowVersion { get; private set; } = 1;

    private readonly List<PropertyImage> _images = new();
    public IReadOnlyCollection<PropertyImage> Images => _images.AsReadOnly();

    private readonly List<PropertyTrace> _traces = new();
    public IReadOnlyCollection<PropertyTrace> Traces => _traces.AsReadOnly();

    private Property() { }

    private Property(Guid id, Guid ownerId, string codeInternal, string name, PropertyType propertyType, 
        decimal price, string addressLine, string city, string state, string postalCode)
    {
        Id = id;
        OwnerId = ownerId;
        CodeInternal = codeInternal;
        Name = name;
        PropertyType = propertyType;
        Price = price;
        AddressLine = addressLine;
        City = city;
        State = state;
        PostalCode = postalCode;
        ListingDate = DateOnly.FromDateTime(DateTime.Today);
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Property Create(Guid ownerId, string codeInternal, string name, PropertyType propertyType, 
        decimal price, string addressLine, string city, string state, string postalCode)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        if (string.IsNullOrWhiteSpace(codeInternal) || codeInternal.Length > 40)
            throw new ArgumentException("CodeInternal is required and cannot exceed 40 characters.", nameof(codeInternal));
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
            throw new ArgumentException("Name is required and cannot exceed 200 characters.", nameof(name));

        var property = new Property(Guid.NewGuid(), ownerId, codeInternal, name, propertyType, 
            price, addressLine, city, state, postalCode);

        var trace = PropertyTrace.Create(property.Id, TraceEventType.CREATED, "Initial creation");
        property._traces.Add(trace);

        return property;
    }

    public void ChangePrice(decimal newPrice, decimal? taxAmount = null, string? actorName = null)
    {
        if (newPrice < 0)
            throw new ArgumentException("New price cannot be negative.", nameof(newPrice));

        var oldPrice = Price;
        Price = newPrice;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;

        var trace = PropertyTrace.Create(Id, TraceEventType.PRICE_CHANGE, 
            $"Price changed from {oldPrice:C} to {newPrice:C}", oldPrice, newPrice, taxAmount, actorName);
        _traces.Add(trace);
    }

    public void AddImage(string url, StorageProvider storageProvider = StorageProvider.S3, 
        string? altText = null, bool isPrimary = false, short sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required.", nameof(url));

        if (isPrimary)
        {
            foreach (var existingImage in _images.Where(i => i.IsPrimary))
            {
                existingImage.SetAsPrimary(false);
            }
        }
        else if (!_images.Any())
        {
            isPrimary = true;
        }

        var image = PropertyImage.Create(Id, url, storageProvider, altText, isPrimary, sortOrder);
        _images.Add(image);
        
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }

    public void Update(string? name = null, string? description = null, short? bedrooms = null, 
        int? bathrooms = null, short? parkingSpaces = null, int? areaSqft = null)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;
        
        Description = description;
        
        if (bedrooms.HasValue && bedrooms.Value >= 0)
            Bedrooms = bedrooms.Value;
        
        if (bathrooms.HasValue && bathrooms.Value >= 0)
            Bathrooms = bathrooms.Value;
        
        if (parkingSpaces.HasValue && parkingSpaces.Value >= 0)
            ParkingSpaces = parkingSpaces.Value;
        
        AreaSqft = areaSqft;
        
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;

        var trace = PropertyTrace.Create(Id, TraceEventType.UPDATED, "Property details updated");
        _traces.Add(trace);
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }
}
