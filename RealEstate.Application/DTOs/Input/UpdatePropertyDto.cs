namespace RealEstate.Application.DTOs.Input;

public class UpdatePropertyDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public short? YearBuilt { get; set; }
    public short Bedrooms { get; set; }
    public decimal Bathrooms { get; set; }
    public short ParkingSpaces { get; set; }
    public int? AreaSqft { get; set; }
    public int? LotSizeSqft { get; set; }
    public decimal? HoaFee { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }
    public string? ListingStatus { get; set; }
    public DateOnly? ListingDate { get; set; }
    public decimal? LastSoldPrice { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public int RowVersion { get; set; }
}