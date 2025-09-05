using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for updating property information (PUT - complete replacement of property data)
/// </summary>
public class UpdatePropertyDto
{
    /// <summary>
    /// Property name (required, max 200 characters)
    /// </summary>
    /// <example>Luxury Oceanfront Penthouse - Updated</example>
    [Description("Property name (required, max 200 characters)")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Detailed property description (optional, max 2000 characters)
    /// </summary>
    /// <example>Stunning updated penthouse with panoramic ocean views, featuring 4 bedrooms, 3.5 bathrooms, gourmet kitchen with top-of-the-line appliances, spacious living areas, and private terrace perfect for entertaining.</example>
    [Description("Detailed property description (optional, max 2000 characters)")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Year the property was built (optional, must be between 1800-2030)
    /// </summary>
    /// <example>2019</example>
    [Description("Year the property was built (optional, must be between 1800-2030)")]
    public short? YearBuilt { get; set; }
    
    /// <summary>
    /// Number of bedrooms (required, must be 0-20)
    /// </summary>
    /// <example>4</example>
    [Description("Number of bedrooms (required, must be 0-20)")]
    public short Bedrooms { get; set; }
    
    /// <summary>
    /// Number of bathrooms (required, must be 0-20)
    /// </summary>
    /// <example>3</example>
    [Description("Number of bathrooms (required, must be 0-20)")]
    public int Bathrooms { get; set; }
    
    /// <summary>
    /// Number of parking spaces (required, must be 0-10)
    /// </summary>
    /// <example>2</example>
    [Description("Number of parking spaces (required, must be 0-10)")]
    public short ParkingSpaces { get; set; }
    
    /// <summary>
    /// Total area in square feet (optional, must be 100-50000)
    /// </summary>
    /// <example>3200</example>
    [Description("Total area in square feet (optional, must be 100-50000)")]
    public int? AreaSqft { get; set; }
    
    /// <summary>
    /// Street address (optional, max 200 characters)
    /// </summary>
    /// <example>456 Updated Ocean Drive</example>
    [Description("Street address (optional, max 200 characters)")]
    public string? AddressLine { get; set; }
    
    /// <summary>
    /// City name (optional, max 120 characters)
    /// </summary>
    /// <example>Miami Beach</example>
    [Description("City name (optional, max 120 characters)")]
    public string? City { get; set; }
    
    /// <summary>
    /// State/Province (optional, max 50 characters)
    /// </summary>
    /// <example>FL</example>
    [Description("State/Province (optional, max 50 characters)")]
    public string? State { get; set; }
    
    /// <summary>
    /// Postal/ZIP code (optional, max 10 characters)
    /// </summary>
    /// <example>33139</example>
    [Description("Postal/ZIP code (optional, max 10 characters)")]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Latitude coordinate (optional, decimal degrees -90 to 90)
    /// </summary>
    /// <example>25.7907</example>
    [Description("Latitude coordinate (optional, decimal degrees -90 to 90)")]
    public decimal? Lat { get; set; }
    
    /// <summary>
    /// Longitude coordinate (optional, decimal degrees -180 to 180)
    /// </summary>
    /// <example>-80.1300</example>
    [Description("Longitude coordinate (optional, decimal degrees -180 to 180)")]
    public decimal? Lng { get; set; }
    
    /// <summary>
    /// Property listing status (optional, valid values: Active, Pending, Sold, Off_Market, Draft)
    /// </summary>
    /// <example>Active</example>
    [Description("Property listing status (optional, valid values: Active, Pending, Sold, Off_Market, Draft)")]
    public string? ListingStatus { get; set; }
    
    /// <summary>
    /// Date when property was listed (optional, format: YYYY-MM-DD)
    /// </summary>
    /// <example>2024-02-15</example>
    [Description("Date when property was listed (optional, format: YYYY-MM-DD)")]
    public DateOnly? ListingDate { get; set; }
    
    /// <summary>
    /// Whether this property is featured/highlighted (required)
    /// </summary>
    /// <example>true</example>
    [Description("Whether this property is featured/highlighted (required)")]
    public bool IsFeatured { get; set; }
    
    /// <summary>
    /// Whether this property is published and visible to public (required)
    /// </summary>
    /// <example>true</example>
    [Description("Whether this property is published and visible to public (required)")]
    public bool IsPublished { get; set; }
    
    /// <summary>
    /// Base price before taxes (optional, must be >= 0)
    /// </summary>
    /// <example>450000.00</example>
    [Description("Base price before taxes (optional, must be >= 0)")]
    public decimal? BasePrice { get; set; }
    
    /// <summary>
    /// Tax amount to be added to base price (optional, must be >= 0)
    /// </summary>
    /// <example>45000.00</example>
    [Description("Tax amount to be added to base price (optional, must be >= 0)")]
    public decimal? TaxAmount { get; set; }
    
    /// <summary>
    /// Total price (BasePrice + TaxAmount) - calculated automatically if BasePrice and TaxAmount provided
    /// </summary>
    /// <example>495000.00</example>
    [Description("Total price (BasePrice + TaxAmount) - calculated automatically if BasePrice and TaxAmount provided")]
    public decimal? Price { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control (required)
    /// </summary>
    /// <example>1</example>
    [Description("Row version for optimistic concurrency control (required)")]
    public int RowVersion { get; set; }
}