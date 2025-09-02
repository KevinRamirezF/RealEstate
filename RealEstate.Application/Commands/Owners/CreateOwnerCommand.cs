using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Owners;

public class CreateOwnerCommand
{
    public CreateOwnerDto Owner { get; set; } = null!;
}

public class CreateOwnerResult
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? ExternalCode { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int RowVersion { get; set; }
}