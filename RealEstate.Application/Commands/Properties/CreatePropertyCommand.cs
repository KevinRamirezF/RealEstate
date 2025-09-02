using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class CreatePropertyCommand
{
    public CreatePropertyDto Data { get; set; } = null!;
}