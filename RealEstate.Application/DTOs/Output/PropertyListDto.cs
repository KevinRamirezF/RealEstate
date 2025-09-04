namespace RealEstate.Application.DTOs.Output;

public class PropertyListDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string CodeInternal { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string PostalCode { get; set; }
    public decimal Price { get; set; }
    public short? YearBuilt { get; set; }
    public short Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int? AreaSqft { get; set; }
    public required string ListingStatus { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public required string OwnerFullName { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
}