using System.ComponentModel;

namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// Complete owner details with all personal and address information
/// </summary>
public class OwnerDetailDto
{
    /// <summary>
    /// Unique owner identifier
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Description("Unique owner identifier")]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Owner's first name
    /// </summary>
    /// <example>John</example>
    [Description("Owner's first name")]
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner's last name
    /// </summary>
    /// <example>Smith</example>
    [Description("Owner's last name")]
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner's full name (first and last name combined)
    /// </summary>
    /// <example>John Smith</example>
    [Description("Owner's full name (first and last name combined)")]
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner's email address
    /// </summary>
    /// <example>john.smith@email.com</example>
    [Description("Owner's email address")]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Owner's phone number (optional)
    /// </summary>
    /// <example>(305) 123-4567</example>
    [Description("Owner's phone number (optional)")]
    public string? PhoneNumber { get; set; }
    
    /// <summary>
    /// Owner's birth date (optional)
    /// </summary>
    /// <example>1985-05-20</example>
    [Description("Owner's birth date (optional)")]
    public DateOnly? DateOfBirth { get; set; }
    
    /// <summary>
    /// Street address where the owner resides
    /// </summary>
    /// <example>123 Main Street, Apt 101</example>
    [Description("Street address where the owner resides")]
    public string AddressLine { get; set; } = string.Empty;
    
    /// <summary>
    /// City where the owner resides
    /// </summary>
    /// <example>Miami</example>
    [Description("City where the owner resides")]
    public string City { get; set; } = string.Empty;
    
    /// <summary>
    /// State/Province where the owner resides
    /// </summary>
    /// <example>FL</example>
    [Description("State/Province where the owner resides")]
    public string State { get; set; } = string.Empty;
    
    /// <summary>
    /// Postal/ZIP code where the owner resides
    /// </summary>
    /// <example>33101</example>
    [Description("Postal/ZIP code where the owner resides")]
    public string PostalCode { get; set; } = string.Empty;
    
    /// <summary>
    /// Country where the owner resides
    /// </summary>
    /// <example>US</example>
    [Description("Country where the owner resides")]
    public string Country { get; set; } = string.Empty;
    
    /// <summary>
    /// External system identifier (optional)
    /// </summary>
    /// <example>EXT001</example>
    [Description("External system identifier (optional)")]
    public string? ExternalCode { get; set; }
    
    /// <summary>
    /// Number of properties owned by this owner
    /// </summary>
    /// <example>3</example>
    [Description("Number of properties owned by this owner")]
    public int PropertyCount { get; set; }
    
    /// <summary>
    /// Date and time when owner was created in the system
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    [Description("Date and time when owner was created in the system")]
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Date and time when owner was last updated (optional)
    /// </summary>
    /// <example>2024-01-20T14:45:00Z</example>
    [Description("Date and time when owner was last updated (optional)")]
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    /// <example>1</example>
    [Description("Row version for optimistic concurrency control")]
    public int RowVersion { get; set; }
}