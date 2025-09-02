using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Filters;
using RealEstate.Application.DTOs.Output;
using RealEstate.Application.Mappers;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Repositories;

public class PropertyRepository : Repository<Property>, IPropertyRepository
{
    private readonly PropertyMapper _mapper;

    public PropertyRepository(RealEstateDbContext context, PropertyMapper mapper) : base(context)
    {
        _mapper = mapper;
    }

    public async Task<PagedResult<PropertyListDto>> GetPagedPropertiesAsync(PropertyFilters filters, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking()
            .Include(p => p.Images.Where(i => i.IsPrimary))
            .Join(_context.Owners,
                p => p.OwnerId,
                o => o.Id,
                (p, o) => new { Property = p, Owner = o });

        // Apply filters
        query = ApplyFilters(query, filters);

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, filters.Sort, filters.Dir);

        // Apply pagination
        var properties = await query
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(x => new
            {
                Property = x.Property,
                OwnerFullName = x.Owner.FullName,
                PrimaryImageUrl = x.Property.Images.FirstOrDefault(i => i.IsPrimary) != null 
                    ? x.Property.Images.FirstOrDefault(i => i.IsPrimary)!.Url 
                    : null
            })
            .ToListAsync(cancellationToken);

        var items = properties.Select(p =>
        {
            var dto = _mapper.ToListDto(p.Property, p.OwnerFullName);
            dto.PrimaryImageUrl = p.PrimaryImageUrl;
            return dto;
        }).ToList();

