using System.ComponentModel;

namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// Simplified property information for list views and search results
/// </summary>
public class PropertyListDto
{
    /// <summary>
    /// Unique property identifier
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Description("Unique property identifier")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Property name
    /// </summary>
    /// <example>Luxury Oceanfront Penthouse</example>
    [Description("Property name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Internal property code for tracking
    /// </summary>
    /// <example>PROP-2024-001</example>
    [Description("Internal property code for tracking")]
    public required string CodeInternal { get; set; }
    
    /// <summary>
    /// City where the property is located
    /// </summary>
    /// <example>Miami Beach</example>
    [Description("City where the property is located")]
    public required string City { get; set; }
    
    /// <summary>
    /// State/Province where the property is located
    /// </summary>
    /// <example>FL</example>
    [Description("State/Province where the property is located")]
    public required string State { get; set; }
    
    /// <summary>
    /// Postal/ZIP code where the property is located
    /// </summary>
    /// <example>33139</example>
    [Description("Postal/ZIP code where the property is located")]
    public required string PostalCode { get; set; }
    
    /// <summary>
    /// Current property price
    /// </summary>
    /// <example>2500000.00</example>
    [Description("Current property price")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Year the property was built
    /// </summary>
    /// <example>2018</example>
    [Description("Year the property was built")]
    public short? YearBuilt { get; set; }
    
    /// <summary>
    /// Number of bedrooms
    /// </summary>
    /// <example>4</example>
    [Description("Number of bedrooms")]
    public short Bedrooms { get; set; }
    
    /// <summary>
    /// Number of bathrooms
    /// </summary>
    /// <example>3</example>
    [Description("Number of bathrooms")]
    public int Bathrooms { get; set; }
    
    /// <summary>
    /// Total area in square feet
    /// </summary>
    /// <example>3500</example>
    [Description("Total area in square feet")]
    public int? AreaSqft { get; set; }
    
    /// <summary>
    /// Current listing status (Active, Pending, Sold, Off_Market, Draft)
    /// </summary>
    /// <example>Active</example>
    [Description("Current listing status (Active, Pending, Sold, Off_Market, Draft)")]
    public required string ListingStatus { get; set; }
    
    /// <summary>
    /// URL of the primary property image (optional)
    /// </summary>
    /// <example>https://images.millionluxury.com/property-123/main-view.jpg</example>
    [Description("URL of the primary property image (optional)")]
    public string? PrimaryImageUrl { get; set; }
    
    /// <summary>
    /// Full name of the property owner
    /// </summary>
    /// <example>John Smith</example>
    [Description("Full name of the property owner")]
    public required string OwnerFullName { get; set; }
    
    /// <summary>
    /// Whether this property is featured/highlighted
    /// </summary>
    /// <example>true</example>
    [Description("Whether this property is featured/highlighted")]
    public bool IsFeatured { get; set; }
    
    /// <summary>
    /// Whether this property is published and visible to public
    /// </summary>
    /// <example>true</example>
    [Description("Whether this property is published and visible to public")]
    public bool IsPublished { get; set; }
}