using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class UpdatePropertyCommand
{
    public Guid Id { get; set; }
    public UpdatePropertyDto Data { get; set; } = null!;
}