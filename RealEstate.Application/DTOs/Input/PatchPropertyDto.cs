using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for partially updating property information (all fields optional)
/// </summary>
public class PatchPropertyDto
{
    /// <summary>
    /// Property name/title (optional, max 200 characters)
    /// </summary>
    /// <example>Updated Property Name</example>
    [Description("Property name/title (optional, max 200 characters)")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Detailed property description (optional, max 8000 characters)
    /// </summary>
    /// <example>Updated description with new amenities and features.</example>
    [Description("Detailed property description (optional, max 8000 characters)")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Year the property was built (optional, between 1800-2100)
    /// </summary>
    /// <example>2022</example>
    [Description("Year the property was built (optional, between 1800-2100)")]
    public short? YearBuilt { get; set; }
    
    /// <summary>
    /// Number of bedrooms (optional, 0-50)
    /// </summary>
    /// <example>4</example>
    [Description("Number of bedrooms (optional, 0-50)")]
    public short? Bedrooms { get; set; }
    
    /// <summary>
    /// Number of bathrooms (optional, 0-50)
    /// </summary>
    /// <example>3</example>
    [Description("Number of bathrooms (optional, 0-50)")]
    public int? Bathrooms { get; set; }
    
    /// <summary>
    /// Number of parking spaces (optional, 0-50)
    /// </summary>
    /// <example>1</example>
    [Description("Number of parking spaces (optional, 0-50)")]
    public short? ParkingSpaces { get; set; }
    
    /// <summary>
    /// Property area in square feet (optional, positive number)
    /// </summary>
    /// <example>2800</example>
    [Description("Property area in square feet (optional, positive number)")]
    public int? AreaSqft { get; set; }
    
    /// <summary>
    /// Street address (optional, max 200 characters)
    /// </summary>
    /// <example>789 Updated Street</example>
    [Description("Street address (optional, max 200 characters)")]
    public string? AddressLine { get; set; }
    
    /// <summary>
    /// City name (optional, max 120 characters)
    /// </summary>
    /// <example>San Francisco</example>
    [Description("City name (optional, max 120 characters)")]
    public string? City { get; set; }
    
    /// <summary>
    /// State/Province (optional, max 50 characters)
    /// </summary>
    /// <example>CA</example>
    [Description("State/Province (optional, max 50 characters)")]
    public string? State { get; set; }
    
    /// <summary>
    /// Postal/ZIP code (optional, max 10 characters)
    /// </summary>
    /// <example>94102</example>
    [Description("Postal/ZIP code (optional, max 10 characters)")]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Latitude coordinate (optional, -90 to 90 degrees with up to 7 decimal places)
    /// </summary>
    /// <example>37.7749</example>
    [Description("Latitude coordinate (optional, -90 to 90 degrees with up to 7 decimal places)")]
    public decimal? Lat { get; set; }
    
    /// <summary>
    /// Longitude coordinate (optional, -180 to 180 degrees with up to 7 decimal places)
    /// </summary>
    /// <example>-122.4194</example>
    [Description("Longitude coordinate (optional, -180 to 180 degrees with up to 7 decimal places)")]
    public decimal? Lng { get; set; }
    
    /// <summary>
    /// Current listing status (optional: ACTIVE, INACTIVE, SOLD, RENTED, PENDING)
    /// </summary>
    /// <example>SOLD</example>
    [Description("Current listing status (optional: ACTIVE, INACTIVE, SOLD, RENTED, PENDING)")]
    public string? ListingStatus { get; set; }
    
    /// <summary>
    /// Date when property was listed (optional, format: YYYY-MM-DD)
    /// </summary>
    /// <example>2024-03-20</example>
    [Description("Date when property was listed (optional, format: YYYY-MM-DD)")]
    public DateOnly? ListingDate { get; set; }
    
    /// <summary>
    /// Whether this property should be featured in listings (optional)
    /// </summary>
    /// <example>true</example>
    [Description("Whether this property should be featured in listings (optional)")]
    public bool? IsFeatured { get; set; }
    
    /// <summary>
    /// Whether this property is published and visible to public (optional)
    /// </summary>
    /// <example>false</example>
    [Description("Whether this property is published and visible to public (optional)")]
    public bool? IsPublished { get; set; }
    
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
    /// Row version for optimistic concurrency (required for updates)
    /// </summary>
    /// <example>AAAAAAAAB9E=</example>
    [Description("Row version for optimistic concurrency (required for updates)")]
    public byte[]? RowVersion { get; set; }
}