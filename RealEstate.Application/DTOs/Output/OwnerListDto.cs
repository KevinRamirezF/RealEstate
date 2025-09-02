namespace RealEstate.Application.DTOs.Output;

public class OwnerListDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int PropertyCount { get; set; }
    public DateTime CreatedAt { get; set; }
}