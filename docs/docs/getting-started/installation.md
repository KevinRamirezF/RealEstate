# Installation Guide

This guide will help you set up the RealEstate API development environment on your local machine.

## Prerequisites

Before you begin, ensure you have the following installed:

### Required Software

- **.NET 8 SDK** - [Download from Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - Local instance or SQL Server Express
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **Git** for version control

### Verify Installation

```bash
# Check .NET version
dotnet --version
# Should show 8.0.x or higher

# Check SQL Server connection
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

## Clone the Repository

```bash
git clone https://github.com/your-org/realestate-api.git
cd realestate-api
```

## Database Setup

### 1. Configure Connection String

Update the connection string in `appsettings.Development.json`:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "Server=localhost;Database=RealEstateDB;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### 2. Run Database Migrations

```bash
# Navigate to the solution root
cd RealEstate

# Apply database migrations
dotnet ef database update --project RealEstate.Infrastructure --startup-project RealEstate.API
```

## Build and Run

### Build the Solution

```bash
# Restore packages and build
dotnet restore
dotnet build
```

### Run the API

```bash
# Start the API server
dotnet run --project RealEstate.API
```

The API will be available at:
- **HTTPS**: `https://localhost:7001`
- **HTTP**: `http://localhost:5000`

### Verify Installation

1. Open your browser and navigate to `https://localhost:7001/swagger`
2. You should see the Swagger UI with API documentation
3. Try the health check endpoint: `GET /health`

## Development Tools

### Recommended Visual Studio Extensions

- **Entity Framework Core Power Tools** - For database visualization
- **REST Client** - For API testing
- **GitLens** - Enhanced Git capabilities

### Recommended VS Code Extensions

- **C# Dev Kit** - Complete C# development experience
- **REST Client** - API testing directly in VS Code
- **Thunder Client** - Alternative API testing tool

## Next Steps

- [Configuration Guide](./configuration) - Configure application settings
- [First Run](./first-run) - Run the application for the first time
- [Development Guide](../development/project-structure) - Understand the project structure

## Troubleshooting

### Common Issues

**Issue**: `dotnet ef` command not found
```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef
```

**Issue**: Database connection fails
- Verify SQL Server is running
- Check connection string format
- Ensure Windows Authentication is enabled (if using Trusted_Connection)

**Issue**: Build errors related to packages
```bash
# Clean and restore packages
dotnet clean
dotnet restore
dotnet build
```

**Issue**: Port already in use
- Check if another application is using ports 5000/7001
- Update ports in `launchSettings.json` if needed

Need more help? Check our [troubleshooting guide](../development/troubleshooting) or open an issue on GitHub.