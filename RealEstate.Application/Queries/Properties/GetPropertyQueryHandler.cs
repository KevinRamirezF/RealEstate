using RealEstate.Application.Common.Interfaces;

namespace RealEstate.Application.Queries.Properties;

public class GetPropertyQueryHandler
{
    private readonly IPropertyRepository _propertyRepository;

    public GetPropertyQueryHandler(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<GetPropertyResult?> Handle(GetPropertyQuery query, CancellationToken cancellationToken = default)
    {
        var property = await _propertyRepository.GetPropertyDetailAsync(query.Id, cancellationToken);
        
        if (property == null)
            return null;

        return new GetPropertyResult
        {
            Property = property
        };
    }
}