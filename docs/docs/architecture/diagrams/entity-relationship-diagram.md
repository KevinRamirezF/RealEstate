# Entity-Relationship Diagram

This document shows the database schema and relationships for the RealEstate application. The schema follows Domain-Driven Design principles with proper normalization and audit trail support.

## Database Schema Overview

```mermaid
erDiagram
    Owners {
        uniqueidentifier Id PK "Primary Key"
        nvarchar_50 ExternalCode "External system reference"
        nvarchar_180 FullName "NOT NULL, Owner full name"
        nvarchar_254 Email "Email address"
        nvarchar_20 Phone "Phone number"
        nvarchar_1000 PhotoUrl "Profile photo URL"
        date BirthDate "Date of birth"
        nvarchar_500 AddressLine "Street address"
        nvarchar_100 City "City name"
        nvarchar_100 State "State/Province"
        nvarchar_20 PostalCode "Postal/ZIP code"
        nvarchar_50 Country "Default: US"
        bit IsActive "Default: 1"
        datetimeoffset CreatedAt "NOT NULL, Creation timestamp"
        datetimeoffset UpdatedAt "NOT NULL, Last update timestamp"
        datetimeoffset DeletedAt "Soft delete timestamp"
        rowversion RowVersion "Concurrency control"
    }

    Properties {
        uniqueidentifier Id PK "Primary Key"
        uniqueidentifier OwnerId FK "NOT NULL, Foreign key to Owners"
        nvarchar_40 CodeInternal "NOT NULL, Unique internal code"
        nvarchar_200 Name "NOT NULL, Property name"
        nvarchar_2000 Description "Property description"
        nvarchar_20 PropertyType "NOT NULL, Enum as string"
        smallint YearBuilt "Year of construction"
        smallint Bedrooms "Number of bedrooms, Default: 0"
        int Bathrooms "Number of bathrooms, Default: 0"
        smallint ParkingSpaces "Number of parking spaces, Default: 0"
        int AreaSqft "Area in square feet"
        decimal_18_2 BasePrice "NOT NULL, Base price before tax"
        decimal_18_2 TaxAmount "NOT NULL, Tax amount"
        decimal_18_2 Price "NOT NULL, Total price (computed)"
        nvarchar_10 Currency "NOT NULL, Default: USD"
        nvarchar_500 AddressLine "NOT NULL, Street address"
        nvarchar_100 City "NOT NULL, City name"
        nvarchar_100 State "NOT NULL, State/Province"
        nvarchar_20 PostalCode "NOT NULL, Postal/ZIP code"
        nvarchar_50 Country "NOT NULL, Default: US"
        decimal_10_8 Lat "Latitude coordinate"
        decimal_11_8 Lng "Longitude coordinate"
        nvarchar_20 ListingStatus "NOT NULL, Enum as string, Default: ACTIVE"
        date ListingDate "NOT NULL, Date property was listed"
        bit IsFeatured "Default: 0, Featured property flag"
        bit IsPublished "Default: 1, Published status"
        datetimeoffset CreatedAt "NOT NULL, Creation timestamp"
        datetimeoffset UpdatedAt "NOT NULL, Last update timestamp"
        datetimeoffset DeletedAt "Soft delete timestamp"
        rowversion RowVersion "Concurrency control"
    }

    PropertyImages {
        uniqueidentifier Id PK "Primary Key"
        uniqueidentifier PropertyId FK "NOT NULL, Foreign key to Properties"
        nvarchar_1000 Url "NOT NULL, Image URL"
        nvarchar_20 StorageProvider "NOT NULL, Enum as string, Default: S3"
        nvarchar_200 AltText "Alternative text for accessibility"
        bit IsPrimary "Default: 0, Primary image flag"
        smallint SortOrder "Default: 0, Display order"
        bit Enabled "Default: 1, Image enabled status"
        nvarchar_64 Checksum "File checksum for integrity"
        datetimeoffset CreatedAt "NOT NULL, Creation timestamp"
        datetimeoffset UpdatedAt "NOT NULL, Last update timestamp"
        datetimeoffset DeletedAt "Soft delete timestamp"
        int RowVersion "Concurrency control, Default: 1"
    }

    PropertyTraces {
        uniqueidentifier Id PK "Primary Key"
        uniqueidentifier PropertyId FK "NOT NULL, Foreign key to Properties"
        nvarchar_20 EventType "NOT NULL, Enum as string"
        datetimeoffset EventDate "NOT NULL, When event occurred"
        nvarchar_180 ActorName "Who performed the action"
        decimal_18_2 OldTotalPrice "Previous total price"
        decimal_18_2 OldPriceBase "Previous base price"
        decimal_18_2 OldTaxAmount "Previous tax amount"
        nvarchar_2000 Notes "Event description/notes"
        datetimeoffset CreatedAt "NOT NULL, Creation timestamp"
    }

    AspNetUsers {
        nvarchar_450 Id PK "Identity User ID"
        nvarchar_256 UserName "Username"
        nvarchar_256 Email "Email address"
        bit EmailConfirmed "Email confirmation status"
        nvarchar_MAX PasswordHash "Hashed password"
        nvarchar_MAX SecurityStamp "Security stamp"
        nvarchar_MAX ConcurrencyStamp "Concurrency stamp"
        nvarchar_50 PhoneNumber "Phone number"
        bit PhoneNumberConfirmed "Phone confirmation status"
        bit TwoFactorEnabled "2FA enabled flag"
        datetimeoffset LockoutEnd "Account lockout end time"
        bit LockoutEnabled "Lockout enabled flag"
        int AccessFailedCount "Failed login attempts"
    }

    AspNetRoles {
        nvarchar_450 Id PK "Role ID"
        nvarchar_256 Name "Role name"
        nvarchar_256 NormalizedName "Normalized role name"
        nvarchar_MAX ConcurrencyStamp "Concurrency stamp"
    }

    AspNetUserRoles {
        nvarchar_450 UserId PK,FK "Foreign key to AspNetUsers"
        nvarchar_450 RoleId PK,FK "Foreign key to AspNetRoles"
    }

    %% Relationships
    Owners ||--o{ Properties : "owns"
    Properties ||--o{ PropertyImages : "has"
    Properties ||--o{ PropertyTraces : "tracked by"
    AspNetUsers ||--o{ AspNetUserRoles : "has"
    AspNetRoles ||--o{ AspNetUserRoles : "assigned to"
```

