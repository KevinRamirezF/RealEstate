using RealEstate.Application.DTOs.Output;

namespace RealEstate.Application.Queries.Owners;

public class GetOwnerQuery
{
    public Guid Id { get; set; }
}

public class GetOwnerResult
{
    public OwnerDto Owner { get; set; } = null!;
}