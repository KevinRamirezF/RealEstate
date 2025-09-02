namespace RealEstate.Application.DTOs.Filters;

public class PropertyFilters
{
    public string? Q { get; set; }
    public Guid? OwnerId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public short? YearFrom { get; set; }
    public short? YearTo { get; set; }
    public short? BedroomsFrom { get; set; }
    public short? BedroomsTo { get; set; }
    public decimal? BathroomsFrom { get; set; }
    public decimal? BathroomsTo { get; set; }
    public int? AreaFrom { get; set; }
    public int? AreaTo { get; set; }
    public int? LotFrom { get; set; }
    public int? LotTo { get; set; }
    public decimal? HoaFrom { get; set; }
    public decimal? HoaTo { get; set; }
    public string[]? PropertyType { get; set; }
    public string[]? ListingStatus { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsPublished { get; set; }
    public bool? HasPrimaryImage { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public decimal? LatMin { get; set; }
    public decimal? LatMax { get; set; }
    public decimal? LngMin { get; set; }
    public decimal? LngMax { get; set; }
    public string Sort { get; set; } = "name";
    public string Dir { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}