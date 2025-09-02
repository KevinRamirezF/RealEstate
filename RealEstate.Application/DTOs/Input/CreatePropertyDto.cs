namespace RealEstate.Application.DTOs.Input;

public class CreatePropertyDto
{
    public Guid OwnerId { get; set; }
    public required string CodeInternal { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string PropertyType { get; set; }
    public short? YearBuilt { get; set; }
    public short Bedrooms { get; set; } = 0;
    public decimal Bathrooms { get; set; } = 0.0m;
    public short ParkingSpaces { get; set; } = 0;
    public int? AreaSqft { get; set; }
    public int? LotSizeSqft { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal? HoaFee { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public string Country { get; set; } = "US";
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public string ListingStatus { get; set; } = "ACTIVE";
    public DateOnly? ListingDate { get; set; }
    public decimal? LastSoldPrice { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsPublished { get; set; } = true;
}