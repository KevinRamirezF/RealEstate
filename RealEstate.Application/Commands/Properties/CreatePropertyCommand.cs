using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class CreatePropertyCommand
{
    public CreatePropertyDto Property { get; set; } = null!;
}

public class CreatePropertyResult
{
    public Guid Id { get; set; }
    public string CodeInternal { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ListingStatus { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public int RowVersion { get; set; }
}