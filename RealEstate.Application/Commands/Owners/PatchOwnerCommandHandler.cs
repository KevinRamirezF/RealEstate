using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;
using System.Reflection;

namespace RealEstate.Application.Commands.Owners;

public class PatchOwnerCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OwnerMapper _mapper;
    private readonly PatchOwnerDtoValidator _validator;

    public PatchOwnerCommandHandler(IUnitOfWork unitOfWork, OwnerMapper mapper, PatchOwnerDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<OwnerDetailDto?> HandleAsync(PatchOwnerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command.Data, cancellationToken);

        var owner = await _unitOfWork.Owners.GetByIdAsync(command.Id, cancellationToken);
        if (owner == null)
            return null;

        // Check email uniqueness only if email is being updated
        if (command.Data.Email != null && command.Data.Email != owner.Email && 
            await _unitOfWork.Owners.ExistsByEmailAsync(command.Data.Email, command.Id, cancellationToken))
            throw new ValidationException("An owner with this email already exists.");

        // Check external code uniqueness only if external code is being updated
        if (command.Data.ExternalCode != null && 
            command.Data.ExternalCode != owner.ExternalCode && 
            await _unitOfWork.Owners.ExistsByExternalCodeAsync(command.Data.ExternalCode, command.Id, cancellationToken))
            throw new ValidationException("An owner with this external code already exists.");

        // Use the domain entity's Update method only if at least one of the core fields is being updated
        if (command.Data.FirstName != null || command.Data.LastName != null || command.Data.Email != null)
        {
            var firstName = command.Data.FirstName ?? owner.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
            var lastName = command.Data.LastName ?? string.Join(" ", owner.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1)) ?? "";
            var fullName = $"{firstName} {lastName}".Trim();
            var email = command.Data.Email ?? owner.Email;
            var phone = command.Data.PhoneNumber ?? owner.Phone;
            
            owner.Update(fullName, email, phone);
        }
        else if (command.Data.PhoneNumber != null)
        {
            // Update only phone number using domain method
            owner.Update(owner.FullName, owner.Email, command.Data.PhoneNumber);
        }

        // Set additional properties via reflection since Owner doesn't expose setters for all properties
        // Only update properties that are not null (PATCH behavior)
        var ownerType = typeof(Owner);
        
        if (command.Data.DateOfBirth.HasValue)
            SetProperty(owner, ownerType, nameof(Owner.BirthDate), command.Data.DateOfBirth);
        if (command.Data.AddressLine != null)
            SetProperty(owner, ownerType, nameof(Owner.AddressLine), command.Data.AddressLine);
        if (command.Data.City != null)
            SetProperty(owner, ownerType, nameof(Owner.City), command.Data.City);
        if (command.Data.State != null)
            SetProperty(owner, ownerType, nameof(Owner.State), command.Data.State);
        if (command.Data.PostalCode != null)
            SetProperty(owner, ownerType, nameof(Owner.PostalCode), command.Data.PostalCode);
        if (command.Data.Country != null)
            SetProperty(owner, ownerType, nameof(Owner.Country), command.Data.Country);
        if (command.Data.ExternalCode != null)
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