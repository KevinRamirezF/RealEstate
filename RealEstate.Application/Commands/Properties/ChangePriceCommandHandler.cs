using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;

namespace RealEstate.Application.Commands.Properties;

public class ChangePriceCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ChangePriceDtoValidator _validator;

    public ChangePriceCommandHandler(IUnitOfWork unitOfWork, ChangePriceDtoValidator validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<ChangePriceResult> Handle(ChangePriceCommand command, CancellationToken cancellationToken = default)
    {
        // Validate input
        var validationResult = await _validator.ValidateAsync(command.PriceChange, cancellationToken);
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
        if (property.RowVersion != command.PriceChange.RowVersion)
        {
            throw new DbUpdateConcurrencyException("The property has been modified by another user. Please refresh and try again.");
        }

        var oldPrice = property.Price;

        // Change price using domain method (creates PRICE_CHANGE trace)
        property.ChangePrice(command.PriceChange.NewPrice, command.PriceChange.TaxAmount, command.PriceChange.ActorName);

        // Update in repository
        _unitOfWork.Properties.Update(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangePriceResult
        {
            Id = property.Id,
            OldPrice = oldPrice,
            NewPrice = property.Price,
            TaxAmount = command.PriceChange.TaxAmount,
            ActorName = command.PriceChange.ActorName,
            ChangedAt = property.UpdatedAt,
            RowVersion = property.RowVersion
        };
    }
}