# ADR-006: Soft Delete Implementation with Global Query Filters

## Status
**Accepted** - July 2024

## Context

The RealEstate application requires data retention and audit capabilities for regulatory compliance and business operations:

- **Regulatory Requirements**: Property and ownership records must be retained for 7+ years
- **Audit Trails**: Need to track when and why records were deleted
- **Data Recovery**: Business users occasionally need to restore accidentally deleted records
- **Referential Integrity**: Deleted properties should not break foreign key relationships
- **Performance**: Query performance should not be significantly impacted by deleted records
- **User Experience**: Deleted records should be invisible to normal business operations

Traditional hard delete approaches would violate regulatory requirements and make audit trails impossible. However, naive soft delete implementations often lead to inconsistent query behavior and application bugs.

## Decision

We have implemented **comprehensive soft delete with Entity Framework Global Query Filters**:

### Soft Delete Architecture:

**1. Soft Delete Interface:**
```csharp
public interface ISoftDeletable
{
    DateTime? DeletedAt { get; }
    string DeletedBy { get; }
    string DeletionReason { get; }
    
    void SoftDelete(string deletedBy, string reason = null);
    void Restore(string restoredBy, string reason = null);
}
```

**2. Base Entity Implementation:**
```csharp
public abstract class Entity : ISoftDeletable
{
    public int Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; protected set; }
    
    // Soft delete properties
    public DateTime? DeletedAt { get; private set; }
    public string DeletedBy { get; private set; }
    public string DeletionReason { get; private set; }
    
    public bool IsDeleted => DeletedAt.HasValue;
    
    public virtual void SoftDelete(string deletedBy, string reason = null)
    {
        if (IsDeleted) return; // Already deleted
        
        ValidateDeletedBy(deletedBy);
        
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        DeletionReason = reason ?? "No reason provided";
        LastModifiedAt = DateTime.UtcNow;
    }
    
    public virtual void Restore(string restoredBy, string reason = null)
    {
        if (!IsDeleted) return; // Not deleted
        
        ValidateDeletedBy(restoredBy);
        
        DeletedAt = null;
        DeletedBy = null;
        DeletionReason = null;
        LastModifiedAt = DateTime.UtcNow;
        
        // Add audit trail for restoration
        AddDomainEvent(new EntityRestoredEvent(GetType().Name, Id, restoredBy, reason));
    }
}
```

**3. Global Query Filters:**
```csharp
public class ApplicationDbContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply soft delete filter to all entities implementing ISoftDeletable
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var method = SetGlobalQueryMethod.MakeGenericMethod(entityType.ClrType);
                method.Invoke(this, new object[] { modelBuilder });
            }
        }
        
        base.OnModelCreating(modelBuilder);
    }
    
    private static readonly MethodInfo SetGlobalQueryMethod = typeof(ApplicationDbContext)
        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
        .Single(t => t.IsGenericMethodDefinition && t.Name == nameof(SetGlobalQuery));
        
    private void SetGlobalQuery`<T>`(ModelBuilder modelBuilder) where T : class, ISoftDeletable
    {
        modelBuilder.Entity`<T>`().HasQueryFilter(e => e.DeletedAt == null);
    }
}
```

**4. Repository Soft Delete Methods:**
```csharp
public class Repository`<TEntity>` : IRepository`<TEntity>` where TEntity : Entity
{
    protected readonly ApplicationDbContext _context;
    
    public async Task SoftDeleteAsync(int id, string deletedBy, string reason = null)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            throw new EntityNotFoundException($"{typeof(TEntity).Name} with ID {id} not found");
            
        entity.SoftDelete(deletedBy, reason);
        await _context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable`<TEntity>`> GetDeletedAsync()
    {
        return await _context.Set`<TEntity>`()
            .IgnoreQueryFilters()
            .Where(e => e.DeletedAt != null)
            .ToListAsync();
    }
    
    public async Task`<TEntity>` GetByIdIncludingDeletedAsync(int id)
    {
        return await _context.Set`<TEntity>`()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
```

**5. Business Layer Integration:**
```csharp
public class DeletePropertyCommand : IDeletePropertyCommand
{
    private readonly IPropertyRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task ExecuteAsync(DeletePropertyRequest request)
    {
        var currentUser = _currentUserService.GetCurrentUser();
        
        await _repository.SoftDeleteAsync(
            request.PropertyId, 
            currentUser.Username, 
            request.DeletionReason);
            
        await _unitOfWork.SaveChangesAsync();
        
        // Publish domain event for audit trail
        await _eventPublisher.PublishAsync(new PropertyDeletedEvent(
            request.PropertyId, 
            currentUser.Username, 
            request.DeletionReason));
    }
}
```

## Consequences

### Positive Consequences:

**Maintainability:**
- Global query filters ensure consistent behavior across all queries automatically
- Centralized soft delete logic in base entity reduces code duplication
- Clear separation between business deletion and data recovery operations
- Standard patterns make it easy for developers to understand and implement
- Repository methods provide explicit control when deleted data access is needed

**Security:**
- Deleted data remains in database for security audit and forensic analysis
- Audit trail captures who deleted records and when, supporting accountability
- Accidental data loss is prevented through reversible deletion process
- Sensitive data can be properly archived rather than immediately destroyed
- Access to deleted records can be controlled through specific repository methods

