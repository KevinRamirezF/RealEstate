using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;
using System.Reflection;

namespace RealEstate.Application.Commands.Owners;

public class UpdateOwnerCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OwnerMapper _mapper;
    private readonly UpdateOwnerDtoValidator _validator;

    public UpdateOwnerCommandHandler(IUnitOfWork unitOfWork, OwnerMapper mapper, UpdateOwnerDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<OwnerDetailDto?> HandleAsync(UpdateOwnerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command.Data, cancellationToken);

        var owner = await _unitOfWork.Owners.GetByIdAsync(command.Id, cancellationToken);
        if (owner == null)
            return null;

        if (command.Data.Email != owner.Email && await _unitOfWork.Owners.ExistsByEmailAsync(command.Data.Email, command.Id, cancellationToken))
            throw new ValidationException("An owner with this email already exists.");

        if (!string.IsNullOrWhiteSpace(command.Data.ExternalCode) && 
            command.Data.ExternalCode != owner.ExternalCode && 
            await _unitOfWork.Owners.ExistsByExternalCodeAsync(command.Data.ExternalCode, command.Id, cancellationToken))
            throw new ValidationException("An owner with this external code already exists.");

        // Use the domain entity's Update method and reflection for properties it doesn't handle
        var fullName = $"{command.Data.FirstName} {command.Data.LastName}".Trim();
        owner.Update(fullName, command.Data.Email, command.Data.PhoneNumber);

        // Set additional properties via reflection since Owner doesn't expose setters for all properties
        var ownerType = typeof(Owner);
        SetProperty(owner, ownerType, nameof(Owner.BirthDate), command.Data.DateOfBirth);
        SetProperty(owner, ownerType, nameof(Owner.AddressLine1), command.Data.AddressLine1);
        SetProperty(owner, ownerType, nameof(Owner.AddressLine2), command.Data.AddressLine2);
        SetProperty(owner, ownerType, nameof(Owner.City), command.Data.City);
        SetProperty(owner, ownerType, nameof(Owner.State), command.Data.State);
        SetProperty(owner, ownerType, nameof(Owner.PostalCode), command.Data.PostalCode);
        SetProperty(owner, ownerType, nameof(Owner.Country), command.Data.Country);
        SetProperty(owner, ownerType, nameof(Owner.ExternalCode), command.Data.ExternalCode);

        _unitOfWork.Owners.Update(owner);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.ToDetailDto(owner);
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