using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Application.DTOs.Input;

/// <summary>
/// DTO for changing property price with audit information
/// </summary>
public class ChangePriceDto
{
    /// <summary>
    /// New base price for the property (required, must be greater than 0)
    /// </summary>
    /// <example>2500000.00</example>
    [Description("New base price for the property (required, must be greater than 0)")]
    public decimal BasePrice { get; set; }
    
    /// <summary>
    /// Tax amount for the property (required, must be greater than or equal to 0)
    /// </summary>
    /// <example>250000.00</example>
    [Description("Tax amount for the property (required, must be greater than or equal to 0)")]
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Name of the person making the price change (optional, max 100 characters)
    /// </summary>
    /// <example>Kevin Ramirez</example>
    [Description("Name of the person making the price change (optional, max 100 characters)")]
    public string? ActorName { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control (required)
    /// </summary>
    /// <example>AAAAAAAAB9E=</example>
    [Description("Row version for optimistic concurrency control (required)")]
    public byte[] RowVersion { get; set; } = new byte[8];
}