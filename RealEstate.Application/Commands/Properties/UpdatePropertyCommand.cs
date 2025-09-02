using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class UpdatePropertyCommand
{
    public Guid Id { get; set; }
    public UpdatePropertyDto Property { get; set; } = null!;
}

public class UpdatePropertyResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ListingStatus { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
    public int RowVersion { get; set; }
}