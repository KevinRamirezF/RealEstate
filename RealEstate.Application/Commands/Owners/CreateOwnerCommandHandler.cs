using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Commands.Owners;

public class CreateOwnerCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateOwnerDtoValidator _validator;

    public CreateOwnerCommandHandler(IUnitOfWork unitOfWork, CreateOwnerDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<CreateOwnerResult> Handle(CreateOwnerCommand command, CancellationToken cancellationToken = default)
    {
        // Validate input
        var validationResult = await _validator.ValidateAsync(command.Owner, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Check email uniqueness
        if (!string.IsNullOrEmpty(command.Owner.Email))
        {
            var emailExists = await _unitOfWork.Owners.ExistsByEmailAsync(command.Owner.Email, cancellationToken: cancellationToken);
            if (emailExists)
            {
                throw new ArgumentException($"Owner with email '{command.Owner.Email}' already exists.");
            }
        }

        // Check external code uniqueness
        if (!string.IsNullOrEmpty(command.Owner.ExternalCode))
        {
            var codeExists = await _unitOfWork.Owners.ExistsByExternalCodeAsync(command.Owner.ExternalCode, cancellationToken: cancellationToken);
            if (codeExists)
            {
                throw new ArgumentException($"Owner with external code '{command.Owner.ExternalCode}' already exists.");
            }
        }

        // Create owner entity
        var owner = Owner.Create(command.Owner.FullName, command.Owner.Email, command.Owner.Phone, command.Owner.ExternalCode);

        // Set optional properties using reflection (since properties have private setters)
        SetPropertyValue(owner, nameof(Owner.PhotoUrl), command.Owner.PhotoUrl);
        SetPropertyValue(owner, nameof(Owner.BirthDate), command.Owner.BirthDate);
        SetPropertyValue(owner, nameof(Owner.AddressLine1), command.Owner.AddressLine1);
        SetPropertyValue(owner, nameof(Owner.AddressLine2), command.Owner.AddressLine2);
        SetPropertyValue(owner, nameof(Owner.City), command.Owner.City);
        SetPropertyValue(owner, nameof(Owner.State), command.Owner.State);
        SetPropertyValue(owner, nameof(Owner.PostalCode), command.Owner.PostalCode);
        SetPropertyValue(owner, nameof(Owner.Country), command.Owner.Country);

        // Add to repository
        await _unitOfWork.Owners.AddAsync(owner, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOwnerResult
        {
            Id = owner.Id,
            FullName = owner.FullName,
            Email = owner.Email,
            Phone = owner.Phone,
            ExternalCode = owner.ExternalCode,
            CreatedAt = owner.CreatedAt,
            RowVersion = owner.RowVersion
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