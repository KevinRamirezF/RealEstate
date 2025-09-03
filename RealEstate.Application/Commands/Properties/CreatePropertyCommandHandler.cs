using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using System.Reflection;

namespace RealEstate.Application.Commands.Properties;

public class CreatePropertyCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PropertyMapper _mapper;
    private readonly CreatePropertyDtoValidator _validator;

    public CreatePropertyCommandHandler(IUnitOfWork unitOfWork, PropertyMapper mapper, CreatePropertyDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<PropertyDetailDto> HandleAsync(CreatePropertyCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command.Data, cancellationToken);

        // Check if owner exists
        var owner = await _unitOfWork.Owners.GetByIdAsync(command.Data.OwnerId, cancellationToken);
        if (owner == null)
            throw new ValidationException($"Owner with ID {command.Data.OwnerId} does not exist.");

        // Check code internal uniqueness
        if (await _unitOfWork.Properties.ExistsByCodeInternalAsync(command.Data.CodeInternal, cancellationToken: cancellationToken))
            throw new ValidationException($"Property with CodeInternal '{command.Data.CodeInternal}' already exists.");

        // Parse enums
        var propertyType = Enum.Parse<PropertyType>(command.Data.PropertyType);
        var listingStatus = Enum.Parse<ListingStatus>(command.Data.ListingStatus);

        // Create property entity
        var property = Property.Create(
            command.Data.OwnerId,
            command.Data.CodeInternal,
            command.Data.Name,
            propertyType,
            command.Data.Price,
            command.Data.AddressLine,
            command.Data.City,
            command.Data.State,
            command.Data.PostalCode
        );

        // Set optional properties using reflection (since properties have private setters)
        var propertyType2 = typeof(Property);
        SetProperty(property, propertyType2, nameof(Property.Description), command.Data.Description);
        SetProperty(property, propertyType2, nameof(Property.YearBuilt), command.Data.YearBuilt);
        SetProperty(property, propertyType2, nameof(Property.Bedrooms), command.Data.Bedrooms);
        SetProperty(property, propertyType2, nameof(Property.Bathrooms), command.Data.Bathrooms);
        SetProperty(property, propertyType2, nameof(Property.ParkingSpaces), command.Data.ParkingSpaces);
        SetProperty(property, propertyType2, nameof(Property.AreaSqft), command.Data.AreaSqft);
        SetProperty(property, propertyType2, nameof(Property.LotSizeSqft), command.Data.LotSizeSqft);
        SetProperty(property, propertyType2, nameof(Property.Currency), command.Data.Currency);
        SetProperty(property, propertyType2, nameof(Property.HoaFee), command.Data.HoaFee);
        SetProperty(property, propertyType2, nameof(Property.Country), command.Data.Country);
        SetProperty(property, propertyType2, nameof(Property.Lat), command.Data.Lat);
        SetProperty(property, propertyType2, nameof(Property.Lng), command.Data.Lng);
        SetProperty(property, propertyType2, nameof(Property.ListingStatus), listingStatus);
        SetProperty(property, propertyType2, nameof(Property.ListingDate), command.Data.ListingDate ?? DateOnly.FromDateTime(DateTime.Today));
        SetProperty(property, propertyType2, nameof(Property.LastSoldPrice), command.Data.LastSoldPrice);
        SetProperty(property, propertyType2, nameof(Property.IsFeatured), command.Data.IsFeatured);
        SetProperty(property, propertyType2, nameof(Property.IsPublished), command.Data.IsPublished);

        await _unitOfWork.Properties.AddAsync(property, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Get the created property with all details populated
        var createdProperty = await _unitOfWork.Properties.GetPropertyDetailAsync(property.Id, cancellationToken);
        return createdProperty!;
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