using Microsoft.EntityFrameworkCore.Storage;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Infrastructure.Persistence;
using System.Collections.Concurrent;

namespace RealEstate.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly RealEstateDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(RealEstateDbContext context, IPropertyRepository properties, IOwnerRepository owners)
    {
        _context = context;
        Properties = properties;
        Owners = owners;
    }

    public IPropertyRepository Properties { get; }
    public IOwnerRepository Owners { get; }

    public IRepository<T> Repository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}