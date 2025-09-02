using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Commands.Properties;

public class CreatePropertyCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreatePropertyDtoValidator _validator;

    public CreatePropertyCommandHandler(IUnitOfWork unitOfWork, CreatePropertyDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreatePropertyResult> Handle(CreatePropertyCommand command, CancellationToken cancellationToken = default)
    {
        // Validate input
        var validationResult = await _validator.ValidateAsync(command.Property, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Check if owner exists
        var owner = await _unitOfWork.Owners.GetByIdAsync(command.Property.OwnerId, cancellationToken);
        if (owner == null)
        {
            throw new ArgumentException($"Owner with ID {command.Property.OwnerId} does not exist.");
        }

        // Check code internal uniqueness
        var codeExists = await _unitOfWork.Properties.ExistsByCodeInternalAsync(command.Property.CodeInternal, cancellationToken: cancellationToken);
        if (codeExists)
        {
            throw new ArgumentException($"Property with CodeInternal '{command.Property.CodeInternal}' already exists.");
        }

        // Parse enums
        var propertyType = Enum.Parse<PropertyType>(command.Property.PropertyType);
        var listingStatus = Enum.Parse<ListingStatus>(command.Property.ListingStatus);

        // Create property entity
        var property = Property.Create(
            command.Property.OwnerId,
            command.Property.CodeInternal,
            command.Property.Name,
            propertyType,
            command.Property.Price,
            command.Property.AddressLine1,
            command.Property.City,
            command.Property.State,
            command.Property.PostalCode
        );

        // Set optional properties using reflection (since properties have private setters)
        SetPropertyValue(property, nameof(Property.Description), command.Property.Description);
        SetPropertyValue(property, nameof(Property.YearBuilt), command.Property.YearBuilt);
        SetPropertyValue(property, nameof(Property.Bedrooms), command.Property.Bedrooms);
        SetPropertyValue(property, nameof(Property.Bathrooms), command.Property.Bathrooms);
        SetPropertyValue(property, nameof(Property.ParkingSpaces), command.Property.ParkingSpaces);
        SetPropertyValue(property, nameof(Property.AreaSqft), command.Property.AreaSqft);
        SetPropertyValue(property, nameof(Property.LotSizeSqft), command.Property.LotSizeSqft);
        SetPropertyValue(property, nameof(Property.Currency), command.Property.Currency);
        SetPropertyValue(property, nameof(Property.HoaFee), command.Property.HoaFee);
        SetPropertyValue(property, nameof(Property.AddressLine2), command.Property.AddressLine2);
        SetPropertyValue(property, nameof(Property.Country), command.Property.Country);
        SetPropertyValue(property, nameof(Property.Lat), command.Property.Lat);
        SetPropertyValue(property, nameof(Property.Lng), command.Property.Lng);
        SetPropertyValue(property, nameof(Property.ListingStatus), listingStatus);
        SetPropertyValue(property, nameof(Property.ListingDate), command.Property.ListingDate ?? DateOnly.FromDateTime(DateTime.Today));
        SetPropertyValue(property, nameof(Property.LastSoldPrice), command.Property.LastSoldPrice);
        SetPropertyValue(property, nameof(Property.IsFeatured), command.Property.IsFeatured);
        SetPropertyValue(property, nameof(Property.IsPublished), command.Property.IsPublished);

        // Add to repository
        await _unitOfWork.Properties.AddAsync(property, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreatePropertyResult
        {
            Id = property.Id,
            CodeInternal = property.CodeInternal,
            Name = property.Name,
            Price = property.Price,
            ListingStatus = property.ListingStatus.ToString(),
            CreatedAt = property.CreatedAt,
            RowVersion = property.RowVersion
        };
    }

    private static void SetPropertyValue(object obj, string propertyName, object? value)
    {
        if (value == null) return;
        
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
    }
}