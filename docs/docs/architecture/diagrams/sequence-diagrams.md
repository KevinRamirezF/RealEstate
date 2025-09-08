# Sequence Diagrams

This document contains sequence diagrams for the most important operations in the RealEstate application, showing the flow of data and control through the different layers of the Clean Architecture.

## Property Creation Flow

This diagram shows how a new property is created, following the CQRS pattern and domain-driven design principles.

```mermaid
sequenceDiagram
    participant Client
    participant Controller as PropertiesController
    participant Handler as CreatePropertyHandler
    participant Validator as CreatePropertyValidator
    participant Domain as Property (Domain)
    participant UoW as Unit of Work
    participant Repo as PropertyRepository
    participant DB as Database

    Client->>Controller: POST /api/properties
    Controller->>Validator: Validate(CreatePropertyCommand)
    
    alt Validation Success
        Validator-->>Controller: ValidationResult (Success)
        Controller->>Handler: Handle(CreatePropertyCommand)
        Handler->>Domain: Property.Create(...)
        
        Domain->>Domain: Validate business rules
        Domain->>Domain: Create PropertyTrace (CREATED)
        Domain-->>Handler: Property entity
        
        Handler->>UoW: BeginTransaction()
        Handler->>Repo: Add(property)
        Handler->>UoW: SaveChangesAsync()
        UoW->>DB: INSERT Property, PropertyTrace
        DB-->>UoW: Success
        UoW-->>Handler: Success
        Handler-->>Controller: PropertyResponse
        Controller-->>Client: 201 Created + PropertyResponse
    else Validation Failed
        Validator-->>Controller: ValidationResult (Failed)
        Controller-->>Client: 400 Bad Request + ValidationErrors
    end
```

## Property Price Update Flow

This diagram shows the property price update operation, which includes audit trail creation and optimistic concurrency control.

```mermaid
sequenceDiagram
    participant Client
    participant Controller as PropertiesController
    participant Handler as UpdatePropertyPriceHandler
    participant Validator as UpdatePropertyPriceValidator
    participant UoW as Unit of Work
    participant Repo as PropertyRepository
    participant Domain as Property (Domain)
    participant DB as Database

    Client->>Controller: PUT /api/properties/{id}/price
    Controller->>Validator: Validate(UpdatePropertyPriceCommand)
    
    alt Validation Success
        Validator-->>Controller: ValidationResult (Success)
        Controller->>Handler: Handle(UpdatePropertyPriceCommand)
        Handler->>UoW: BeginTransaction()
        Handler->>Repo: GetByIdAsync(propertyId)
        Repo->>DB: SELECT Property WHERE Id = {id}
        DB-->>Repo: Property entity
        Repo-->>Handler: Property entity
        
        alt Property Found
            Handler->>Domain: property.ChangePrice(newBasePrice, newTaxAmount, actorName)
            Domain->>Domain: Validate new prices
            Domain->>Domain: Update Price fields
            Domain->>Domain: Create PropertyTrace (PRICE_CHANGE)
            Domain-->>Handler: Success
            
            Handler->>UoW: SaveChangesAsync()
            UoW->>DB: UPDATE Property, INSERT PropertyTrace
            
            alt Concurrency Check Success
                DB-->>UoW: Success
                UoW-->>Handler: Success
                Handler-->>Controller: PropertyResponse
                Controller-->>Client: 200 OK + PropertyResponse
            else Concurrency Conflict
                DB-->>UoW: Concurrency Exception
                UoW-->>Handler: Concurrency Exception
                Handler-->>Controller: Concurrency Exception
                Controller-->>Client: 409 Conflict
            end
        else Property Not Found
            Handler-->>Controller: Not Found
            Controller-->>Client: 404 Not Found
        end
    else Validation Failed
        Validator-->>Controller: ValidationResult (Failed)
        Controller-->>Client: 400 Bad Request + ValidationErrors
    end
```

## Property Query with Filtering

This diagram shows how property queries work with filtering, pagination, and caching.