        return new PagedResult<PropertyListDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filters.Page,
            PageSize = filters.PageSize
        };
    }

    public async Task<PropertyDetailDto?> GetPropertyDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AsNoTracking()
            .Include(p => p.Images.OrderBy(i => !i.IsPrimary).ThenBy(i => i.SortOrder))
            .Include(p => p.Traces)
            .Join(_context.Owners,
                p => p.OwnerId,
                o => o.Id,
                (p, o) => new { Property = p, Owner = o })
            .FirstOrDefaultAsync(x => x.Property.Id == id, cancellationToken);

        if (result == null)
            return null;

        var lastPriceChange = result.Property.Traces
            .Where(t => t.EventType == TraceEventType.PRICE_CHANGE)
            .OrderByDescending(t => t.EventDate)
            .FirstOrDefault();

        return _mapper.ToDetailDto(result.Property, result.Owner.FullName, lastPriceChange);
    }

    public async Task<bool> ExistsByCodeInternalAsync(string codeInternal, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(p => p.CodeInternal == codeInternal);
        
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Property?> GetPropertyWithImagesAndTracesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Images)
            .Include(p => p.Traces)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<PropertyTrace?> GetLastPriceChangeAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await _context.PropertyTraces.AsNoTracking()
            .Where(t => t.PropertyId == propertyId && t.EventType == TraceEventType.PRICE_CHANGE)
            .OrderByDescending(t => t.EventDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, PropertyFilters filters)
        where T : class
    {
        // Cast to specific anonymous type for property query with join
        var propertyQuery = (IQueryable<dynamic>)query;

        // Text search
        if (!string.IsNullOrEmpty(filters.Q))
        {
            var searchTerm = filters.Q.ToLower();
            query = query.Where(x => 
                EF.Property<string>(EF.Property<Property>(x, "Property"), "Name").ToLower().Contains(searchTerm) ||
                EF.Property<string>(EF.Property<Property>(x, "Property"), "Description").ToLower().Contains(searchTerm) ||
                EF.Property<string>(EF.Property<Property>(x, "Property"), "AddressLine1").ToLower().Contains(searchTerm) ||
                EF.Property<string>(EF.Property<Property>(x, "Property"), "City").ToLower().Contains(searchTerm) ||
                EF.Property<string>(EF.Property<Property>(x, "Property"), "State").ToLower().Contains(searchTerm) ||
                EF.Property<string>(EF.Property<Property>(x, "Property"), "PostalCode").Contains(searchTerm) ||
                EF.Property<string>(EF.Property<Property>(x, "Property"), "CodeInternal").ToLower().Contains(searchTerm));
        }

        // Owner filter
        if (filters.OwnerId.HasValue)
            query = query.Where(x => EF.Property<Guid>(EF.Property<Property>(x, "Property"), "OwnerId") == filters.OwnerId.Value);

        // Price range
        if (filters.MinPrice.HasValue)
            query = query.Where(x => EF.Property<decimal>(EF.Property<Property>(x, "Property"), "Price") >= filters.MinPrice.Value);
        if (filters.MaxPrice.HasValue)
            query = query.Where(x => EF.Property<decimal>(EF.Property<Property>(x, "Property"), "Price") <= filters.MaxPrice.Value);

        // Year range
        if (filters.YearFrom.HasValue)
            query = query.Where(x => EF.Property<short?>(EF.Property<Property>(x, "Property"), "YearBuilt") >= filters.YearFrom.Value);
        if (filters.YearTo.HasValue)
            query = query.Where(x => EF.Property<short?>(EF.Property<Property>(x, "Property"), "YearBuilt") <= filters.YearTo.Value);

        // Property types
        if (filters.PropertyType?.Length > 0)
        {
            var types = filters.PropertyType.Select(t => Enum.Parse<PropertyType>(t)).ToList();
            query = query.Where(x => types.Contains(EF.Property<PropertyType>(EF.Property<Property>(x, "Property"), "PropertyType")));
        }

        // Listing status
        if (filters.ListingStatus?.Length > 0)
        {
            var statuses = filters.ListingStatus.Select(s => Enum.Parse<ListingStatus>(s)).ToList();
            query = query.Where(x => statuses.Contains(EF.Property<ListingStatus>(EF.Property<Property>(x, "Property"), "ListingStatus")));
        }

        // Boolean filters
        if (filters.IsFeatured.HasValue)
            query = query.Where(x => EF.Property<bool>(EF.Property<Property>(x, "Property"), "IsFeatured") == filters.IsFeatured.Value);
        if (filters.IsPublished.HasValue)
            query = query.Where(x => EF.Property<bool>(EF.Property<Property>(x, "Property"), "IsPublished") == filters.IsPublished.Value);

        // Location filters
        if (!string.IsNullOrEmpty(filters.State))
            query = query.Where(x => EF.Property<string>(EF.Property<Property>(x, "Property"), "State") == filters.State);
        if (!string.IsNullOrEmpty(filters.City))
            query = query.Where(x => EF.Property<string>(EF.Property<Property>(x, "Property"), "City") == filters.City);
        if (!string.IsNullOrEmpty(filters.PostalCode))
            query = query.Where(x => EF.Property<string>(EF.Property<Property>(x, "Property"), "PostalCode") == filters.PostalCode);

        return query;
    }

    private static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sort, string direction)
        where T : class
    {
        var isDescending = direction.ToLowerInvariant() == "desc";

        return sort.ToLowerInvariant() switch
        {
            "price" => isDescending 
                ? query.OrderByDescending(x => EF.Property<decimal>(EF.Property<Property>(x, "Property"), "Price"))
                : query.OrderBy(x => EF.Property<decimal>(EF.Property<Property>(x, "Property"), "Price")),
            "listing_date" => isDescending
                ? query.OrderByDescending(x => EF.Property<DateOnly>(EF.Property<Property>(x, "Property"), "ListingDate"))
                : query.OrderBy(x => EF.Property<DateOnly>(EF.Property<Property>(x, "Property"), "ListingDate")),
            "year_built" => isDescending
                ? query.OrderByDescending(x => EF.Property<short?>(EF.Property<Property>(x, "Property"), "YearBuilt"))
                : query.OrderBy(x => EF.Property<short?>(EF.Property<Property>(x, "Property"), "YearBuilt")),
            "area_sqft" => isDescending
                ? query.OrderByDescending(x => EF.Property<int?>(EF.Property<Property>(x, "Property"), "AreaSqft"))
                : query.OrderBy(x => EF.Property<int?>(EF.Property<Property>(x, "Property"), "AreaSqft")),
            _ => isDescending // default: name
                ? query.OrderByDescending(x => EF.Property<string>(EF.Property<Property>(x, "Property"), "Name"))
                : query.OrderBy(x => EF.Property<string>(EF.Property<Property>(x, "Property"), "Name"))
        };
    }
}