using FluentValidation;
using RealEstate.Application.DTOs.Input;

namespace RealEstate.Application.Validators;

public class ChangePriceDtoValidator : AbstractValidator<ChangePriceDto>
{
    public ChangePriceDtoValidator()
    {
        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Base Price must be 0 or greater.")
            .LessThanOrEqualTo(999999999999.99m)
            .WithMessage("Base Price cannot exceed $999,999,999,999.99.");

        RuleFor(x => x.TaxAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tax Amount must be 0 or greater.")
            .LessThanOrEqualTo(99999999.99m)
            .WithMessage("Tax Amount cannot exceed $99,999,999.99.");

        RuleFor(x => x.ActorName)
            .MaximumLength(180)
            .WithMessage("Actor Name cannot exceed 180 characters.");

        RuleFor(x => x.RowVersion)
            .GreaterThan(0)
            .WithMessage("Row Version is required for concurrency control.");
    }
}