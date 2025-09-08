# ADR-005: FluentValidation for Comprehensive Input Validation

## Status
**Accepted** - July 2024

## Context

The RealEstate API requires robust input validation across multiple layers:

- **API Layer**: HTTP request validation for controllers
- **Application Layer**: Command and query parameter validation  
- **Business Rules**: Complex validation involving multiple properties and external data
- **Security Requirements**: Input sanitization and injection attack prevention
- **User Experience**: Detailed, localized error messages for API consumers

Traditional validation approaches include:
- Data Annotations: Limited expressiveness and testability
- Manual validation: Scattered code with inconsistent error handling
- Custom validation attributes: Difficult to maintain and test
- Built-in ASP.NET Core validation: Insufficient for complex business rules

The team needed a solution that provides strong typing, comprehensive validation rules, excellent testability, and clear error messaging.

## Decision

We have implemented **FluentValidation** as our primary validation framework with the following architecture:

### Validation Strategy:

**1. Automatic Validator Registration:**
```csharp
// In Application/DependencyInjection.cs
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddScoped(typeof(IValidator<>), typeof(Validator<>));
```

**2. Request/Command Validation:**
```csharp
public class CreatePropertyRequestValidator : AbstractValidator<CreatePropertyRequest>
{
    public CreatePropertyRequestValidator()
    {
        RuleFor(x => x.Address)
            .NotNull()
            .WithMessage("Property address is required");
            
        RuleFor(x => x.Address.Street)
            .NotEmpty()
            .Length(5, 100)
            .WithMessage("Street address must be between 5 and 100 characters");
            
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Property price must be greater than zero")
            .LessThanOrEqualTo(10_000_000)
            .WithMessage("Property price cannot exceed $10,000,000");
            
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID is required")
            .MustAsync(OwnerExistsAsync)
            .WithMessage("Owner does not exist");
    }
    
    private async Task<bool> OwnerExistsAsync(int ownerId, CancellationToken cancellationToken)
    {
        // Inject repository through constructor for async validation
        return await _ownerRepository.ExistsAsync(ownerId);
    }
}
```

**3. Complex Business Rule Validation:**
```csharp
public class PropertyTransferRequestValidator : AbstractValidator<PropertyTransferRequest>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IOwnerRepository _ownerRepository;
    
    public PropertyTransferRequestValidator(
        IPropertyRepository propertyRepository,
        IOwnerRepository ownerRepository)
    {
        _propertyRepository = propertyRepository;
        _ownerRepository = ownerRepository;
        
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .MustAsync(PropertyExistsAsync)
            .WithMessage("Property does not exist");
            
        RuleFor(x => x.NewOwnerId)
            .NotEmpty()
            .MustAsync(NewOwnerExistsAsync)
            .WithMessage("New owner does not exist");
            
        RuleFor(x => x)
            .MustAsync(ValidateTransferRulesAsync)
            .WithMessage("Property transfer validation failed");
    }
    
    private async Task<bool> ValidateTransferRulesAsync(
        PropertyTransferRequest request, 
        CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        var newOwner = await _ownerRepository.GetByIdAsync(request.NewOwnerId);
        
        // Complex business rules
        if (property.Owner.Id == newOwner.Id)
            return false; // Cannot transfer to same owner
            
        if (!property.IsTransferEligible)
            return false; // Property not eligible for transfer
            
        if (newOwner.HasReachedPropertyLimit)
            return false; // Owner has too many properties
            
        return true;
    }
}
```

**4. Validation Pipeline Integration:**
```csharp
public class CreatePropertyCommand : ICreatePropertyCommand
{
    private readonly IValidator<CreatePropertyRequest> _validator;
    
    public async Task<PropertyDto> ExecuteAsync(CreatePropertyRequest request)
    {
        // Validate input with detailed error information
        var validationResult = await _validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        // Continue with business logic...
    }
}
```

## Consequences

### Positive Consequences:

**Maintainability:**
- Validation rules are centralized in dedicated validator classes
- Fluent API provides expressive and readable validation logic
- Easy to modify validation rules without affecting business logic
- Consistent validation patterns across the entire application
- Separation of concerns between validation and business operations

**Security:**
- Comprehensive input validation prevents injection attacks
- Length limits and format validation protect against buffer overflow attempts
- Custom validation rules can implement security policies
- Async validation enables checking against blacklists or external security services
- Detailed error messages don't expose sensitive system information

**Scalability:**
- Validators are registered as scoped services, supporting dependency injection
- Async validation rules can query databases or external services efficiently  
- Rule conditions can be cached for performance optimization
- Validation logic can be independently optimized without affecting business logic
- Custom validators can implement complex caching strategies

**Developer Experience:**
- IntelliSense support for validation rules and error messages
- Excellent testability with clear arrange-act-assert patterns
- Rich ecosystem of pre-built validation rules
- Easy debugging with clear validation failure reasons
- Consistent error handling across all endpoints

