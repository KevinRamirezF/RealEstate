using FluentValidation;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Validators;

namespace RealEstate.Application.Queries.Properties;

public class GetPropertiesQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PropertyFiltersValidator _validator;

    public GetPropertiesQueryHandler(IUnitOfWork unitOfWork, PropertyFiltersValidator validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<PagedResult<PropertyListDto>> HandleAsync(GetPropertiesQuery query, CancellationToken cancellationToken = default)
    {
        await _validator.ValidateAndThrowAsync(query.Filters, cancellationToken);

        return await _unitOfWork.Properties.GetPagedPropertiesAsync(query.Filters, cancellationToken);
    }
}