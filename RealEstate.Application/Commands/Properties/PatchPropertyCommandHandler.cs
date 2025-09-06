using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Enums;
using System.Reflection;

namespace RealEstate.Application.Commands.Properties;

public class PatchPropertyCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PropertyMapper _mapper;
    private readonly PatchPropertyDtoValidator _validator;

    public PatchPropertyCommandHandler(IUnitOfWork unitOfWork, PropertyMapper mapper, PatchPropertyDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<PropertyDetailDto?> HandleAsync(PatchPropertyCommand command, CancellationToken cancellationToken = default)
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
        // Only update properties that are not null (PATCH behavior)
        var propertyTypeReflection = typeof(Domain.Entities.Property);
        
        if (command.Data.Name != null)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Name), command.Data.Name);
        if (command.Data.Description != null)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Description), command.Data.Description);
        if (command.Data.YearBuilt.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.YearBuilt), command.Data.YearBuilt);
        if (command.Data.Bedrooms.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Bedrooms), command.Data.Bedrooms);
        if (command.Data.Bathrooms.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Bathrooms), command.Data.Bathrooms);
        if (command.Data.ParkingSpaces.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.ParkingSpaces), command.Data.ParkingSpaces);
        if (command.Data.AreaSqft.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.AreaSqft), command.Data.AreaSqft);
        if (command.Data.AddressLine != null)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.AddressLine), command.Data.AddressLine);
        if (command.Data.City != null)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.City), command.Data.City);
        if (command.Data.State != null)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.State), command.Data.State);
        if (command.Data.PostalCode != null)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.PostalCode), command.Data.PostalCode);
        if (command.Data.Lat.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Lat), command.Data.Lat);
        if (command.Data.Lng.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Lng), command.Data.Lng);
        if (listingStatus.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.ListingStatus), listingStatus.Value);
        if (command.Data.ListingDate.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.ListingDate), command.Data.ListingDate);
        if (command.Data.IsFeatured.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.IsFeatured), command.Data.IsFeatured);
        if (command.Data.IsPublished.HasValue)
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.IsPublished), command.Data.IsPublished);
            
        // Handle price updates using domain method to maintain data integrity
        if (command.Data.BasePrice.HasValue && command.Data.TaxAmount.HasValue)
        {
            // Use domain method which creates proper traces and maintains invariants
            property.ChangePrice(command.Data.BasePrice.Value, command.Data.TaxAmount.Value, "PATCH Update");
        }
        else if (command.Data.BasePrice.HasValue || command.Data.TaxAmount.HasValue)
        {
            // Partial price update - use reflection but maintain Price = BasePrice + TaxAmount invariant
            if (command.Data.BasePrice.HasValue)
                SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.BasePrice), command.Data.BasePrice.Value);
            if (command.Data.TaxAmount.HasValue)
                SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.TaxAmount), command.Data.TaxAmount.Value);
                
            // Recalculate total price to maintain invariant
            var currentBasePrice = (decimal)propertyTypeReflection.GetProperty("BasePrice")?.GetValue(property)!;
            var currentTaxAmount = (decimal)propertyTypeReflection.GetProperty("TaxAmount")?.GetValue(property)!;
            SetProperty(property, propertyTypeReflection, nameof(Domain.Entities.Property.Price), currentBasePrice + currentTaxAmount);
        }
        
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