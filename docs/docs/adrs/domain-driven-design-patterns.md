# ADR-004: Domain-Driven Design Patterns Implementation

## Status
**Accepted** - July 2024

## Context

The RealEstate application involves complex business rules around property management, ownership, pricing, and transaction history. The domain includes:

- **Property Lifecycle**: Creation, pricing changes, status updates, ownership transfers
- **Business Invariants**: Price validation, ownership constraints, audit trail requirements  
- **Complex Relationships**: Properties, owners, images, traces, and their interdependencies
- **Business Rules**: Property valuation logic, transfer validation, pricing history

Traditional anemic domain models with business logic scattered across services would lead to:
- Difficult maintenance as business rules are spread throughout the application
- Inconsistent rule enforcement across different use cases
- Complex testing scenarios due to external dependencies in business logic
- Poor encapsulation of domain knowledge

## Decision

We have implemented **Domain-Driven Design (DDD) patterns** with rich domain models:

### Core DDD Patterns Applied:

**1. Rich Domain Entities with Encapsulation:**
```csharp
public class Property : Entity
{
    private readonly List<PropertyImage> _images = new();
    private readonly List<PropertyTrace> _traces = new();
    
    // Private constructor - enforce factory method usage
    private Property() { }
    
    // Factory method with business rule validation
    public static Property Create(Address address, decimal price, Owner owner)
    {
        ValidatePrice(price);
        ValidateOwner(owner);
        
        var property = new Property
        {
            Address = address,
            Price = price,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        
        property.AddTrace(PropertyTraceType.Created, $"Property created with initial price {price:C}");
        return property;
    }
    
    // Business methods with invariant enforcement
    public void ChangePrice(decimal newPrice, string reason)
    {
        ValidatePrice(newPrice);
        if (newPrice == Price) return;
        
        var oldPrice = Price;
        Price = newPrice;
        LastModifiedAt = DateTime.UtcNow;
        
        AddTrace(PropertyTraceType.PriceChange, $"Price changed from {oldPrice:C} to {newPrice:C}. Reason: {reason}");
    }
}
```

**2. Value Objects for Complex Data:**
```csharp
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    
    private Address() { } // EF Core constructor
    
    public static Address Create(string street, string city, string state, string zipCode)
    {
        ValidateAddress(street, city, state, zipCode);
        
        return new Address
        {
            Street = street?.Trim(),
            City = city?.Trim(),
            State = state?.Trim().ToUpperInvariant(),
            ZipCode = zipCode?.Trim()
        };
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return ZipCode;
    }
}
```

**3. Domain Services for Multi-Entity Operations:**
```csharp
public class PropertyValuationService
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IMarketDataService _marketDataService;
    
    public async Task<PropertyValuation> CalculateValuationAsync(Property property)
    {
        var comparableProperties = await _propertyRepository
            .GetComparablePropertiesAsync(property.Address, property.Size);
            
        var marketTrends = await _marketDataService
            .GetMarketTrendsAsync(property.Address.City, property.Address.State);
            
        return PropertyValuation.Calculate(property, comparableProperties, marketTrends);
    }
}
```

