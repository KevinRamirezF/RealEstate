using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.Mappers;

namespace RealEstate.Application.Queries.Owners;

public class GetOwnerQueryHandler
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly OwnerMapper _mapper;

    public GetOwnerQueryHandler(IOwnerRepository ownerRepository, OwnerMapper mapper)
    {
        _ownerRepository = ownerRepository;
        _mapper = mapper;
    }

    public async Task<GetOwnerResult?> Handle(GetOwnerQuery query, CancellationToken cancellationToken = default)
    {
        var owner = await _ownerRepository.GetByIdAsync(query.Id, cancellationToken);
        
        if (owner == null)
            return null;

        return new GetOwnerResult
        {
            Owner = _mapper.ToDto(owner)
        };
    }
}