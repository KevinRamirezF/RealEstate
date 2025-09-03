namespace RealEstate.Application.DTOs.Output;

public class OwnerDto
{
    public Guid Id { get; set; }
    public string? ExternalCode { get; set; }
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PhotoUrl { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "US";
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int RowVersion { get; set; }
}