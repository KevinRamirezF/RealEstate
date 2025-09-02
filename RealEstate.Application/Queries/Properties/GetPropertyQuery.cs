using RealEstate.Application.DTOs.Output;

namespace RealEstate.Application.Queries.Properties;

public class GetPropertyQuery
{
    public Guid Id { get; set; }
}

public class GetPropertyResult
{
    public PropertyDetailDto Property { get; set; } = null!;
}