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

        // Use domain method for updates (creates appropriate traces)
        property.Update(
            name: command.Data.Name,
            description: command.Data.Description,
            bedrooms: command.Data.Bedrooms,
            bathrooms: command.Data.Bathrooms,
            parkingSpaces: command.Data.ParkingSpaces,
            areaSqft: command.Data.AreaSqft,
            basePrice: command.Data.BasePrice,
            taxAmount: command.Data.TaxAmount,
            yearBuilt: command.Data.YearBuilt,
            addressLine: command.Data.AddressLine,
            city: command.Data.City,
            state: command.Data.State,
            postalCode: command.Data.PostalCode,
            lat: command.Data.Lat,
            lng: command.Data.Lng,
            isFeatured: command.Data.IsFeatured,
            isPublished: command.Data.IsPublished
        );
        
        // Handle ListingStatus and ListingDate separately via reflection (not in domain method yet)
        if (!string.IsNullOrWhiteSpace(command.Data.ListingStatus))
        {
            var listingStatus = Enum.Parse<ListingStatus>(command.Data.ListingStatus);
            SetProperty(property, typeof(Domain.Entities.Property), nameof(Domain.Entities.Property.ListingStatus), listingStatus);
        }
        
        if (command.Data.ListingDate.HasValue)
        {
            SetProperty(property, typeof(Domain.Entities.Property), nameof(Domain.Entities.Property.ListingDate), command.Data.ListingDate.Value);
        }

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