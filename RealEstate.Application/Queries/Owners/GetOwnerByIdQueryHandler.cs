using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;

namespace RealEstate.Application.Queries.Owners;

public class GetOwnerByIdQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OwnerMapper _mapper;

    public GetOwnerByIdQueryHandler(IUnitOfWork unitOfWork, OwnerMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OwnerDetailDto?> HandleAsync(GetOwnerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var owner = await _unitOfWork.Owners.GetByIdAsync(query.Id, cancellationToken);
        return owner != null ? _mapper.ToDetailDto(owner) : null;
    }
}