**Scalability:**
- Database indexes remain effective as deleted records are filtered at query level
- Global query filters are applied at SQL generation time, maintaining performance
- Deleted records can be archived to separate tables for long-term storage
- Query performance is not significantly impacted by soft-deleted records
- Background processes can clean up very old soft-deleted records as needed

**Regulatory Compliance:**
- Complete audit trail satisfies regulatory requirements for data retention
- Records can be retained for required periods before permanent deletion
- Data restoration capabilities support compliance with data recovery requirements
- Deleted data timestamps enable automated retention policy enforcement
- Comprehensive logging supports regulatory audit processes

**Business Continuity:**
- Accidental deletions can be quickly recovered without data loss
- Business users can restore records through application interfaces
- Historical analysis can include deleted records when appropriate
- Data migration and system changes are safer with reversible deletions
- Support team can investigate issues using complete historical data

### Negative Consequences:

**Storage Overhead:**
- Database size increases as deleted records are retained
- Indexes need to account for additional records, slightly impacting performance
- Backup and restore operations handle larger datasets
- Database maintenance operations process more records

**Query Complexity:**
- Developers must understand when to use IgnoreQueryFilters for specific scenarios
- Complex queries involving soft-deleted related entities require careful consideration
- Relationship navigation can be confusing when related entities are soft-deleted
- Troubleshooting query behavior requires understanding of global filters

**Application Complexity:**
- Additional business logic required for restoration and permanent deletion
- User interface needs to handle soft-deleted state and restoration options
- Permission system must account for deletion and restoration operations
- Testing scenarios must cover both soft and hard delete cases

### Mitigation Strategies:

1. **Performance Optimization:**
   ```sql
   -- Create indexes that support soft delete queries
   CREATE INDEX IX_Properties_DeletedAt_Active 
   ON Properties (DeletedAt) 
   WHERE DeletedAt IS NULL;
   
   -- Partition tables by deletion status for large datasets
   CREATE PARTITION SCHEME PS_SoftDelete AS PARTITION PF_SoftDelete
   TO ([Active], [Deleted]);
   ```

2. **Data Archival Strategy:**
   ```csharp
   public class DataArchivalService
   {
       public async Task ArchiveOldDeletedRecordsAsync()
       {
           var cutoffDate = DateTime.UtcNow.AddYears(-7);
           
           var oldDeletedProperties = await _context.Properties
               .IgnoreQueryFilters()
               .Where(p => p.DeletedAt < cutoffDate)
               .ToListAsync();
               
           // Move to archive table
           await _archiveContext.ArchivedProperties.AddRangeAsync(
               oldDeletedProperties.Select(p => ArchivedProperty.FromProperty(p)));
               
           // Hard delete from main table
           _context.Properties.RemoveRange(oldDeletedProperties);
           
           await _context.SaveChangesAsync();
           await _archiveContext.SaveChangesAsync();
       }
   }
   ```

3. **Developer Guidelines:**
   ```csharp
   public static class SoftDeleteExtensions
   {
       public static IQueryable`<T>` IncludeDeleted`<T>`(this IQueryable`<T>` query) 
           where T : class, ISoftDeletable
       {
           return query.IgnoreQueryFilters();
       }
       
       public static IQueryable`<T>` OnlyDeleted`<T>`(this IQueryable`<T>` query) 
           where T : class, ISoftDeletable
       {
           return query.IgnoreQueryFilters().Where(e => e.DeletedAt != null);
       }
   }
   ```

4. **Monitoring and Maintenance:**
   ```csharp
   public class SoftDeleteMetricsService
   {
       public async Task<SoftDeleteMetrics> GetMetricsAsync()
       {
           var metrics = new SoftDeleteMetrics();
           
           metrics.ActivePropertyCount = await _context.Properties.CountAsync();
           metrics.DeletedPropertyCount = await _context.Properties
               .IgnoreQueryFilters()
               .CountAsync(p => p.DeletedAt != null);
               
           metrics.StorageOverheadPercentage = 
               (double)metrics.DeletedPropertyCount / 
               (metrics.ActivePropertyCount + metrics.DeletedPropertyCount) * 100;
               
           return metrics;
       }
   }
   ```

## Alternatives Considered

**Hard Delete with Archive Tables:**
- Rejected due to complexity of maintaining separate archive schema
- Difficult to maintain referential integrity across active and archive tables  
- Complex queries when historical data analysis is needed
- Higher risk of data loss during archive migration processes

**Soft Delete Without Global Filters:**
- Rejected due to high risk of accidentally including deleted records in queries
- Inconsistent behavior across different parts of the application
- Requires manual filtering in every query, leading to bugs and maintenance issues
- Poor developer experience with easy-to-forget query modifications

**Event Sourcing for Audit Trail:**
- Rejected as over-engineered for current requirements
- Significant complexity overhead for relatively simple audit needs
- Requires expertise in event sourcing patterns that team lacks
- Event store infrastructure and query complexity not justified by benefits

## Related Decisions

- [ADR-003: Entity Framework Core with Repository Pattern](./ef-core-repository-pattern.md)
- [ADR-004: Domain-Driven Design Patterns](./domain-driven-design-patterns.md)

## Success Metrics

- **Data Integrity**: Zero instances of hard-deleted data that should have been soft-deleted
- **Performance**: less than 5% query performance impact from global filters
- **Compliance**: 100% audit trail coverage for all deletion operations
- **Recovery**: under 1 hour average time to restore accidentally deleted records