using System.ComponentModel;

namespace RealEstate.Application.DTOs.Filters;

/// <summary>
/// Filter and pagination parameters for property search
/// </summary>
public class PropertyFilters
{
    /// <summary>
    /// General search query (searches in name, description, and address fields)
    /// </summary>
    /// <example>oceanfront penthouse</example>
    [Description("General search query (searches in name, description, and address fields)")]
    public string? Q { get; set; }
    
    /// <summary>
    /// Filter by specific owner ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Description("Filter by specific owner ID")]
    public Guid? OwnerId { get; set; }
    
    /// <summary>
    /// Minimum price filter
    /// </summary>
    /// <example>1000000</example>
    [Description("Minimum price filter")]
    public decimal? MinPrice { get; set; }
    
    /// <summary>
    /// Maximum price filter
    /// </summary>
    /// <example>5000000</example>
    [Description("Maximum price filter")]
    public decimal? MaxPrice { get; set; }
    
    /// <summary>
    /// Filter by specific year built
    /// </summary>
    /// <example>2020</example>
    [Description("Filter by specific year built")]
    public short? YearBuilt { get; set; }
    
    /// <summary>
    /// Minimum number of bedrooms
    /// </summary>
    /// <example>3</example>
    [Description("Minimum number of bedrooms")]
    public short? MinBedrooms { get; set; }
    
    /// <summary>
    /// Maximum number of bedrooms
    /// </summary>
    /// <example>6</example>
    [Description("Maximum number of bedrooms")]
    public short? MaxBedrooms { get; set; }
    
    /// <summary>
    /// Minimum number of bathrooms
    /// </summary>
    /// <example>2</example>
    [Description("Minimum number of bathrooms")]
    public int? MinBathrooms { get; set; }
    
    /// <summary>
    /// Maximum number of bathrooms
    /// </summary>
    /// <example>4</example>
    [Description("Maximum number of bathrooms")]
    public int? MaxBathrooms { get; set; }
    
    /// <summary>
    /// Minimum area in square feet
    /// </summary>
    /// <example>2000</example>
    [Description("Minimum area in square feet")]
    public int? MinAreaSqft { get; set; }
    
    /// <summary>
    /// Maximum area in square feet
    /// </summary>
    /// <example>6000</example>
    [Description("Maximum area in square feet")]
    public int? MaxAreaSqft { get; set; }
    
    /// <summary>
    /// Filter by property types (multiple values allowed)
    /// </summary>
    /// <example>["Apartment", "Condo"]</example>
    [Description("Filter by property types (multiple values allowed)")]
    public string[]? PropertyType { get; set; }
    
    /// <summary>
    /// Filter by listing statuses (multiple values allowed)
    /// </summary>
    /// <example>["Active", "Pending"]</example>
    [Description("Filter by listing statuses (multiple values allowed)")]
    public string[]? ListingStatus { get; set; }
    
    /// <summary>
    /// Filter by featured status
    /// </summary>
    /// <example>true</example>
    [Description("Filter by featured status")]
    public bool? IsFeatured { get; set; }
    
    /// <summary>
    /// Filter by published status
    /// </summary>
    /// <example>true</example>
    [Description("Filter by published status")]
    public bool? IsPublished { get; set; }
    
    /// <summary>
    /// Filter by state/province
    /// </summary>
    /// <example>FL</example>
    [Description("Filter by state/province")]
    public string? State { get; set; }
    
    /// <summary>
    /// Filter by city
    /// </summary>
    /// <example>Miami Beach</example>
    [Description("Filter by city")]
    public string? City { get; set; }
    
    /// <summary>
    /// Filter by postal/ZIP code
    /// </summary>
    /// <example>33139</example>
    [Description("Filter by postal/ZIP code")]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Minimum latitude for geographic bounding box
    /// </summary>
    /// <example>25.7500</example>
    [Description("Minimum latitude for geographic bounding box")]
    public decimal? LatMin { get; set; }
    
    /// <summary>
    /// Maximum latitude for geographic bounding box
    /// </summary>
    /// <example>25.8000</example>
    [Description("Maximum latitude for geographic bounding box")]
    public decimal? LatMax { get; set; }
    
    /// <summary>
    /// Minimum longitude for geographic bounding box
    /// </summary>
    /// <example>-80.1500</example>
    [Description("Minimum longitude for geographic bounding box")]
    public decimal? LngMin { get; set; }
    
    /// <summary>
    /// Maximum longitude for geographic bounding box
    /// </summary>
    /// <example>-80.1200</example>
    [Description("Maximum longitude for geographic bounding box")]
    public decimal? LngMax { get; set; }
    
    /// <summary>
    /// Sort field (name, price, yearBuilt, bedrooms, bathrooms, areaSquareFeet, listingDate)
    /// </summary>
    /// <example>price</example>
    [Description("Sort field (name, price, yearBuilt, bedrooms, bathrooms, areaSquareFeet, listingDate)")]
    public string Sort { get; set; } = "name";
    
    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    /// <example>desc</example>
    [Description("Sort direction (asc or desc)")]
    public string Dir { get; set; } = "asc";
    
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    /// <example>1</example>
    [Description("Page number (1-based)")]
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// Number of items per page (1-100)
    /// </summary>
    /// <example>20</example>
    [Description("Number of items per page (1-100)")]
    public int PageSize { get; set; } = 20;
}