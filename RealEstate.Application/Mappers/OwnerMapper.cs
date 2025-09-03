using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mappers;

public class OwnerMapper
{
    public OwnerListDto ToListDto(Owner entity)
    {
        return new OwnerListDto
        {
            Id = entity.Id,
            FullName = entity.FullName,
            Email = entity.Email ?? string.Empty,
            PhoneNumber = entity.Phone,
            City = entity.City ?? string.Empty,
            State = entity.State ?? string.Empty,
            PropertyCount = 0, // TODO: Calculate property count
            CreatedAt = entity.CreatedAt.DateTime
        };
    }

    public OwnerDetailDto ToDetailDto(Owner entity)
    {
        var fullNameParts = entity.FullName.Split(' ', 2);
        var firstName = fullNameParts.Length > 0 ? fullNameParts[0] : string.Empty;
        var lastName = fullNameParts.Length > 1 ? fullNameParts[1] : string.Empty;

        return new OwnerDetailDto
        {
            Id = entity.Id,
            FirstName = firstName,
            LastName = lastName,
            FullName = entity.FullName,
            Email = entity.Email ?? string.Empty,
            PhoneNumber = entity.Phone,
            DateOfBirth = entity.BirthDate,
            AddressLine = entity.AddressLine ?? string.Empty,
            City = entity.City ?? string.Empty,
            State = entity.State ?? string.Empty,
            PostalCode = entity.PostalCode ?? string.Empty,
            Country = entity.Country,
            ExternalCode = entity.ExternalCode,
            PropertyCount = 0, // TODO: Calculate property count
            CreatedAt = entity.CreatedAt.DateTime,
            UpdatedAt = entity.UpdatedAt.DateTime,
            RowVersion = entity.RowVersion
        };
    }
}