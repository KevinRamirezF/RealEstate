using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Infrastructure.Repositories;

public class OwnerRepository : Repository<Owner>, IOwnerRepository
{
    public OwnerRepository(RealEstateDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(o => o.Email == email);
        
        if (excludeId.HasValue)
            query = query.Where(o => o.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsByExternalCodeAsync(string externalCode, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(o => o.ExternalCode == externalCode);
        
        if (excludeId.HasValue)
            query = query.Where(o => o.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Owner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<Owner?> GetByExternalCodeAsync(string externalCode, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(o => o.ExternalCode == externalCode, cancellationToken);
    }
}