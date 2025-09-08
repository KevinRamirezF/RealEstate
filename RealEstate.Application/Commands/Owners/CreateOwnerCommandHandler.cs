using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;
using System.Reflection;

namespace RealEstate.Application.Commands.Owners;

public class CreateOwnerCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OwnerMapper _mapper;
    private readonly CreateOwnerDtoValidator _validator;

    public CreateOwnerCommandHandler(IUnitOfWork unitOfWork, OwnerMapper mapper, CreateOwnerDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<OwnerDetailDto> HandleAsync(CreateOwnerCommand command, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(command.Data, cancellationToken);

        if (!string.IsNullOrWhiteSpace(command.Data.Email) && 
            await _unitOfWork.Owners.ExistsByEmailAsync(command.Data.Email, cancellationToken: cancellationToken))
            throw new ValidationException("An owner with this email already exists.");

        if (!string.IsNullOrWhiteSpace(command.Data.ExternalCode) && 
            await _unitOfWork.Owners.ExistsByExternalCodeAsync(command.Data.ExternalCode, cancellationToken: cancellationToken))
            throw new ValidationException("An owner with this external code already exists.");

        var owner = Owner.Create(command.Data.FullName, command.Data.Email, command.Data.Phone, command.Data.ExternalCode);

        // Set additional properties via reflection since Owner doesn't expose setters for all properties
        var ownerType = typeof(Owner);
        SetProperty(owner, ownerType, nameof(Owner.BirthDate), command.Data.BirthDate);
        SetProperty(owner, ownerType, nameof(Owner.AddressLine), command.Data.AddressLine);
        SetProperty(owner, ownerType, nameof(Owner.City), command.Data.City);
        SetProperty(owner, ownerType, nameof(Owner.State), command.Data.State);
        SetProperty(owner, ownerType, nameof(Owner.PostalCode), command.Data.PostalCode);
        SetProperty(owner, ownerType, nameof(Owner.Country), command.Data.Country);

        await _unitOfWork.Owners.AddAsync(owner, cancellationToken);
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