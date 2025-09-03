namespace RealEstate.Application.DTOs.Input;

public class CreateOwnerDto
{
    public required string FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? ExternalCode { get; set; }
    public string? PhotoUrl { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "US";
}