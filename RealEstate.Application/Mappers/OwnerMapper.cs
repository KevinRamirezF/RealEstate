using RealEstate.Application.DTOs.Input;
using RealEstate.Application.DTOs.Output;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Mappers;

public class OwnerMapper
{
    // Manual mappings since entities use factory methods
    public OwnerDto ToDto(Owner entity)
    {
        return new OwnerDto
        {
            Id = entity.Id,
            ExternalCode = entity.ExternalCode,
            FullName = entity.FullName,
            Email = entity.Email,
            Phone = entity.Phone,
            PhotoUrl = entity.PhotoUrl,
            BirthDate = entity.BirthDate,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            City = entity.City,
            State = entity.State,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RowVersion = entity.RowVersion
        };
    }
}