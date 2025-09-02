using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;

namespace RealEstate.Application.Queries.Owners;

public class GetOwnersQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OwnerMapper _mapper;

    public GetOwnersQueryHandler(IUnitOfWork unitOfWork, OwnerMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<OwnerListDto>> HandleAsync(GetOwnersQuery query, CancellationToken cancellationToken = default)
    {
        var owners = await _unitOfWork.Owners.GetAllAsync(cancellationToken);
        return owners.Select(_mapper.ToListDto);
    }
}