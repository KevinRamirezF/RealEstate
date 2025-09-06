using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class ChangePriceCommand
{
    public Guid Id { get; set; }
    public ChangePriceDto PriceChange { get; set; } = null!;
}

public class ChangePriceResult
{
    public Guid Id { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? ActorName { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public byte[] RowVersion { get; set; } = new byte[8];
}