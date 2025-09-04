using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Enums;
using System.Reflection;

namespace RealEstate.Application.Commands.Properties;

public class UpdatePropertyCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PropertyMapper _mapper;
    private readonly UpdatePropertyDtoValidator _validator;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork, PropertyMapper mapper, UpdatePropertyDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<PropertyDetailDto?> HandleAsync(UpdatePropertyCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command.Data, cancellationToken);

        var property = await _unitOfWork.Properties.GetByIdAsync(command.Id, cancellationToken);
        if (property == null)
            return null;

        // Parse enum if provided
        ListingStatus? listingStatus = null;
        if (!string.IsNullOrWhiteSpace(command.Data.ListingStatus))
            listingStatus = Enum.Parse<ListingStatus>(command.Data.ListingStatus);

        // Update properties using reflection since properties have private setters
        var propertyTypeReflection = typeof(Domain.Entities.Property);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Name), command.Data.Name);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Description), command.Data.Description);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.YearBuilt), command.Data.YearBuilt);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Bedrooms), command.Data.Bedrooms);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Bathrooms), command.Data.Bathrooms);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.ParkingSpaces), command.Data.ParkingSpaces);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.AreaSqft), command.Data.AreaSqft);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.AddressLine), command.Data.AddressLine);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.City), command.Data.City);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.State), command.Data.State);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.PostalCode), command.Data.PostalCode);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Lat), command.Data.Lat);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Lng), command.Data.Lng);
        if (listingStatus.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.ListingStatus), listingStatus.Value);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.ListingDate), command.Data.ListingDate);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.IsFeatured), command.Data.IsFeatured);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.IsPublished), command.Data.IsPublished);
        SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.UpdatedAt), DateTimeOffset.UtcNow);

        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return updated property details
        return await _unitOfWork.Properties.GetPropertyDetailAsync(property.Id, cancellationToken);
    }

    private static void SetProperty(object obj, Type type, string propertyName, object? value)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            var backingField = type.GetField($"<{propertyName}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
            backingField?.SetValue(obj, value);
        }
    }
}