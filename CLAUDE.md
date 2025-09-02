# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Architecture

This is a .NET 8 real estate management application built using Clean Architecture principles:

- **RealEstate.API** - Web API layer with ASP.NET Core, Swagger/OpenAPI documentation
- **RealEstate.Application** - Application/business logic layer (currently minimal structure)
- **RealEstate.Domain** - Core domain entities and business rules
- **RealEstate.Infrastructure** - Data access, Identity, Entity Framework configurations
- **RealEstate.UnitTests** - NUnit-based test project with FluentAssertions

The domain uses Entity Framework Core with SQL Server, ASP.NET Core Identity for user management, and follows DDD patterns with aggregate roots and value objects.

## Core Domain Model

- **Property** - Main aggregate root with private setters, factory method `Create()`, and domain methods like `ChangePrice()` and `AddImage()`
- **Owner** - Property owner entity
- **PropertyImage** - Property images with primary image support
- **PropertyTrace** - Audit trail for property changes (price changes, etc.)

Entity configurations are in `RealEstate.Infrastructure/Persistence/Configurations/`.

## Development Commands

### Build and Run
```bash
# Build entire solution
dotnet build

# Run API project
dotnet run --project RealEstate.API

# Build specific project
dotnet build RealEstate.API
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test RealEstate.UnitTests
```

### Database Operations
The project uses Entity Framework Core with SQL Server. Connection string should be configured in `appsettings.json` under `ConnectionStrings:DefaultConnection`.

```bash
# Add migration
dotnet ef migrations add <MigrationName> --project RealEstate.Infrastructure --startup-project RealEstate.API

# Update database
dotnet ef database update --project RealEstate.Infrastructure --startup-project RealEstate.API
```

## Key Implementation Patterns

- Domain entities use private constructors with public factory methods
- Properties have private setters to maintain invariants
- Collections are exposed as `IReadOnlyCollection<T>` with private backing lists
- Entity configurations use Fluent API in separate configuration classes
- Dependency injection configured via extension methods in `DependencyInjection.cs`

## Technology Stack

- **.NET 8** with nullable reference types enabled
- **ASP.NET Core** Web API with JWT Bearer authentication setup
- **Entity Framework Core** with SQL Server provider
- **ASP.NET Core Identity** for user management
- **NUnit** with FluentAssertions for testing
- **Swagger/OpenAPI** for API documentation