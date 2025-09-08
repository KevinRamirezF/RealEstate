using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Commands.Properties;

public class AddImageCommand
{
    public Guid PropertyId { get; set; }
    public AddImageDto Image { get; set; } = null!;
}

public class AddImageResult
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string StorageProvider { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public short SortOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}