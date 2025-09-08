using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Owners;

public class CreateOwnerCommand
{
    public CreateOwnerDto Data { get; set; } = null!;
}