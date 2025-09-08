using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for partial owner updates (PATCH - only sends fields that need to be updated)
/// </summary>
public class PatchOwnerDto
{
    /// <summary>
    /// Owner's first name (optional, max 100 characters)
    /// </summary>
    /// <example>John</example>
    [Description("Owner's first name (optional, max 100 characters)")]
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Owner's last name (optional, max 100 characters)
    /// </summary>
    /// <example>Smith</example>
    [Description("Owner's last name (optional, max 100 characters)")]
    public string? LastName { get; set; }
    
    /// <summary>
    /// Owner's email address (optional, must be valid email format)
    /// </summary>
    /// <example>john.smith.updated@email.com</example>
    [Description("Owner's email address (optional, must be valid email format)")]
    public string? Email { get; set; }
    
    /// <summary>
    /// Owner's phone number (optional, format: (555) 123-4567 or +1-555-123-4567)
    /// </summary>
    /// <example>(555) 987-6543</example>
    [Description("Owner's phone number (optional, format: (555) 123-4567 or +1-555-123-4567)")]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Owner's birth date (optional, format: YYYY-MM-DD, must be 18+ years old)
    /// </summary>
    /// <example>1980-05-20</example>
    [Description("Owner's birth date (optional, format: YYYY-MM-DD, must be 18+ years old)")]
    public DateOnly? DateOfBirth { get; set; }
    
    /// <summary>
    /// Street address (optional, max 200 characters)
    /// </summary>
    /// <example>789 Updated Avenue</example>
    [Description("Street address (optional, max 200 characters)")]
    public string? AddressLine { get; set; }
    
    /// <summary>
    /// City name (optional, max 120 characters)
    /// </summary>
    /// <example>Miami</example>
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
    /// <example>33101</example>
    [Description("Postal/ZIP code (optional, max 10 characters)")]
    public string? PostalCode { get; set; }
    
    /// <summary>
    /// Country code (optional, 2-character ISO code: US, CA, MX, etc.)
    /// </summary>
    /// <example>US</example>
    [Description("Country code (optional, 2-character ISO code: US, CA, MX, etc.)")]
    public string? Country { get; set; }
    
    /// <summary>
    /// External system identifier (optional, alphanumeric, max 50 characters)
    /// </summary>
    /// <example>EXT001-UPDATED</example>
    [Description("External system identifier (optional, alphanumeric, max 50 characters)")]
    public string? ExternalCode { get; set; }
}