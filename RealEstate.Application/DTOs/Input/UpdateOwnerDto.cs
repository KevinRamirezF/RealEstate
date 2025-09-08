using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for updating owner information (PUT - complete replacement of owner data)
/// </summary>
public class UpdateOwnerDto
{
    /// <summary>
    /// Owner's first name (required, max 100 characters)
    /// </summary>
    /// <example>John</example>
    [Description("Owner's first name (required, max 100 characters)")]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner's last name (required, max 100 characters)
    /// </summary>
    /// <example>Smith</example>
    [Description("Owner's last name (required, max 100 characters)")]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner's email address (required, must be valid email format)
    /// </summary>
    /// <example>john.smith.updated@email.com</example>
    [Description("Owner's email address (required, must be valid email format)")]
    public string Email { get; set; } = string.Empty;
    
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
    /// Street address (required, max 200 characters)
    /// </summary>
    /// <example>789 Updated Avenue</example>
    [Description("Street address (required, max 200 characters)")]
    public string AddressLine { get; set; } = string.Empty;
    
    /// <summary>
    /// City name (required, max 120 characters)
    /// </summary>
    /// <example>Miami</example>
    [Description("City name (required, max 120 characters)")]
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// State/Province (required, max 50 characters)
    /// </summary>
    /// <example>FL</example>
    [Description("State/Province (required, max 50 characters)")]
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// Postal/ZIP code (required, max 10 characters)
    /// </summary>
    /// <example>33101</example>
    [Description("Postal/ZIP code (required, max 10 characters)")]
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Country code (required, 2-character ISO code: US, CA, MX, etc.)
    /// </summary>
    /// <example>US</example>
    [Description("Country code (required, 2-character ISO code: US, CA, MX, etc.)")]
    public string Country { get; set; } = string.Empty;
    
    /// <summary>
    /// External system identifier (optional, alphanumeric, max 50 characters)
    /// </summary>
    /// <example>EXT001-UPDATED</example>
    [Description("External system identifier (optional, alphanumeric, max 50 characters)")]
    public string? ExternalCode { get; set; }
}