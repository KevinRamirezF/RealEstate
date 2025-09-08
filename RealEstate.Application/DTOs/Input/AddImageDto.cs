using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for adding an image to a property
/// </summary>
public class AddImageDto
{
    /// <summary>
    /// Image URL or file path (required, max 500 characters, must be valid URL format)
    /// </summary>
    /// <example>https://images.millionluxury.com/property-123/living-room.jpg</example>
    [Description("Image URL or file path (required, max 500 characters, must be valid URL format)")]
    public required string Url { get; set; }
    
    /// <summary>
    /// Storage provider identifier (required, max 20 characters, valid values: S3, Azure, Local)
    /// </summary>
    /// <example>S3</example>
    [Description("Storage provider identifier (required, max 20 characters, valid values: S3, Azure, Local)")]
    public string StorageProvider { get; set; } = "S3";
    
    /// <summary>
    /// Alternative text for accessibility (optional, max 200 characters)
    /// </summary>
    /// <example>Spacious living room with floor-to-ceiling windows and ocean view</example>
    [Description("Alternative text for accessibility (optional, max 200 characters)")]
    public string? AltText { get; set; }
    
    /// <summary>
    /// Whether this image should be the primary/featured image (required, only one primary allowed per property)
    /// </summary>
    /// <example>false</example>
    [Description("Whether this image should be the primary/featured image (required, only one primary allowed per property)")]
    public bool IsPrimary { get; set; } = false;
    
    /// <summary>
    /// Sort order for image display (required, must be 0-999, lower numbers display first)
    /// </summary>
    /// <example>2</example>
    [Description("Sort order for image display (required, must be 0-999, lower numbers display first)")]
    public short SortOrder { get; set; } = 0;
}