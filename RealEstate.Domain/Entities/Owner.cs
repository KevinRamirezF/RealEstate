using System;

namespace RealEstate.Domain.Entities;

public class Owner
{
    public Guid Id { get; private set; }
    public string? ExternalCode { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? PhotoUrl { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? AddressLine1 { get; private set; }
    public string? AddressLine2 { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string Country { get; private set; } = "US";
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public int RowVersion { get; private set; } = 1;

    private Owner() { }

    private Owner(Guid id, string fullName, string? email, string? phone, string? externalCode)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Phone = phone;
        ExternalCode = externalCode;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Owner Create(string fullName, string? email = null, string? phone = null, string? externalCode = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        
        if (fullName.Length > 180)
            throw new ArgumentException("Full name cannot exceed 180 characters.", nameof(fullName));

        return new Owner(Guid.NewGuid(), fullName, email, phone, externalCode);
    }

    public void Update(string fullName, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        FullName = fullName;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }

    public void SoftDelete()
    {
        DeletedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        RowVersion++;
    }
}
