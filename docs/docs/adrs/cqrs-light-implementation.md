# ADR-002: CQRS Light Implementation Without MediatR

## Status
**Accepted** - July 2024

## Context

The RealEstate application requires a clear separation between read and write operations to optimize for different access patterns:

- **Write Operations**: Property creation, price updates, ownership transfers require strong consistency and complex business rule validation
- **Read Operations**: Property searches, listings, reports need optimized queries and can tolerate eventual consistency
- **Performance Requirements**: Read operations are expected to be 10x more frequent than writes
- **Team Preferences**: Developers preferred explicit dependencies over reflection-based frameworks

Traditional approaches include full CQRS with event sourcing, MediatR-based CQRS, or simple service layer patterns. The team evaluated complexity vs. benefits for each approach.

## Decision

We have implemented **CQRS Light** pattern without MediatR, featuring:

### Command Side (Write Operations):
```csharp
// Commands for state-changing operations
public interface ICreatePropertyCommand
{
    Task<PropertyDto> ExecuteAsync(CreatePropertyRequest request);
}

// Direct dependency injection in controllers
[ApiController]
public class PropertiesController : ControllerBase
{
    private readonly ICreatePropertyCommand _createPropertyCommand;
    private readonly IUpdatePropertyPriceCommand _updatePriceCommand;
    
    // Explicit dependencies, no magic
}
```

### Query Side (Read Operations):
```csharp
// Queries for data retrieval
public interface IGetPropertiesQuery
{
    Task<IEnumerable<PropertyListDto>> ExecuteAsync(PropertySearchCriteria criteria);
}

// Optimized for specific read scenarios
public interface IGetPropertyStatisticsQuery
{
    Task<PropertyStatisticsDto> ExecuteAsync(StatisticsRequest request);
}
```

### Key Implementation Principles:

1. **No MediatR**: Direct dependency injection for explicit, traceable dependencies
2. **Separate Models**: Different DTOs for commands and queries
3. **Interface Segregation**: Specific interfaces for each operation
4. **Validation at Boundaries**: FluentValidation for command inputs
5. **Repository Abstraction**: Same repositories used by both sides with different methods

## Consequences

### Positive Consequences:

**Maintainability:**
- Explicit dependencies make code easier to follow and debug
- No reflection-based magic reduces complexity and improves IDE support
- Clear separation enables independent optimization of read/write paths
- Interface segregation ensures each handler has minimal dependencies

**Security:**
- Command validation is explicit and easily auditable
- Authorization logic can be applied at handler level
- Input sanitization occurs at well-defined boundaries
- Business rule enforcement is centralized in command handlers

**Scalability:**
- Read and write operations can be scaled independently
- Query handlers can use optimized read models or caching layers
- Command handlers focus on consistency and business rule validation
- Future migration to separate read/write databases is possible

**Performance:**
- Queries can be optimized for specific use cases without affecting commands
- No MediatR overhead (reflection, pipeline traversal)
- Direct method calls provide better performance characteristics
- Caching strategies can be applied at query handler level

**Developer Experience:**
- IntelliSense and code navigation work perfectly with explicit interfaces
- Debugging is straightforward with clear call stacks
- Unit testing is simplified with specific interfaces
- New developers can understand request flow without framework knowledge

### Negative Consequences:

**Code Volume:**
- More interfaces and classes compared to MediatR approach
- Command and query handlers require explicit registration
- Some code duplication in controller constructor injection

**Framework Consistency:**
- Deviates from common .NET patterns using MediatR
- May confuse developers familiar with MediatR-based CQRS
- Different from many online examples and tutorials

**Cross-Cutting Concerns:**
- Logging, caching, and validation must be implemented per handler
- No built-in pipeline for common behaviors
- Authorization logic may be duplicated across handlers

### Mitigation Strategies:

1. **Base Classes**: Implement abstract base classes for common handler patterns
2. **Decorator Pattern**: Use decorators for cross-cutting concerns like logging and caching
3. **Code Generation**: Consider T4 templates or source generators for boilerplate
4. **Documentation**: Maintain clear examples and patterns for new handlers
5. **Consistent Naming**: Establish naming conventions for commands and queries

### Implementation Guidelines:

**Command Handler Pattern:**
```csharp
public class CreatePropertyCommand : ICreatePropertyCommand
{
    private readonly IPropertyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreatePropertyRequest> _validator;
    
    public async Task<PropertyDto> ExecuteAsync(CreatePropertyRequest request)
    {
        // 1. Validate input
        await _validator.ValidateAndThrowAsync(request);
        
        // 2. Execute business logic
        var property = Property.Create(request.Address, request.Price);
        
        // 3. Persist changes
        await _repository.AddAsync(property);
        await _unitOfWork.SaveChangesAsync();
        
        // 4. Return result
        return PropertyDto.FromEntity(property);
    }
}
```

**Query Handler Pattern:**
```csharp
public class GetPropertiesQuery : IGetPropertiesQuery
{
    private readonly IPropertyRepository _repository;
    private readonly IMemoryCache _cache;
    
    public async Task<IEnumerable<PropertyListDto>> ExecuteAsync(PropertySearchCriteria criteria)
    {
        // 1. Check cache
        var cacheKey = $"properties_{criteria.GetHashCode()}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<PropertyListDto> cached))
            return cached;
        
        // 2. Query data
        var properties = await _repository.GetByCriteriaAsync(criteria);
        
        // 3. Transform and cache
        var result = properties.Select(PropertyListDto.FromEntity);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        
        return result;
    }
}
```

## Alternatives Considered

**Full CQRS with Event Sourcing:**
- Rejected due to complexity overhead for current requirements
- Event store infrastructure would add significant operational complexity
- Team lacks expertise in event sourcing patterns
- No clear business requirement for event replay or temporal queries

**MediatR-based CQRS:**
- Rejected due to performance concerns with reflection
- Team preference for explicit dependencies over magic
- Debugging complexity with pipeline behaviors
- Additional learning curve for junior developers

**Simple Service Layer:**
- Rejected as insufficient separation of read/write concerns
- Would not support future scaling requirements
- Mixed responsibilities in single service classes
- Difficult to optimize for different access patterns

## Related Decisions

- [ADR-001: Clean Architecture Adoption](./clean-architecture-adoption.md)
- [ADR-003: Entity Framework Core with Repository Pattern](./ef-core-repository-pattern.md)
- [ADR-005: FluentValidation for Input Validation](./fluentvalidation-input-validation.md)

## Success Metrics

- **Performance**: Command/Query execution times under 100ms/50ms respectively
- **Maintainability**: New feature development time within 20% of baseline
- **Code Quality**: Maintain greater than 80% test coverage for handlers
- **Developer Satisfaction**: Team satisfaction scores above 4/5 for architecture clarity