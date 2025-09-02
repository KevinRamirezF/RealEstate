using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Validators;

namespace RealEstate.Application.Queries.Properties;

public class GetPropertiesQueryHandler
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly PropertyFiltersValidator _validator;

    public GetPropertiesQueryHandler(IPropertyRepository propertyRepository, PropertyFiltersValidator validator)
    {
        _propertyRepository = propertyRepository;
        _validator = validator;
    }

    public async Task<GetPropertiesResult> Handle(GetPropertiesQuery query, CancellationToken cancellationToken = default)
    {
        // Validate filters
        var validationResult = await _validator.ValidateAsync(query.Filters, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Get paged properties
        var properties = await _propertyRepository.GetPagedPropertiesAsync(query.Filters, cancellationToken);

        return new GetPropertiesResult
        {
            Properties = properties
        };
    }
}