using System;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class PropertyImage
{
    public Guid Id { get; private set; }
    public Guid PropertyId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public StorageProvider StorageProvider { get; private set; } = StorageProvider.S3;
    public string? AltText { get; private set; }
    public bool IsPrimary { get; private set; } = false;
    public short SortOrder { get; private set; } = 0;
    public bool Enabled { get; private set; } = true;
    public string? Checksum { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public int RowVersion { get; private set; } = 1;

    private PropertyImage() { }

    private PropertyImage(Guid id, Guid propertyId, string url, StorageProvider storageProvider, 
        string? altText, bool isPrimary, short sortOrder)
    {
        Id = id;
        PropertyId = propertyId;
        Url = url;
        StorageProvider = storageProvider;
        AltText = altText;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static PropertyImage Create(Guid propertyId, string url, StorageProvider storageProvider = StorageProvider.S3, 
        string? altText = null, bool isPrimary = false, short sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is required.", nameof(url));
        
        if (url.Length > 1000)
            throw new ArgumentException("URL cannot exceed 1000 characters.", nameof(url));

        return new PropertyImage(Guid.NewGuid(), propertyId, url, storageProvider, altText, isPrimary, sortOrder);
    }

    public void SetAsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }

    public void UpdateSortOrder(short sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }

    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }
}
