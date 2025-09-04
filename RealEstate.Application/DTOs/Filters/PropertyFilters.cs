namespace RealEstate.Application.DTOs.Filters;

public class PropertyFilters
{
    public string? Q { get; set; }
    public Guid? OwnerId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public short? YearBuilt { get; set; }
    public short? MinBedrooms { get; set; }
    public short? MaxBedrooms { get; set; }
    public int? MinBathrooms { get; set; }
    public int? MaxBathrooms { get; set; }
    public int? MinAreaSqft { get; set; }
    public int? MaxAreaSqft { get; set; }
    public string[]? PropertyType { get; set; }
    public string[]? ListingStatus { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsPublished { get; set; }
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