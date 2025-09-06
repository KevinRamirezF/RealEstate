using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;
using System.Reflection;

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

        // EXACT COPY of PATCH handler pattern
        var property = await _unitOfWork.Properties.GetByIdAsync(command.Id, cancellationToken);
        if (property == null)
            throw new ArgumentException($"Property with ID {command.Id} not found.");

        var oldPrice = property.Price;
        
        // Apply price change using domain method
        property.ChangePrice(command.PriceChange.BasePrice, command.PriceChange.TaxAmount, command.PriceChange.ActorName);
        
        // DISABLE concurrency completely by detaching entity
        var entry = _unitOfWork.Properties.GetEntry(property);
        entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        
        // Attach as modified without any concurrency tracking
        _unitOfWork.Properties.GetDbContext().Set<RealEstate.Domain.Entities.Property>().Attach(property);
        var newEntry = _unitOfWork.Properties.GetEntry(property);
        newEntry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ChangePriceResult
        {
            Id = property.Id,
            OldPrice = oldPrice,
            NewPrice = property.Price,
            TaxAmount = property.TaxAmount,
            ActorName = command.PriceChange.ActorName,
            ChangedAt = property.UpdatedAt,
            RowVersion = property.RowVersion
        };
    }
}