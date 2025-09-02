using RealEstate.Domain.Entities;

namespace RealEstate.Application.Common.Interfaces;

public interface IOwnerRepository : IRepository<Owner>
{
    Task<bool> ExistsByEmailAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByExternalCodeAsync(string externalCode, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<Owner?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Owner?> GetByExternalCodeAsync(string externalCode, CancellationToken cancellationToken = default);
}