## Key Database Features

### Relationships and Constraints

#### Primary Relationships
- **Owners → Properties**: One-to-Many (One owner can have multiple properties)
- **Properties → PropertyImages**: One-to-Many (One property can have multiple images)
- **Properties → PropertyTraces**: One-to-Many (One property has multiple audit trail entries)

#### Foreign Key Constraints
- `Properties.OwnerId` → `Owners.Id` (NOT NULL, with cascade rules)
- `PropertyImages.PropertyId` → `Properties.Id` (NOT NULL, cascade delete)
- `PropertyTraces.PropertyId` → `Properties.Id` (NOT NULL, cascade delete)

### Indexing Strategy

```sql
-- Primary Keys (Clustered Indexes)
PK_Owners (Id)
PK_Properties (Id)
PK_PropertyImages (Id)
PK_PropertyTraces (Id)

-- Foreign Key Indexes
IX_Properties_OwnerId (OwnerId)
IX_PropertyImages_PropertyId (PropertyId)
IX_PropertyTraces_PropertyId (PropertyId)

-- Business Logic Indexes
IX_Properties_CodeInternal (CodeInternal) -- UNIQUE
IX_Properties_ListingStatus_IsPublished (ListingStatus, IsPublished)
IX_Properties_City_State (City, State)
IX_PropertyImages_PropertyId_IsPrimary (PropertyId, IsPrimary)
IX_PropertyTraces_PropertyId_EventDate (PropertyId, EventDate DESC)

-- Soft Delete Support
IX_Owners_DeletedAt (DeletedAt) -- For filtering active records
IX_Properties_DeletedAt (DeletedAt) -- For filtering active records
IX_PropertyImages_DeletedAt (DeletedAt) -- For filtering active records
```

### Data Types and Constraints

#### Precision Specifications
- **Decimal Fields**: `decimal(18,2)` for monetary values ensuring precision
- **Geographic Coordinates**: `decimal(10,8)` for latitude, `decimal(11,8)` for longitude
- **Unicode Support**: `nvarchar` for all text fields supporting international characters

#### Business Rule Constraints
```sql
-- Price constraints
CHECK (BasePrice >= 0)
CHECK (TaxAmount >= 0)
CHECK (Price = BasePrice + TaxAmount)

-- Enum constraints
CHECK (PropertyType IN ('HOUSE', 'CONDO', 'TOWNHOUSE', 'MULTI_FAMILY', 'LAND', 'APARTMENT', 'OTHER'))
CHECK (ListingStatus IN ('DRAFT', 'ACTIVE', 'PENDING', 'SOLD', 'OFF_MARKET'))
CHECK (StorageProvider IN ('S3', 'GCS', 'AZURE', 'LOCAL', 'EXTERNAL'))
CHECK (EventType IN ('CREATED', 'UPDATED', 'DELETED', 'PRICE_CHANGE', 'LISTED', 'SOLD', 'TAX_UPDATE', 'NOTE'))

-- Coordinate constraints
CHECK (Lat >= -90 AND Lat <= 90)
CHECK (Lng >= -180 AND Lng <= 180)

-- Image constraints
CHECK (SortOrder >= 0)
```

### Audit Trail Design

#### Soft Delete Pattern
All main entities support soft delete:
- `DeletedAt` timestamp field (NULL = active, NOT NULL = deleted)
- Global query filters automatically exclude deleted records
- Audit trail preserved even after soft deletion

#### Concurrency Control
- **Properties/Owners**: `rowversion` (SQL Server timestamp) for optimistic concurrency
- **PropertyImages**: `int RowVersion` manually incremented
- **PropertyTraces**: Immutable records (no concurrency control needed)

#### Audit Trail Features
- **Automatic Tracking**: Created via domain events during entity operations
- **Price History**: Detailed tracking of price changes with old/new values
- **Actor Tracking**: Optional actor name for identifying who made changes
- **Event Types**: Comprehensive event type enumeration for different operations

### Performance Optimizations

#### Query Patterns
- **Active Records**: Global query filters for soft delete (WHERE DeletedAt IS NULL)
- **Property Listing**: Composite indexes on status and publication flags
- **Geographic Queries**: Spatial indexes on Lat/Lng for location-based searches
- **Image Loading**: Primary image queries optimized with specific indexing

#### Caching Strategy
- **Output Caching**: ETag-based caching for GET operations
- **Query Result Caching**: Distributed caching for expensive aggregations
- **Connection Pooling**: Optimized connection management for high throughput

### Security Considerations

#### Identity Integration
- **ASP.NET Core Identity**: Full integration with user management
- **Role-Based Access**: Support for role-based authorization
- **JWT Authentication**: Stateless authentication for API access

#### Data Protection
- **No PII in Traces**: Sensitive data excluded from audit trails
- **Encrypted Connections**: All database connections use SSL/TLS
- **Parameter Queries**: Protection against SQL injection attacks