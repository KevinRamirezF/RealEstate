# ADR-003: Entity Framework Core with Repository Pattern and Unit of Work

## Status
**Accepted** - July 2024

## Context

The RealEstate application requires robust data persistence with the following requirements:

- **Complex Queries**: Property search with multiple filters, geographical queries, price ranges
- **Transaction Management**: Property transfers, price updates, and audit trail consistency
- **Performance**: Sub-second response times for property listings and searches
- **Testing**: Ability to unit test business logic without database dependencies
- **Scalability**: Support for read replicas and potential database sharding

The team evaluated direct Entity Framework usage vs. Repository pattern implementation, considering the trade-offs between abstraction benefits and Entity Framework's built-in Unit of Work pattern.

## Decision

We have implemented **Entity Framework Core with Repository Pattern and explicit Unit of Work**, structured as:

### Repository Layer Architecture:
```csharp
// Generic repository interface
public interface IRepository`<TEntity>` where TEntity : class
{
    Task`<TEntity>` GetByIdAsync(int id);
    Task<IEnumerable`<TEntity>`> GetAllAsync();
    Task`<TEntity>` AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// Specific repository for complex business queries
public interface IPropertyRepository : IRepository<Property>
{
    Task<IEnumerable<Property>> GetByOwnerIdAsync(int ownerId);
    Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchCriteria criteria);
    Task<Property> GetWithImagesAsync(int propertyId);
    Task<decimal> GetAveragePriceByLocationAsync(string city, string state);
}
```

### Unit of Work Implementation:
```csharp
public interface IUnitOfWork : IDisposable
{
    IPropertyRepository Properties { get; }
    IOwnerRepository Owners { get; }
    IPropertyTraceRepository PropertyTraces { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### Key Design Decisions:

1. **Repository Over DbSet**: Abstract EF Core behind repositories for testability
2. **Specific Repositories**: Inherit from generic repository for domain-specific queries
3. **Explicit Unit of Work**: Separate from DbContext for transaction control
4. **Query Optimization**: Repository methods optimized for specific use cases
5. **Soft Delete Support**: Global query filters with repository-level soft delete methods

## Consequences

### Positive Consequences:

**Maintainability:**
- Clear separation between business logic and data access implementation
- Repository interfaces define explicit contracts for data operations
- Complex queries are encapsulated in specific repository methods
- Easy to modify data access patterns without affecting business logic
- Database schema changes are isolated to repository implementations

**Security:**
- SQL injection protection through parameterized queries and LINQ
- Authorization logic can be implemented at repository level
- Sensitive data access is controlled through specific repository methods
- Audit trails are automatically maintained through Unit of Work pattern
- Data access permissions can be enforced consistently across repositories

**Scalability:**
- Repository pattern enables caching strategies at data access layer
- Read replicas can be configured at repository implementation level
- Query optimization can be applied per repository without affecting others
- Future migration to different ORMs or databases is possible through interface changes
- Connection pooling and performance tuning isolated to infrastructure layer

**Testability:**
- Business logic can be unit tested with mock repositories
- Repository implementations can be integration tested independently
- Unit of Work pattern enables transaction testing scenarios
- Database-specific logic is isolated and easily testable
- Test data setup is simplified through repository interfaces

**Developer Experience:**
- IntelliSense support for all data operations through strongly-typed interfaces
- Clear documentation of available queries through interface methods
- Consistent patterns for data access across the application
- Easy debugging with explicit method calls rather than dynamic LINQ
- New developers can understand data access patterns quickly

### Negative Consequences:

**Performance Overhead:**
- Additional abstraction layer introduces minor performance cost
- Repository pattern may lead to multiple database round trips
- Generic repository methods may not be optimized for specific scenarios
- Entity tracking overhead in contexts that don't require change tracking

**Code Complexity:**
- More interfaces and classes compared to direct DbContext usage
- Potential over-abstraction for simple CRUD operations
- Unit of Work pattern adds complexity for straightforward scenarios
- Repository registration and dependency injection configuration

**Development Overhead:**
- Need to implement repository methods for each new query pattern
- Additional testing required for repository implementations
- More files to maintain and navigate during development
- Potential inconsistencies between different repository implementations

### Mitigation Strategies:

1. **Performance Optimization**:
   ```csharp
   // Use AsNoTracking for read-only queries
   public async Task<IEnumerable<Property>> GetPropertiesForListingAsync()
   {
       return await _context.Properties
           .AsNoTracking()
           .Where(p => p.IsActive && p.DeletedAt == null)
           .ToListAsync();
   }
   
   // Include related data to avoid N+1 problems
   public async Task<Property> GetWithAllDetailsAsync(int id)
   {
       return await _context.Properties
           .Include(p => p.Owner)
           .Include(p => p.Images)
           .Include(p => p.PropertyTraces)
           .FirstOrDefaultAsync(p => p.Id == id);
   }
   ```

2. **Code Generation**: Use T4 templates for basic repository implementations

3. **Base Repository**: Implement common patterns in abstract base class

4. **Query Splitting**: Configure EF Core for optimal query execution

5. **Connection Resilience**: Implement retry policies for transient failures

### Implementation Patterns:

**Repository Implementation:**
```csharp
public class PropertyRepository : Repository<Property>, IPropertyRepository
{
    public PropertyRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchCriteria criteria)
    {
        var query = _context.Properties.AsQueryable();
        
        if (!string.IsNullOrEmpty(criteria.City))
            query = query.Where(p => p.Address.City.Contains(criteria.City));
            
        if (criteria.MinPrice.HasValue)
            query = query.Where(p => p.Price >= criteria.MinPrice);
            
        if (criteria.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= criteria.MaxPrice);
            
        return await query
            .Include(p => p.Owner)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }
}
```

**Unit of Work Transaction Management:**
```csharp
public async Task<PropertyDto> TransferPropertyAsync(TransferPropertyRequest request)
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId);
        var newOwner = await _unitOfWork.Owners.GetByIdAsync(request.NewOwnerId);
        
        property.ChangeOwner(newOwner);
        property.AddTrace(PropertyTraceType.OwnershipTransfer, request.TransferReason);
        
        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
        
        return PropertyDto.FromEntity(property);
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
```

## Alternatives Considered

**Direct Entity Framework Usage:**
- Rejected due to tight coupling between business logic and EF Core
- Difficult to unit test business logic without database
- Complex transaction management across multiple aggregate roots
- No clear separation of data access concerns

**Dapper with Manual SQL:**
- Rejected due to increased maintenance overhead for complex queries
- Loss of compile-time safety for database schema changes
- No built-in change tracking for complex entity updates
- Additional effort required for relationship mapping

**Generic Repository Only:**
- Rejected as insufficient for complex domain queries
- Would require business logic to construct complex LINQ expressions
- No encapsulation of query optimization strategies
- Difficult to implement domain-specific caching

## Related Decisions

- [ADR-001: Clean Architecture Adoption](./clean-architecture-adoption.md)
- [ADR-002: CQRS Light Implementation](./cqrs-light-implementation.md)
- [ADR-004: Domain-Driven Design Patterns](./domain-driven-design-patterns.md)

## Success Metrics

- **Performance**: Database queries under 100ms for 95th percentile
- **Test Coverage**: greater than 90% coverage for repository implementations
- **Code Quality**: Zero direct DbContext usage in business logic layer
- **Developer Productivity**: New query implementation time under 2 hours average