**4. Aggregate Root Pattern:**
```csharp
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

## Consequences

### Positive Consequences:

**Maintainability:**
- Business rules are centralized within domain entities, making them easy to find and modify
- Rich domain models make code self-documenting through expressive method names
- Factory methods ensure entities are always created in valid states
- Private setters prevent external code from bypassing business rules
- Value objects eliminate primitive obsession and encapsulate validation logic

**Security:**
- Domain invariants are enforced at the entity level, preventing invalid states
- Factory methods validate all inputs before entity creation
- Private constructors prevent bypassing validation through direct instantiation
- Business rule violations are caught early in the domain layer
- Immutable value objects prevent accidental data corruption

**Scalability:**
- Domain events enable loose coupling between aggregates
- Rich domain models reduce database round trips by encapsulating related operations
- Aggregate boundaries help identify potential microservice boundaries
- Domain services can be independently scaled based on business operations
- Clear separation enables caching strategies at appropriate levels

**Testability:**
- Domain logic can be unit tested without external dependencies
- Factory methods simplify test data setup with valid entities
- Business rules are tested through domain methods rather than property manipulation
- Value object equality makes assertions straightforward
- Domain events can be tested independently of infrastructure

**Developer Experience:**
- Expressive domain model makes business logic clear to new developers
- IDE support for navigation and refactoring is excellent with explicit methods
- Business rules are discoverable through entity method signatures
- Compiler catches business rule violations at compile time
- Code reviews focus on business logic rather than technical plumbing

### Negative Consequences:

**Learning Curve:**
- Developers need training on DDD concepts and patterns
- More complex than simple CRUD operations with anemic models
- Requires understanding of aggregate boundaries and consistency rules
- Value object concepts may be unfamiliar to some developers

**Initial Development Overhead:**
- More boilerplate code required for entities and value objects
- Factory methods and validation logic increase initial development time
- Need to design aggregate boundaries carefully upfront
- Domain events infrastructure requires additional setup

**Performance Considerations:**
- Rich domain models may have slight memory overhead compared to anemic models
- Factory method validation adds processing time during entity creation
- Domain event handling introduces additional complexity
- Value object creation and comparison has computational cost

### Mitigation Strategies:

1. **Team Training and Documentation:**
   ```csharp
   // Provide clear examples and templates
   public abstract class Entity
   {
       public int Id { get; protected set; }
       public DateTime CreatedAt { get; protected set; }
       public DateTime? LastModifiedAt { get; protected set; }
       
       protected static void ValidateRequired`<T>`(T value, string parameterName) 
           where T : class
       {
           if (value == null)
               throw new DomainException($"{parameterName} is required");
       }
   }
   ```

2. **Code Generation Tools**: Create templates for common entity patterns

3. **Performance Monitoring**: Track entity creation and method execution times

4. **Gradual Adoption**: Start with core entities and expand DDD patterns incrementally

### Domain Model Guidelines:

**Entity Design Principles:**
- Keep aggregates small and focused on single business concepts
- Use factory methods for complex entity creation logic
- Implement business methods that maintain invariants
- Expose collections as IReadOnlyCollection to prevent external modification
- Use domain exceptions for business rule violations

**Value Object Best Practices:**
- Make value objects immutable after creation
- Implement structural equality through GetEqualityComponents
- Validate all properties during construction
- Use value objects for complex data that has business meaning
- Consider performance implications of frequent value object creation

**Domain Service Usage:**
- Use domain services for operations that don't naturally fit within a single entity
- Keep domain services stateless and focused on business logic
- Inject only repositories and other domain services
- Avoid infrastructure dependencies in domain services

## Alternatives Considered

**Anemic Domain Model with Service Layer:**
- Rejected due to scattered business logic across multiple services
- Poor encapsulation leads to duplicated validation logic
- Difficult to maintain consistency as business rules evolve
- Testing becomes complex with external dependencies

**Active Record Pattern:**
- Rejected due to tight coupling between domain and persistence
- Difficult to unit test business logic without database
- Violates single responsibility principle by mixing concerns
- Makes it harder to evolve persistence strategy independently

**Functional Domain Modeling:**
- Considered but rejected due to team familiarity with object-oriented patterns
- F# would be ideal but not aligned with existing .NET/C# expertise
- Immutable data structures have performance implications for large datasets
- More difficult to integrate with Entity Framework

## Related Decisions

- [ADR-001: Clean Architecture Adoption](./clean-architecture-adoption.md)
- [ADR-003: Entity Framework Core with Repository Pattern](./ef-core-repository-pattern.md)
- [ADR-006: Soft Delete Implementation](./soft-delete-implementation.md)

## Success Metrics

- **Code Quality**: greater than 85% of business logic contained within domain entities
- **Test Coverage**: greater than 90% coverage for domain layer without infrastructure dependencies
- **Defect Rate**: less than 5% of bugs related to business rule violations in production
- **Developer Satisfaction**: Team rates domain model clarity above 4/5