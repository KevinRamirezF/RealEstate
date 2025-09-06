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
    public decimal BasePrice { get; private set; }
    public decimal TaxAmount { get; private set; }
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
        decimal basePrice, decimal taxAmount, string addressLine, string city, string state, string postalCode)
    {
        Id = id;
        OwnerId = ownerId;
        CodeInternal = codeInternal;
        Name = name;
        PropertyType = propertyType;
        BasePrice = basePrice;
        TaxAmount = taxAmount;
        Price = basePrice + taxAmount;
        AddressLine = addressLine;
        City = city;
        State = state;
        PostalCode = postalCode;
        ListingDate = DateOnly.FromDateTime(DateTime.Today);
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Property Create(Guid ownerId, string codeInternal, string name, PropertyType propertyType, 
        decimal basePrice, decimal taxAmount, string addressLine, string city, string state, string postalCode)
    {
        if (basePrice < 0)
            throw new ArgumentException("BasePrice cannot be negative.", nameof(basePrice));
        if (taxAmount < 0)
            throw new ArgumentException("TaxAmount cannot be negative.", nameof(taxAmount));
        if (string.IsNullOrWhiteSpace(codeInternal) || codeInternal.Length > 40)
            throw new ArgumentException("CodeInternal is required and cannot exceed 40 characters.", nameof(codeInternal));
        if (string.IsNullOrWhiteSpace(name) || name.Length > 200)
            throw new ArgumentException("Name is required and cannot exceed 200 characters.", nameof(name));

        var property = new Property(Guid.NewGuid(), ownerId, codeInternal, name, propertyType, 
            basePrice, taxAmount, addressLine, city, state, postalCode);

        var trace = PropertyTrace.Create(property.Id, TraceEventType.CREATED, "Initial creation");
        property._traces.Add(trace);

        return property;
    }

    public void ChangePrice(decimal newBasePrice, decimal newTaxAmount, string? actorName = null)
    {
        if (newBasePrice < 0)
            throw new ArgumentException("BasePrice cannot be negative.", nameof(newBasePrice));
        if (newTaxAmount < 0)
            throw new ArgumentException("TaxAmount cannot be negative.", nameof(newTaxAmount));

        var oldBasePrice = BasePrice;
        var oldTaxAmount = TaxAmount;
        var oldPrice = Price;
        
        BasePrice = newBasePrice;
        TaxAmount = newTaxAmount;
        Price = newBasePrice + newTaxAmount;
        
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;

        var trace = PropertyTrace.Create(Id, TraceEventType.PRICE_CHANGE, 
            $"Price changed from {oldPrice:C} to {Price:C} (Base: {oldBasePrice:C}→{newBasePrice:C}, Tax: {oldTaxAmount:C}→{newTaxAmount:C})", 
            oldPrice, oldBasePrice, oldTaxAmount, actorName);
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
        int? bathrooms = null, short? parkingSpaces = null, int? areaSqft = null, 
        decimal? basePrice = null, decimal? taxAmount = null, short? yearBuilt = null,
        string? addressLine = null, string? city = null, string? state = null, string? postalCode = null,
        decimal? lat = null, decimal? lng = null, bool? isFeatured = null, bool? isPublished = null)
    {
        var changes = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(name) && name != Name)
        {
            changes.Add($"Name: '{Name}' → '{name}'");
            Name = name;
        }
        
        if (description != Description)
        {
            changes.Add($"Description updated");
            Description = description;
        }
        
        if (bedrooms.HasValue && bedrooms.Value >= 0 && bedrooms.Value != Bedrooms)
        {
            changes.Add($"Bedrooms: {Bedrooms} → {bedrooms.Value}");
            Bedrooms = bedrooms.Value;
        }
        
        if (bathrooms.HasValue && bathrooms.Value >= 0 && bathrooms.Value != Bathrooms)
        {
            changes.Add($"Bathrooms: {Bathrooms} → {bathrooms.Value}");
            Bathrooms = bathrooms.Value;
        }
        
        if (parkingSpaces.HasValue && parkingSpaces.Value >= 0 && parkingSpaces.Value != ParkingSpaces)
        {
            changes.Add($"ParkingSpaces: {ParkingSpaces} → {parkingSpaces.Value}");
            ParkingSpaces = parkingSpaces.Value;
        }
        
        if (areaSqft != AreaSqft)
        {
            changes.Add($"AreaSqft: {AreaSqft} → {areaSqft}");
            AreaSqft = areaSqft;
        }
        
        if (yearBuilt != YearBuilt)
        {
            changes.Add($"YearBuilt: {YearBuilt} → {yearBuilt}");
            YearBuilt = yearBuilt;
        }
        
        if (!string.IsNullOrWhiteSpace(addressLine) && addressLine != AddressLine)
        {
            changes.Add($"AddressLine updated");
            AddressLine = addressLine;
        }
        
        if (!string.IsNullOrWhiteSpace(city) && city != City)
        {
            changes.Add($"City: '{City}' → '{city}'");
            City = city;
        }
        
        if (!string.IsNullOrWhiteSpace(state) && state != State)
        {
            changes.Add($"State: '{State}' → '{state}'");
            State = state;
        }
        
        if (!string.IsNullOrWhiteSpace(postalCode) && postalCode != PostalCode)
        {
            changes.Add($"PostalCode: '{PostalCode}' → '{postalCode}'");
            PostalCode = postalCode;
        }
        
        if (lat != Lat)
        {
            changes.Add($"Latitude: {Lat} → {lat}");
            Lat = lat;
        }
        
        if (lng != Lng)
        {
            changes.Add($"Longitude: {Lng} → {lng}");
            Lng = lng;
        }
        
        if (isFeatured.HasValue && isFeatured.Value != IsFeatured)
        {
            changes.Add($"Featured: {IsFeatured} → {isFeatured.Value}");
            IsFeatured = isFeatured.Value;
        }
        
        if (isPublished.HasValue && isPublished.Value != IsPublished)
        {
            changes.Add($"Published: {IsPublished} → {isPublished.Value}");
            IsPublished = isPublished.Value;
        }

        // Handle price changes (but don't create PRICE_CHANGE trace in Update method)
        if ((basePrice.HasValue && basePrice.Value != BasePrice) || 
            (taxAmount.HasValue && taxAmount.Value != TaxAmount))
        {
            var oldBasePrice = BasePrice;
            var oldTaxAmount = TaxAmount;
            var oldTotalPrice = Price;
            
            if (basePrice.HasValue && basePrice.Value >= 0)
                BasePrice = basePrice.Value;
            if (taxAmount.HasValue && taxAmount.Value >= 0)
                TaxAmount = taxAmount.Value;
                
            Price = BasePrice + TaxAmount;
            
            changes.Add($"Price: {oldTotalPrice:C} → {Price:C} (Base: {oldBasePrice:C}→{BasePrice:C}, Tax: {oldTaxAmount:C}→{TaxAmount:C})");
        }
        
        // Always create UPDATED trace if there are any changes
        if (changes.Any())
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            RowVersion++;

            var trace = PropertyTrace.Create(Id, TraceEventType.UPDATED, 
                $"Property updated: {string.Join(", ", changes)}");
            _traces.Add(trace);
        }
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;

        var trace = PropertyTrace.Create(Id, TraceEventType.DELETED, "Property soft deleted");
        _traces.Add(trace);
    }
}
