namespace RealEstate.Application.DTOs.Input;

public class ChangePriceDto
{
    public decimal NewPrice { get; set; }
    public decimal? TaxAmount { get; set; }
    public string? ActorName { get; set; }
    public int RowVersion { get; set; }
}