using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.DTOs.Output;

namespace RealEstate.Application.Queries.Properties;

public class GetPropertiesQuery
{
    public PropertyFilters Filters { get; set; } = new();
}

public class GetPropertiesResult
{
    public PagedResult<PropertyListDto> Properties { get; set; } = new();
}