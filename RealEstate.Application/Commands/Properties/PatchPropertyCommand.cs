using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class PatchPropertyCommand
{
    public Guid Id { get; set; }
    public PatchPropertyDto Data { get; set; } = null!;
}