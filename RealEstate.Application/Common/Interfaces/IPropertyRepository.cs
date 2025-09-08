using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.DTOs.Output;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Interfaces;

public interface IPropertyRepository : IRepository<Property>
{
    Task<PagedResult<PropertyListDto>> GetPagedPropertiesAsync(PropertyFilters filters, CancellationToken cancellationToken = default);
    Task<PropertyDetailDto?> GetPropertyDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeInternalAsync(string codeInternal, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Property?> GetPropertyWithImagesAndTracesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PropertyTrace?> GetLastPriceChangeAsync(Guid propertyId, CancellationToken cancellationToken = default);
    EntityEntry<Property> GetEntry(Property property);
    DbContext GetDbContext();
}