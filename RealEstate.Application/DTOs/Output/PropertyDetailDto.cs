using System.ComponentModel;

namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// Complete property details with all information including owner, images, and price history
/// </summary>
public class PropertyDetailDto
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
    /// Detailed property description
    /// </summary>
    /// <example>Stunning penthouse with panoramic ocean views, featuring 4 bedrooms, 3.5 bathrooms, gourmet kitchen, and private terrace.</example>
    [Description("Detailed property description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of property (Apartment, House, Condo, Townhouse, Villa)
    /// </summary>
    /// <example>Apartment</example>
    [Description("Type of property (Apartment, House, Condo, Townhouse, Villa)")]
    public required string PropertyType { get; set; }
    
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
    /// Number of parking spaces
    /// </summary>
    /// <example>2</example>
    [Description("Number of parking spaces")]
    public short ParkingSpaces { get; set; }
    
    /// <summary>
    /// Total area in square feet
    /// </summary>
    /// <example>3500</example>
    [Description("Total area in square feet")]
    public int? AreaSqft { get; set; }
    
    /// <summary>
    /// Current property price
    /// </summary>
    /// <example>2500000.00</example>
    [Description("Current property price")]
    public decimal Price { get; set; }
    
    /// <summary>
    /// Currency of the price
    /// </summary>
    /// <example>USD</example>
    [Description("Currency of the price")]
    public string Currency { get; set; } = "USD";
    
    /// <summary>
    /// Street address
    /// </summary>
    /// <example>123 Ocean Drive, Apt 2001</example>
    [Description("Street address")]
    public required string AddressLine { get; set; }
    
    /// <summary>
    /// City name
    /// </summary>
    /// <example>Miami Beach</example>
    [Description("City name")]
    public required string City { get; set; }
    
    /// <summary>
    /// State or province
    /// </summary>
    /// <example>FL</example>
    [Description("State or province")]
    public required string State { get; set; }
    
    /// <summary>
    /// Postal or ZIP code
    /// </summary>
    /// <example>33139</example>
    [Description("Postal or ZIP code")]
    public required string PostalCode { get; set; }
    
    /// <summary>
    /// Country code
    /// </summary>
    /// <example>US</example>
    [Description("Country code")]
    public string Country { get; set; } = "US";
    
    /// <summary>
    /// Latitude coordinate
    /// </summary>
    /// <example>25.7907</example>
    [Description("Latitude coordinate")]
    public decimal? Lat { get; set; }
    
    /// <summary>
    /// Longitude coordinate
    /// </summary>
    /// <example>-80.1300</example>
    [Description("Longitude coordinate")]
    public decimal? Lng { get; set; }
    
    /// <summary>
    /// Current listing status (Active, Pending, Sold, Off_Market, Draft)
    /// </summary>
    /// <example>Active</example>
    [Description("Current listing status (Active, Pending, Sold, Off_Market, Draft)")]
    public required string ListingStatus { get; set; }
    
    /// <summary>
    /// Date when property was first listed
    /// </summary>
    /// <example>2024-01-15</example>
    [Description("Date when property was first listed")]
    public DateOnly ListingDate { get; set; }
    
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
    
    /// <summary>
    /// Full name of the property owner
    /// </summary>
    /// <example>John Smith</example>
    [Description("Full name of the property owner")]
    public required string OwnerFullName { get; set; }
    
    /// <summary>
    /// List of property images
    /// </summary>
    [Description("List of property images")]
    public List<PropertyImageDto> Images { get; set; } = new();
    
    /// <summary>
    /// Most recent price change information
    /// </summary>
    [Description("Most recent price change information")]
    public PropertyPriceChangeDto? LastPriceChange { get; set; }
    
    /// <summary>
    /// Total number of property traces/changes recorded
    /// </summary>
    /// <example>5</example>
    [Description("Total number of property traces/changes recorded")]
    public int TracesCount { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    /// <example>1</example>
    [Description("Row version for optimistic concurrency control")]
    public int RowVersion { get; set; }
}

/// <summary>
/// Property image information
/// </summary>
public class PropertyImageDto
{
    /// <summary>
    /// Unique image identifier
    /// </summary>
    /// <example>7fa85f64-5717-4562-b3fc-2c963f66afa7</example>
    [Description("Unique image identifier")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Image URL or path
    /// </summary>
    /// <example>https://images.millionluxury.com/property-123/main-bedroom.jpg</example>
    [Description("Image URL or path")]
    public required string Url { get; set; }
    
    /// <summary>
    /// Whether this is the primary/featured image
    /// </summary>
    /// <example>true</example>
    [Description("Whether this is the primary/featured image")]
    public bool IsPrimary { get; set; }
    
    /// <summary>
    /// Sort order for image display (lower numbers first)
    /// </summary>
    /// <example>1</example>
    [Description("Sort order for image display (lower numbers first)")]
    public short SortOrder { get; set; }
    
    /// <summary>
    /// Whether the image is enabled/visible
    /// </summary>
    /// <example>true</example>
    [Description("Whether the image is enabled/visible")]
    public bool Enabled { get; set; }
    
    /// <summary>
    /// Alternative text for accessibility
    /// </summary>
    /// <example>Main bedroom with ocean view and modern decor</example>
    [Description("Alternative text for accessibility")]
    public string? AltText { get; set; }
}

/// <summary>
/// Property price change history information
/// </summary>
public class PropertyPriceChangeDto
{
    /// <summary>
    /// Date and time when the price change occurred
    /// </summary>
    /// <example>2024-01-20T10:30:00Z</example>
    [Description("Date and time when the price change occurred")]
    public DateTimeOffset EventDate { get; set; }
    
    /// <summary>
    /// Previous price value
    /// </summary>
    /// <example>2400000.00</example>
    [Description("Previous price value")]
    public decimal? OldValue { get; set; }
    
    /// <summary>
    /// New price value
    /// </summary>
    /// <example>2500000.00</example>
    [Description("New price value")]
    public decimal? NewValue { get; set; }
    
    /// <summary>
    /// Tax amount associated with the price change
    /// </summary>
    /// <example>5000.00</example>
    [Description("Tax amount associated with the price change")]
    public decimal? TaxAmount { get; set; }
    
    /// <summary>
    /// Name of the person who made the price change
    /// </summary>
    /// <example>Kevin Ramirez</example>
    [Description("Name of the person who made the price change")]
    public string? ActorName { get; set; }
}