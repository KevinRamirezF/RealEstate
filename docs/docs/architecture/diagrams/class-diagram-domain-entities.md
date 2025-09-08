# Domain Entities - Class Diagram

This class diagram shows the core domain entities and their relationships in the RealEstate application. The entities follow Domain-Driven Design principles with private setters, factory methods, and rich domain behavior.

## Domain Model Overview

```mermaid
classDiagram
    class Property {
        +Guid Id
        +Guid OwnerId
        +string CodeInternal
        +string Name
        +string Description
        +PropertyType PropertyType
        +short YearBuilt
        +short Bedrooms
        +int Bathrooms
        +short ParkingSpaces
        +int AreaSqft
        +decimal BasePrice
        +decimal TaxAmount
        +decimal Price
        +string Currency
        +string AddressLine
        +string City
        +string State
        +string PostalCode
        +string Country
        +decimal Lat
        +decimal Lng
        +ListingStatus ListingStatus
        +DateOnly ListingDate
        +bool IsFeatured
        +bool IsPublished
        +DateTimeOffset CreatedAt
        +DateTimeOffset UpdatedAt
        +DateTimeOffset DeletedAt
        +byte[] RowVersion
        +IReadOnlyCollection~PropertyImage~ Images
        +IReadOnlyCollection~PropertyTrace~ Traces
        
        +Create(ownerId, codeInternal, name, propertyType, basePrice, taxAmount, addressLine, city, state, postalCode)$ Property
        +ChangePrice(newBasePrice, newTaxAmount, actorName) void
        +AddImage(url, storageProvider, altText, isPrimary, sortOrder) void
        +Update(...) void
        +SoftDelete() void
    }
    
    class Owner {
        +Guid Id
        +string ExternalCode
        +string FullName
        +string Email
        +string Phone
        +string PhotoUrl
        +DateOnly BirthDate
        +string AddressLine
        +string City
        +string State
        +string PostalCode
        +string Country
        +bool IsActive
        +DateTimeOffset CreatedAt
        +DateTimeOffset UpdatedAt
        +DateTimeOffset DeletedAt
        +byte[] RowVersion
        
        +Create(fullName, email, phone, externalCode)$ Owner
        +Update(fullName, email, phone) void
        +SoftDelete() void
    }
    
    class PropertyImage {
        +Guid Id
        +Guid PropertyId
        +string Url
        +StorageProvider StorageProvider
        +string AltText
        +bool IsPrimary
        +short SortOrder
        +bool Enabled
        +string Checksum
        +DateTimeOffset CreatedAt
        +DateTimeOffset UpdatedAt
        +DateTimeOffset DeletedAt
        +int RowVersion
        
        +Create(propertyId, url, storageProvider, altText, isPrimary, sortOrder)$ PropertyImage
        +SetAsPrimary(isPrimary) void
        +UpdateSortOrder(sortOrder) void
        +SetEnabled(enabled) void
        +SoftDelete() void
    }
    
    class PropertyTrace {
        +Guid Id
        +Guid PropertyId
        +TraceEventType EventType
        +DateTimeOffset EventDate
        +string ActorName
        +decimal OldTotalPrice
        +decimal OldPriceBase
        +decimal OldTaxAmount
        +string Notes
        +DateTimeOffset CreatedAt
        
        +Create(propertyId, eventType, notes, oldTotalPrice, oldPriceBase, oldTaxAmount, actorName)$ PropertyTrace
    }
    
    class PropertyType {
        <<enumeration>>
        HOUSE
        CONDO
        TOWNHOUSE
        MULTI_FAMILY
        LAND
        APARTMENT
        OTHER
    }
    
    class ListingStatus {
        <<enumeration>>
        DRAFT
        ACTIVE
        PENDING
        SOLD
        OFF_MARKET
    }
    
    class TraceEventType {
        <<enumeration>>
        CREATED
        UPDATED
        DELETED
        PRICE_CHANGE
        LISTED
        SOLD
        TAX_UPDATE
        NOTE
    }
    
    class StorageProvider {
        <<enumeration>>
        S3
        GCS
        AZURE
        LOCAL
        EXTERNAL
    }

    Property -- Owner : "1"
    Property "1" *-- "0..*" PropertyImage : contains
    Property "1" *-- "0..*" PropertyTrace : contains
    Property -- PropertyType : "has type"
    Property -- ListingStatus : "has status"
    PropertyImage -- StorageProvider : "stored in"
    PropertyTrace -- TraceEventType : "event type"
```

## Key Design Patterns

### Domain-Driven Design (DDD)
- **Aggregate Roots**: `Property` and `Owner` are aggregate roots that ensure consistency boundaries
- **Value Objects**: Enums represent value objects that provide type safety
- **Domain Services**: Business logic is encapsulated within the entities themselves

### Rich Domain Model
- **Private Setters**: All properties have private setters to maintain invariants
- **Factory Methods**: Static `Create()` methods ensure proper entity initialization
- **Domain Methods**: Methods like `ChangePrice()`, `AddImage()`, and `Update()` contain business logic
- **Collections**: Exposed as `IReadOnlyCollection<T>` to prevent external manipulation

### Audit and Soft Delete Pattern
- **Soft Delete**: All entities support soft delete via `DeletedAt` timestamp
- **Audit Trail**: `CreatedAt`, `UpdatedAt`, and `RowVersion` fields track changes
- **Property Trace**: Dedicated audit trail for property changes with detailed history

### Relationships
- **Property-Owner**: One-to-many relationship (Owner can have multiple Properties)
- **Property-PropertyImage**: One-to-many with business rules for primary images
- **Property-PropertyTrace**: One-to-many audit trail with automatic trace creation
- **Enum Dependencies**: Entities reference enums for type safety and consistency

## Business Rules Enforced

1. **Property Creation**: Requires valid owner, positive prices, and proper address information
2. **Price Changes**: Automatically creates audit trail entries with old/new price information
3. **Primary Images**: Only one primary image per property, automatically managed
4. **Soft Deletes**: Entities are never physically deleted, only marked as deleted
5. **Concurrency**: Row versions prevent concurrent update conflicts