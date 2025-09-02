namespace RealEstate.Application.DTOs.Input;

public class AddImageDto
{
    public required string Url { get; set; }
    public string StorageProvider { get; set; } = "S3";
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; } = false;
    public short SortOrder { get; set; } = 0;
}