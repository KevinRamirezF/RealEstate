namespace RealEstate.Application.DTOs.Output;

public class PropertyDetailDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string CodeInternal { get; set; }
    public string? Description { get; set; }
    public required string PropertyType { get; set; }
    public short? YearBuilt { get; set; }
    public short Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public short ParkingSpaces { get; set; }
    public int? AreaSqft { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public required string AddressLine { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string Country { get; set; } = "US";
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public required string ListingStatus { get; set; }
    public DateOnly ListingDate { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public required string OwnerFullName { get; set; }
    public List<PropertyImageDto> Images { get; set; } = new();
    public PropertyPriceChangeDto? LastPriceChange { get; set; }
    public int TracesCount { get; set; }
    public int RowVersion { get; set; }
}

public class PropertyImageDto
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public bool IsPrimary { get; set; }
    public short SortOrder { get; set; }
    public bool Enabled { get; set; }
    public string? AltText { get; set; }
}

public class PropertyPriceChangeDto
{
    public DateTimeOffset EventDate { get; set; }
    public decimal? OldValue { get; set; }
    public decimal? NewValue { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? ActorName { get; set; }
}