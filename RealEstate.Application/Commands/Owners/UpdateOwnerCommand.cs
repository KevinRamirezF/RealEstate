using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Owners;

public class UpdateOwnerCommand
{
    public Guid Id { get; set; }
    public UpdateOwnerDto Data { get; set; } = null!;
}