using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for creating a new property owner
/// </summary>
public class CreateOwnerDto
{
    /// <summary>
    /// Owner's full name (required, max 200 characters)
    /// </summary>
    /// <example>John Smith</example>
    [Description("Owner's full name (required, max 200 characters)")]
    public required string FullName { get; set; }
    
    /// <summary>
    /// Owner's email address (optional, must be valid email format)
    /// </summary>
    /// <example>john.smith@email.com</example>
    [Description("Owner's email address (optional, must be valid email format)")]
    public string? Email { get; set; }
    
    /// <summary>
    /// Owner's phone number (optional, format: (555) 123-4567 or +1-555-123-4567)
    /// </summary>
    /// <example>(555) 123-4567</example>
    [Description("Owner's phone number (optional, format: (555) 123-4567 or +1-555-123-4567)")]
    public string? Phone { get; set; }
    
    /// <summary>
    /// External system identifier (optional, alphanumeric, max 50 characters)
    /// </summary>
    /// <example>EXT001</example>
    [Description("External system identifier (optional, alphanumeric, max 50 characters)")]
    public string? ExternalCode { get; set; }
    
    /// <summary>
    /// URL to owner's photo (optional, must be valid URL format)
    /// </summary>
    /// <example>https://example.com/photos/john-smith.jpg</example>
    [Description("URL to owner's photo (optional, must be valid URL format)")]
    public string? PhotoUrl { get; set; }
    
    /// <summary>
    /// Owner's birth date (optional, format: YYYY-MM-DD, must be 18+ years old)
    /// </summary>
    /// <example>1985-06-15</example>
    [Description("Owner's birth date (optional, format: YYYY-MM-DD, must be 18+ years old)")]
    public DateOnly? BirthDate { get; set; }
    
    /// <summary>
    /// Street address (optional, max 200 characters)
    /// </summary>
    /// <example>456 Oak Avenue</example>
    [Description("Street address (optional, max 200 characters)")]
    public string? AddressLine { get; set; }
    
    /// <summary>
    /// City name (optional, max 120 characters)
    /// </summary>
    /// <example>New York</example>
    [Description("City name (optional, max 120 characters)")]
    public string? City { get; set; }
    
    /// <summary>
    /// State/Province (optional, max 50 characters)
    /// </summary>
    /// <example>NY</example>
    [Description("State/Province (optional, max 50 characters)")]
    public string? State { get; set; }
    
    /// <summary>
    /// Postal/ZIP code (optional, max 10 characters)
    /// </summary>
    /// <example>10001</example>
    [Description("Postal/ZIP code (optional, max 10 characters)")]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Country code (2-character ISO code: US, CA, MX, etc.)
    /// </summary>
    /// <example>US</example>
    [Description("Country code (2-character ISO code: US, CA, MX, etc.)")]
    public string Country { get; set; } = "US";
}