**User Experience:**
- Detailed, user-friendly error messages
- Multiple validation errors returned in single request
- Consistent error response format across all endpoints
- Support for localization and internationalization
- Client applications can provide specific field-level error feedback

### Negative Consequences:

**Performance Overhead:**
- Additional processing time for validation execution
- Async validation rules may introduce latency
- Memory allocation for validation result objects
- Dependency injection resolution overhead for validators

**Development Complexity:**
- Learning curve for FluentValidation API and patterns
- More classes and files to maintain (one validator per request type)
- Complex validation scenarios may require significant setup
- Testing async validation rules requires additional infrastructure

**Runtime Dependencies:**
- Additional NuGet package dependency
- Validators require dependency injection container setup
- Async validation rules need database or service connections
- Error handling becomes more complex with validation exceptions

### Mitigation Strategies:

1. **Performance Optimization:**
   ```csharp
   public class OptimizedPropertyValidator : AbstractValidator<PropertyRequest>
   {
       private readonly IMemoryCache _cache;
       
       public OptimizedPropertyValidator(IMemoryCache cache)
       {
           _cache = cache;
           
           // Cache expensive validation results
           RuleFor(x => x.ZipCode)
               .MustAsync(async (zipCode, cancellation) =>
               {
                   var cacheKey = $"zipcode_valid_{zipCode}";
                   if (_cache.TryGetValue(cacheKey, out bool isValid))
                       return isValid;
                       
                   isValid = await ValidateZipCodeAsync(zipCode);
                   _cache.Set(cacheKey, isValid, TimeSpan.FromHours(1));
                   return isValid;
               });
       }
   }
   ```

2. **Validation Rule Libraries:**
   ```csharp
   public static class CommonValidationRules
   {
       public static IRuleBuilderOptions<T, string> ValidAddress`<T>`(
           this IRuleBuilder<T, string> ruleBuilder)
       {
           return ruleBuilder
               .NotEmpty()
               .Length(5, 200)
               .Matches(@"^[a-zA-Z0-9\s,.-]+$")
               .WithMessage("Address contains invalid characters");
       }
       
       public static IRuleBuilderOptions<T, decimal> ValidCurrency`<T>`(
           this IRuleBuilder<T, decimal> ruleBuilder)
       {
           return ruleBuilder
               .GreaterThan(0)
               .ScalePrecision(2, 10)
               .WithMessage("Amount must be a positive currency value");
       }
   }
   ```

3. **Testing Infrastructure:**
   ```csharp
   [TestFixture]
   public class CreatePropertyRequestValidatorTests
   {
       private CreatePropertyRequestValidator _validator;
       private Mock<IOwnerRepository> _mockOwnerRepository;
       
       [SetUp]
       public void Setup()
       {
           _mockOwnerRepository = new Mock<IOwnerRepository>();
           _validator = new CreatePropertyRequestValidator(_mockOwnerRepository.Object);
       }
       
       [Test]
       public async Task Should_Have_Error_When_Price_Is_Zero()
       {
           var request = new CreatePropertyRequest { Price = 0 };
           var result = await _validator.ValidateAsync(request);
           
           result.IsValid.Should().BeFalse();
           result.Errors.Should().Contain(e => e.PropertyName == nameof(request.Price));
       }
   }
   ```

4. **Global Exception Handling:**
   ```csharp
   public class ValidationExceptionMiddleware
   {
       public async Task InvokeAsync(HttpContext context, RequestDelegate next)
       {
           try
           {
               await next(context);
           }
           catch (ValidationException ex)
           {
               await HandleValidationExceptionAsync(context, ex);
           }
       }
       
       private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
       {
           var response = new ValidationProblemDetails(
               ex.Errors.GroupBy(e => e.PropertyName)
                       .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
           );
           
           context.Response.StatusCode = 400;
           await context.Response.WriteAsync(JsonSerializer.Serialize(response));
       }
   }
   ```

## Alternatives Considered

**Data Annotations:**
- Rejected due to limited expressiveness for complex business rules
- Difficult to test validation logic in isolation
- No built-in support for async validation scenarios
- Poor separation of concerns mixing validation with DTOs

**Manual Validation in Controllers:**
- Rejected due to code duplication across endpoints
- Inconsistent error handling and response formats
- Difficult to maintain as validation rules evolve
- Poor testability and violation of single responsibility principle

**Custom Validation Attributes:**
- Rejected due to complexity of implementing reusable validation logic
- Difficult dependency injection for database-dependent validation
- Limited composability and extensibility
- Poor debugging and error reporting capabilities

## Related Decisions

- [ADR-002: CQRS Light Implementation](./cqrs-light-implementation.md)

## Success Metrics

- **Security**: Zero input validation related security vulnerabilities in production
- **Performance**: Validation overhead under 10ms for 95th percentile of requests
- **Code Quality**: greater than 95% of API inputs validated with comprehensive rule coverage
- **Developer Experience**: New validation rule implementation time under 30 minutes average