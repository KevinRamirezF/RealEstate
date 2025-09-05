using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for creating a new property listing
/// </summary>
public class CreatePropertyDto
{
    /// <summary>
    /// ID of the property owner (must be an existing owner)
    /// </summary>
    /// <example>16cbcd24-2318-4ba3-aa8a-dfaafc077a39</example>
    [Description("ID of the property owner (must be an existing owner)")]
    public Guid OwnerId { get; set; }
    
    /// <summary>
    /// Internal property code for identification (unique, alphanumeric, max 40 characters)
    /// </summary>
    /// <example>PROP001</example>
    [Description("Internal property code for identification (unique, alphanumeric, max 40 characters)")]
    public required string CodeInternal { get; set; }
    
    /// <summary>
    /// Property name/title (max 200 characters)
    /// </summary>
    /// <example>Beautiful Family Home</example>
    [Description("Property name/title (max 200 characters)")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Detailed property description (optional, max 8000 characters)
    /// </summary>
    /// <example>A spacious 3-bedroom home with modern amenities, updated kitchen, and large backyard perfect for families.</example>
    [Description("Detailed property description (optional, max 8000 characters)")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of property (HOUSE, APARTMENT, CONDO, TOWNHOUSE, LAND, COMMERCIAL, OTHER)
    /// </summary>
    /// <example>HOUSE</example>
    [Description("Type of property (HOUSE, APARTMENT, CONDO, TOWNHOUSE, LAND, COMMERCIAL, OTHER)")]
    public required string PropertyType { get; set; }
    
    /// <summary>
    /// Year the property was built (optional, between 1800-2100)
    /// </summary>
    /// <example>2020</example>
    [Description("Year the property was built (optional, between 1800-2100)")]
    public short? YearBuilt { get; set; }
    
    /// <summary>
    /// Number of bedrooms (0-50)
    /// </summary>
    /// <example>3</example>
    [Description("Number of bedrooms (0-50)")]
    public short Bedrooms { get; set; } = 0;
    
    /// <summary>
    /// Number of bathrooms (0-50)
    /// </summary>
    /// <example>2</example>
    [Description("Number of bathrooms (0-50)")]
    public int Bathrooms { get; set; } = 0;
    
    /// <summary>
    /// Number of parking spaces (0-50)
    /// </summary>
    /// <example>2</example>
    [Description("Number of parking spaces (0-50)")]
    public short ParkingSpaces { get; set; } = 0;
    
    /// <summary>
    /// Property area in square feet (optional, positive number)
    /// </summary>
    /// <example>2500</example>
    [Description("Property area in square feet (optional, positive number)")]
    public int? AreaSqft { get; set; }
    
    /// <summary>
    /// Property base price (positive decimal, max 14 digits with 2 decimals)
    /// </summary>
    /// <example>400000.00</example>
    [Description("Property base price (positive decimal, max 14 digits with 2 decimals)")]
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Property tax amount (positive decimal, max 14 digits with 2 decimals)
    /// </summary>
    /// <example>50000.00</example>
    [Description("Property tax amount (positive decimal, max 14 digits with 2 decimals)")]
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Currency code (3-character ISO code: USD, EUR, GBP, CAD, MXN, etc.)
    /// </summary>
    /// <example>USD</example>
    [Description("Currency code (3-character ISO code: USD, EUR, GBP, CAD, MXN, etc.)")]
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Street address (max 200 characters)
    /// </summary>
    /// <example>123 Main Street</example>
    [Description("Street address (max 200 characters)")]
    public required string AddressLine { get; set; }
    
    /// <summary>
    /// City name (max 120 characters)
    /// </summary>
    /// <example>Los Angeles</example>
    [Description("City name (max 120 characters)")]
    public required string City { get; set; }
    
    /// <summary>
    /// State/Province (max 50 characters)
    /// </summary>
    /// <example>CA</example>
    [Description("State/Province (max 50 characters)")]
    public required string State { get; set; }
    
    /// <summary>
    /// Postal/ZIP code (max 10 characters)
    /// </summary>
    /// <example>90210</example>
    [Description("Postal/ZIP code (max 10 characters)")]
    public required string PostalCode { get; set; }
    
    /// <summary>
    /// Country code (2-character ISO code: US, CA, MX, etc.)
    /// </summary>
    /// <example>US</example>
    [Description("Country code (2-character ISO code: US, CA, MX, etc.)")]
    public string Country { get; set; } = "US";
    
    /// <summary>
    /// Latitude coordinate (optional, -90 to 90 degrees with up to 7 decimal places)
    /// </summary>
    /// <example>34.0522</example>
    [Description("Latitude coordinate (optional, -90 to 90 degrees with up to 7 decimal places)")]
    public decimal? Lat { get; set; }
    
    /// <summary>
    /// Longitude coordinate (optional, -180 to 180 degrees with up to 7 decimal places)
    /// </summary>
    /// <example>-118.2437</example>
    [Description("Longitude coordinate (optional, -180 to 180 degrees with up to 7 decimal places)")]
    public decimal? Lng { get; set; }
    
    /// <summary>
    /// Current listing status (ACTIVE, INACTIVE, SOLD, RENTED, PENDING)
    /// </summary>
    /// <example>ACTIVE</example>
    [Description("Current listing status (ACTIVE, INACTIVE, SOLD, RENTED, PENDING)")]
    public string ListingStatus { get; set; } = "ACTIVE";
    
    /// <summary>
    /// Date when property was listed (optional, format: YYYY-MM-DD)
    /// </summary>
    /// <example>2024-01-15</example>
    [Description("Date when property was listed (optional, format: YYYY-MM-DD)")]
    public DateOnly? ListingDate { get; set; }
    
    /// <summary>
    /// Whether this property should be featured in listings
    /// </summary>
    /// <example>false</example>
    [Description("Whether this property should be featured in listings")]
    public bool IsFeatured { get; set; } = false;
    
    /// <summary>
    /// Whether this property is published and visible to public
    /// </summary>
    /// <example>true</example>
    [Description("Whether this property is published and visible to public")]
    public bool IsPublished { get; set; } = true;
}