using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;

namespace RealEstate.Application.Commands.Properties;

public class UpdatePropertyCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdatePropertyDtoValidator _validator;

    public UpdatePropertyCommandHandler(IUnitOfWork unitOfWork, UpdatePropertyDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<UpdatePropertyResult> Handle(UpdatePropertyCommand command, CancellationToken cancellationToken = default)
    {
        // Validate input
        var validationResult = await _validator.ValidateAsync(command.Property, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Get existing property
        var property = await _unitOfWork.Properties.GetByIdAsync(command.Id, cancellationToken);
        if (property == null)
        {
            throw new ArgumentException($"Property with ID {command.Id} not found.");
        }

        // Check concurrency
        if (property.RowVersion != command.Property.RowVersion)
        {
            throw new DbUpdateConcurrencyException("The property has been modified by another user. Please refresh and try again.");
        }

        // Update property using domain method
        property.Update(
            command.Property.Name,
            command.Property.Description,
            command.Property.Bedrooms,
            command.Property.Bathrooms,
            command.Property.ParkingSpaces,
            command.Property.AreaSqft,
            command.Property.LotSizeSqft,
            command.Property.HoaFee
        );

        // Update other properties using reflection (since properties have private setters)
        if (command.Property.YearBuilt.HasValue)
            SetPropertyValue(property, nameof(property.YearBuilt), command.Property.YearBuilt.Value);
        
        if (!string.IsNullOrEmpty(command.Property.AddressLine1))
            SetPropertyValue(property, nameof(property.AddressLine1), command.Property.AddressLine1);
        
        if (!string.IsNullOrEmpty(command.Property.AddressLine2))
            SetPropertyValue(property, nameof(property.AddressLine2), command.Property.AddressLine2);
        
        if (!string.IsNullOrEmpty(command.Property.City))
            SetPropertyValue(property, nameof(property.City), command.Property.City);
        
        if (!string.IsNullOrEmpty(command.Property.State))
            SetPropertyValue(property, nameof(property.State), command.Property.State);
        
        if (!string.IsNullOrEmpty(command.Property.PostalCode))
            SetPropertyValue(property, nameof(property.PostalCode), command.Property.PostalCode);
        
        if (command.Property.Lat.HasValue)
            SetPropertyValue(property, nameof(property.Lat), command.Property.Lat.Value);
        
        if (command.Property.Lng.HasValue)
            SetPropertyValue(property, nameof(property.Lng), command.Property.Lng.Value);
        
        if (!string.IsNullOrEmpty(command.Property.ListingStatus))
        {
            var listingStatus = Enum.Parse<Domain.Enums.ListingStatus>(command.Property.ListingStatus);
            SetPropertyValue(property, nameof(property.ListingStatus), listingStatus);
        }
        
        if (command.Property.ListingDate.HasValue)
            SetPropertyValue(property, nameof(property.ListingDate), command.Property.ListingDate.Value);
        
        if (command.Property.LastSoldPrice.HasValue)
            SetPropertyValue(property, nameof(property.LastSoldPrice), command.Property.LastSoldPrice.Value);
        
        SetPropertyValue(property, nameof(property.IsFeatured), command.Property.IsFeatured);
        SetPropertyValue(property, nameof(property.IsPublished), command.Property.IsPublished);

        // Update in repository
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdatePropertyResult
        {
            Id = property.Id,
            Name = property.Name,
            Price = property.Price,
            ListingStatus = property.ListingStatus.ToString(),
            UpdatedAt = property.UpdatedAt,
            RowVersion = property.RowVersion
        };
    }

    private static void SetPropertyValue(object obj, string propertyName, object value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
    }
}