using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Infrastructure.Persistence.Queries
{
    public static class PropertyCompiledQueries
    {
        public static readonly Func<RealEstateDbContext, Guid, Task<Property?>> GetPropertyByIdAsync =
            EF.CompileAsyncQuery((RealEstateDbContext context, Guid id) =>
                context.Properties
                    .FirstOrDefault(p => p.Id == id));

        // This query can be expanded with more parameters for filtering, sorting, and pagination.
        public static readonly Func<RealEstateDbContext, IAsyncEnumerable<Property>> ListPropertiesWithFiltersAsync =
            EF.CompileAsyncQuery((RealEstateDbContext context) =>
                context.Properties.AsNoTracking());

        public static readonly Func<RealEstateDbContext, Guid, IAsyncEnumerable<PropertyImage>> GetImagesByPropertyIdAsync =
            EF.CompileAsyncQuery((RealEstateDbContext context, Guid propertyId) =>
                context.PropertyImages
                    .AsNoTracking()
                    .Where(pi => pi.PropertyId == propertyId));
    }
}
