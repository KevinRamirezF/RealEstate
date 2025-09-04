using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Owners;

public class PatchOwnerCommand
{
    public Guid Id { get; set; }
    public PatchOwnerDto Data { get; set; } = null!;
}