```mermaid
sequenceDiagram
    participant Client
    participant Controller as PropertiesController
    participant Handler as GetPropertiesQueryHandler
    participant Cache as Output Cache
    participant Repo as PropertyRepository
    participant DB as Database

    Client->>Controller: GET /api/properties?city=Seattle&status=ACTIVE
    Controller->>Cache: Check cache with ETag
    
    alt Cache Hit
        Cache-->>Controller: Cached PropertyListResponse + ETag
        Controller-->>Client: 304 Not Modified
    else Cache Miss
        Cache-->>Controller: Cache miss
        Controller->>Handler: Handle(GetPropertiesQuery)
        Handler->>Repo: GetPropertiesAsync(filters, pagination)
        Repo->>DB: SELECT Properties with filters and pagination
        DB-->>Repo: Property collection
        Repo-->>Handler: Property collection
        Handler->>Handler: Map to PropertyListResponse
        Handler-->>Controller: PropertyListResponse
        Controller->>Cache: Store with ETag
        Controller-->>Client: 200 OK + PropertyListResponse + ETag
    end
```

## JWT Authentication Flow

This diagram shows the JWT authentication process used throughout the API.

```mermaid
sequenceDiagram
    participant Client
    participant Controller as AuthController
    participant Handler as LoginHandler
    participant UserManager as Identity UserManager
    participant TokenService as JWT Token Service
    participant DB as Database

    Client->>Controller: POST /api/auth/login
    Controller->>Handler: Handle(LoginCommand)
    Handler->>UserManager: FindByEmailAsync(email)
    UserManager->>DB: SELECT User WHERE Email = {email}
    DB-->>UserManager: User entity
    UserManager-->>Handler: User entity
    
    alt User Found
        Handler->>UserManager: CheckPasswordAsync(user, password)
        UserManager-->>Handler: Password check result
        
        alt Password Valid
            Handler->>TokenService: GenerateAccessToken(user)
            TokenService->>TokenService: Create JWT with claims
            TokenService-->>Handler: JWT Token
            Handler-->>Controller: LoginResponse (token, expires)
            Controller-->>Client: 200 OK + LoginResponse
        else Invalid Password
            Handler-->>Controller: Unauthorized
            Controller-->>Client: 401 Unauthorized
        end
    else User Not Found
        Handler-->>Controller: Unauthorized
        Controller-->>Client: 401 Unauthorized
    end
```

## Soft Delete Operation

This diagram shows how soft delete operations work while maintaining referential integrity.

```mermaid
sequenceDiagram
    participant Client
    participant Controller as PropertiesController
    participant Handler as DeletePropertyHandler
    participant UoW as Unit of Work
    participant Repo as PropertyRepository
    participant Domain as Property (Domain)
    participant DB as Database

    Client->>Controller: DELETE /api/properties/{id}
    Controller->>Handler: Handle(DeletePropertyCommand)
    Handler->>UoW: BeginTransaction()
    Handler->>Repo: GetByIdAsync(propertyId)
    Repo->>DB: SELECT Property WHERE Id = {id} AND DeletedAt IS NULL
    DB-->>Repo: Property entity
    Repo-->>Handler: Property entity
    
    alt Property Found
        Handler->>Domain: property.SoftDelete()
        Domain->>Domain: Set DeletedAt = Now
        Domain->>Domain: Create PropertyTrace (DELETED)
        Domain-->>Handler: Success
        
        Handler->>UoW: SaveChangesAsync()
        UoW->>DB: UPDATE Property SET DeletedAt = {now}, INSERT PropertyTrace
        DB-->>UoW: Success
        UoW-->>Handler: Success
        Handler-->>Controller: Success
        Controller-->>Client: 204 No Content
    else Property Not Found
        Handler-->>Controller: Not Found
        Controller-->>Client: 404 Not Found
    end
```

## Key Patterns Illustrated

### CQRS Light Pattern
- **Commands** flow through validation → handler → domain → repository
- **Queries** go directly from handler → repository, bypassing domain logic
- **Separation** of read and write operations with different optimization strategies

### Domain-Driven Design
- **Rich Domain Models** contain business logic and validation
- **Factory Methods** ensure proper entity creation
- **Domain Events** (traces) are created automatically during state changes

### Clean Architecture
- **Dependency Flow** always points inward toward the domain
- **Controllers** orchestrate but contain no business logic
- **Handlers** coordinate between layers without business rules

### Audit and Concurrency
- **Automatic Audit Trails** created for all significant operations
- **Optimistic Concurrency** using RowVersion for conflict detection
- **Soft Deletes** maintain data integrity and audit history

### Performance Optimizations
- **Output Caching** with ETag support for conditional requests
- **Query Optimization** with proper filtering at the database level
- **Transaction Management** ensures data consistency