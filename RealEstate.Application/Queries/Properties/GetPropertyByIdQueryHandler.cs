using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;

namespace RealEstate.Application.Queries.Properties;

public class GetPropertyByIdQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PropertyMapper _mapper;

    public GetPropertyByIdQueryHandler(IUnitOfWork unitOfWork, PropertyMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PropertyDetailDto?> HandleAsync(GetPropertyByIdQuery query, CancellationToken cancellationToken = default)
    {
        var property = await _unitOfWork.Properties.GetPropertyDetailAsync(query.Id, cancellationToken);
        return property;
    }
}