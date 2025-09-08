using System.ComponentModel;

namespace RealEstate.Application.DTOs.Output;

/// <summary>
/// Simplified owner information for list views
/// </summary>
public class OwnerListDto
{
    /// <summary>
    /// Unique owner identifier
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Description("Unique owner identifier")]
    public Guid Id { get; set; }
    
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
}