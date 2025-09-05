using System.ComponentModel;

namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// Basic owner information for API responses
/// </summary>
public class OwnerDto
{
    /// <summary>
    /// Unique owner identifier
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Description("Unique owner identifier")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// External system identifier (optional)
    /// </summary>
    /// <example>EXT001</example>
    [Description("External system identifier (optional)")]
    public string? ExternalCode { get; set; }
    
    /// <summary>
    /// Owner's full name (first and last name combined)
    /// </summary>
    /// <example>John Smith</example>
    [Description("Owner's full name (first and last name combined)")]
    public required string FullName { get; set; }
    
    /// <summary>
    /// Owner's email address (optional)
    /// </summary>
    /// <example>john.smith@email.com</example>
    [Description("Owner's email address (optional)")]
    public string? Email { get; set; }
    
    /// <summary>
    /// Owner's phone number (optional)
    /// </summary>
    /// <example>(305) 123-4567</example>
    [Description("Owner's phone number (optional)")]
    public string? Phone { get; set; }
    
    /// <summary>
    /// URL to owner's photo (optional)
    /// </summary>
    /// <example>https://images.millionluxury.com/owners/john-smith.jpg</example>
    [Description("URL to owner's photo (optional)")]
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Owner's birth date (optional)
    /// </summary>
    /// <example>1985-05-20</example>
    [Description("Owner's birth date (optional)")]
    public DateOnly? BirthDate { get; set; }
    
    /// <summary>
    /// Street address where the owner resides (optional)
    /// </summary>
    /// <example>123 Main Street, Apt 101</example>
    [Description("Street address where the owner resides (optional)")]
    public string? AddressLine { get; set; }
    
    /// <summary>
    /// City where the owner resides (optional)
    /// </summary>
    /// <example>Miami</example>
    [Description("City where the owner resides (optional)")]
    public string? City { get; set; }
    
    /// <summary>
    /// State/Province where the owner resides (optional)
    /// </summary>
    /// <example>FL</example>
    [Description("State/Province where the owner resides (optional)")]
    public string? State { get; set; }
    
    /// <summary>
    /// Postal/ZIP code where the owner resides (optional)
    /// </summary>
    /// <example>33101</example>
    [Description("Postal/ZIP code where the owner resides (optional)")]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Country where the owner resides
    /// </summary>
    /// <example>US</example>
    [Description("Country where the owner resides")]
    public string Country { get; set; } = "US";
    
    /// <summary>
    /// Whether the owner is active in the system
    /// </summary>
    /// <example>true</example>
    [Description("Whether the owner is active in the system")]
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Date and time when owner was created in the system
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    [Description("Date and time when owner was created in the system")]
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when owner was last updated
    /// </summary>
    /// <example>2024-01-20T14:45:00Z</example>
    [Description("Date and time when owner was last updated")]
    public DateTimeOffset UpdatedAt { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    /// <example>1</example>
    [Description("Row version for optimistic concurrency control")]
    public int RowVersion { get; set; }
}