# Configuration Guide

Learn how to configure the RealEstate API for different environments and customize settings.

## Configuration Overview

The application uses the standard .NET configuration system with `appsettings.json` files for different environments.

### Configuration Files

```
├── appsettings.json              # Base configuration
├── appsettings.Development.json  # Development overrides
├── appsettings.Production.json   # Production overrides
└── appsettings.Staging.json      # Staging overrides (optional)
```

## Database Configuration

### Connection String

Configure your database connection in `appsettings.json`:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "Server=localhost;Database=RealEstateDB;Trusted_Connection=true;TrustServerCertificate=true",
    "CommandTimeout": 30,
    "EnableSensitiveDataLogging": false,
    "EnableDetailedErrors": false
  }
}
```

### Database Settings Explained

| Setting | Description | Default |
|---------|-------------|---------|
| `ConnectionString` | SQL Server connection string | Required |
| `CommandTimeout` | Command timeout in seconds | 30 |
| `EnableSensitiveDataLogging` | Log parameter values (dev only) | false |
| `EnableDetailedErrors` | Show detailed EF errors (dev only) | false |

### Environment-Specific Overrides

**Development** (`appsettings.Development.json`):
```json
{
  "DatabaseSettings": {
    "EnableSensitiveDataLogging": true,
    "EnableDetailedErrors": true
  }
}
```

**Production** (`appsettings.Production.json`):
```json
{
  "DatabaseSettings": {
    "ConnectionString": "Server=prod-server;Database=RealEstate;User Id=api-user;Password=***;Encrypt=true;"
  }
}
```

## JWT Authentication Settings

Configure JWT token settings:

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secure-secret-key-here",
    "Issuer": "RealEstateAPI",
    "Audience": "RealEstateClients", 
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### JWT Settings Explained

| Setting | Description |
|---------|-------------|
| `SecretKey` | Secret key for signing tokens (min 256 bits) |
| `Issuer` | Token issuer identifier |
| `Audience` | Token audience identifier |
| `ExpirationMinutes` | Access token expiration time |
| `RefreshTokenExpirationDays` | Refresh token expiration time |

:::warning Security Note
**Never** commit real secret keys to source control. Use environment variables or Azure Key Vault in production.
:::

## Logging Configuration

Configure structured logging with Serilog:

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/realestate-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

## CORS Configuration

Configure Cross-Origin Resource Sharing:

```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "https://localhost:3001",
      "https://your-frontend-domain.com"
    ],
    "AllowedMethods": [ "GET", "POST", "PUT", "DELETE", "OPTIONS" ],
    "AllowedHeaders": [ "Content-Type", "Authorization" ],
    "AllowCredentials": true
  }
}
```

## API Configuration

General API settings:

```json
{
  "ApiSettings": {
    "Title": "RealEstate API",
    "Version": "v1.0",
    "Description": "RESTful API for real estate management",
    "MaxPageSize": 100,
    "DefaultPageSize": 20,
    "EnableSwagger": true,
    "EnableOutputCaching": true,
    "CacheExpirationMinutes": 5
  }
}
```

## Health Check Configuration

Configure health checks for monitoring:

```json
{
  "HealthChecks": {
    "Enabled": true,
    "DatabaseCheckEnabled": true,
    "MemoryCheckEnabled": true,
    "DiskSpaceCheckEnabled": true
  }
}
```

## Environment Variables

For production deployments, use environment variables:

### Required Environment Variables

```bash
# Database
DATABASE_SETTINGS__CONNECTIONSTRING="Server=prod;Database=RealEstate;..."

# JWT
JWT_SETTINGS__SECRETKEY="your-production-secret-key"
JWT_SETTINGS__ISSUER="RealEstateAPI"

# Logging
SERILOG__MINIMUMLEVEL__DEFAULT="Information"
```

### Docker Environment File

Create `.env` file for Docker deployments:

```env
# Database Configuration
DATABASE_SETTINGS__CONNECTIONSTRING=Server=db;Database=RealEstate;User Id=sa;Password=YourPassword123!

# JWT Configuration  
JWT_SETTINGS__SECRETKEY=your-super-secure-production-secret-key-minimum-256-bits
JWT_SETTINGS__ISSUER=RealEstateAPI
JWT_SETTINGS__AUDIENCE=RealEstateClients

# API Configuration
API_SETTINGS__ENABLESWAGGER=false
ASPNETCORE_ENVIRONMENT=Production
```

## Configuration Validation

The application validates configuration at startup. Invalid configurations will prevent the application from starting with clear error messages.

### Configuration Validation Rules

- `DatabaseSettings.ConnectionString` is required
- `JwtSettings.SecretKey` must be at least 256 bits (32 characters)
- `ApiSettings.MaxPageSize` must be between 1 and 1000
- `ApiSettings.DefaultPageSize` must be less than MaxPageSize

## Configuration Best Practices

1. **Never commit secrets** - Use environment variables or secure vaults
2. **Use different settings per environment** - Override only what changes
3. **Validate configuration** - Fail fast with clear error messages
4. **Document all settings** - Keep this guide updated
5. **Use strongly-typed configuration** - Leverage IOptions pattern

## Next Steps

- [First Run Guide](./first-run) - Start the application
- [Development Guide](../development/project-structure) - Understand the codebase
- [API Documentation](../api/overview) - Explore the API endpoints