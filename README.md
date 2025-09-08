# 🏠 RealEstate API - Complete Application Stack

> **Million Realty LLC - Real Estate Management API**  
> A comprehensive Real Estate management API built with Clean Architecture principles

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download)
[![Docker](https://img.shields.io/badge/Docker-Compose-blue.svg)](https://www.docker.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean-green.svg)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![Documentation](https://img.shields.io/badge/Docs-Docusaurus-orange.svg)](https://docusaurus.io/)

## 🚀 Quick Start

### One-Command Startup (Recommended)

**For Windows:**
```bash
.\start.bat
```

**For Linux/macOS:**
```bash
./start.sh
```

### Manual Startup
```bash
docker-compose up -d --build
```

## 📍 Service Endpoints

Once the application is running:

| Service | URL | Description |
|---------|-----|-------------|
| 📖 **Documentation** | http://localhost:3000 | Complete project documentation |
| 🔗 **API Server** | http://localhost:5000 | REST API with integrated Scalar UI |
| 📋 **API Reference** | http://localhost:5000/scalar | Interactive API testing interface |
| 🗄️ **Database** | localhost:1433 | SQL Server (sa/RealEstate123!) |

## 🏗️ Architecture Overview

This application implements **Clean Architecture** with the following layers:

```
┌─────────────────────────────────────────┐
│              RealEstate.API             │ ← Controllers, Middleware, DTOs
├─────────────────────────────────────────┤
│          RealEstate.Application         │ ← Use Cases, CQRS, Validation
├─────────────────────────────────────────┤  
│            RealEstate.Domain            │ ← Entities, Value Objects, Rules
├─────────────────────────────────────────┤
│        RealEstate.Infrastructure        │ ← Data Access, External Services
└─────────────────────────────────────────┘
```

### Key Patterns & Technologies

- ✅ **Clean Architecture** - Dependency rule, separation of concerns
- ✅ **CQRS Light** - Command/Query separation without MediatR
- ✅ **Domain-Driven Design** - Rich domain models, value objects
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **Unit of Work** - Transaction management
- ✅ **FluentValidation** - Comprehensive input validation
- ✅ **Entity Framework Core** - ORM with code-first migrations
- ✅ **Soft Delete** - Data retention with audit trails
- ✅ **Docker Compose** - Complete containerized environment

## 📊 API Features

### Core Endpoints

| Resource | Methods | Description |
|----------|---------|-------------|
| **Properties** | GET, POST, PUT, PATCH, DELETE | Property management |
| **Owners** | GET, POST, PUT, PATCH, DELETE | Owner management |  
| **Property Images** | GET, POST, DELETE | Image handling |
| **Property Traces** | GET | Audit trail tracking |
| **Health** | GET | System health check |

### Advanced Features

- 🔍 **Advanced Filtering** - Multi-criteria property search
- ✏️ **Partial Updates** - Efficient PATCH operations  
- 🔐 **JWT Authentication** - Bearer token support
- 📝 **Comprehensive Validation** - FluentValidation integration
- 📋 **Audit Trails** - Complete change tracking
- ⚡ **Output Caching** - Performance optimization
- 🚨 **Global Exception Handling** - Standardized error responses

## 🛠️ Development Stack

### Backend Technologies
- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM and migrations
- **SQL Server** - Primary database
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Scalar** - Interactive API documentation

### Frontend & Documentation  
- **Docusaurus** - Documentation platform
- **React** - Documentation UI components
- **Nginx** - Web server for documentation
- **Scalar UI** - Interactive API testing

### DevOps & Infrastructure
- **Docker** - Containerization
- **Docker Compose** - Multi-service orchestration
- **Health Checks** - Service monitoring
- **Volume Persistence** - Database data retention

## 📂 Project Structure

```
RealEstate/
├── RealEstate.API/              # Web API layer
├── RealEstate.Application/      # Business logic layer  
├── RealEstate.Domain/           # Core domain layer
├── RealEstate.Infrastructure/   # Data access layer
├── RealEstate.UnitTests/        # Unit tests
├── docs/                        # Docusaurus documentation
├── database/                    # Database scripts
├── scalar/                      # API documentation UI
├── docker-compose.yml           # Service orchestration
├── Dockerfile                   # API containerization
├── start.sh                     # Linux/macOS startup script
├── start.bat                    # Windows startup script
└── README.md                    # This file
```

## 🔧 Management Commands

### Service Management
```bash
# Start all services
docker-compose up -d

# Stop all services  
docker-compose down

# Rebuild and restart
docker-compose up -d --build

# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f realestate-api
```

### Database Management
```bash  
# Reset database (destroys all data)
docker-compose down -v
docker-compose up -d

# Database backup
docker-compose exec realestate-db sqlcmd -S localhost -U sa -P RealEstate123! -Q "BACKUP DATABASE RealEstateDB TO DISK='/tmp/backup.bak'"

# Access database directly
docker-compose exec realestate-db sqlcmd -S localhost -U sa -P RealEstate123!
```

### Development Commands
```bash
# Build .NET solution
dotnet build

# Run tests
dotnet test

# Create new migration
dotnet ef migrations add MigrationName --project RealEstate.Infrastructure --startup-project RealEstate.API

# Update documentation
cd docs && npm run build
```

## 🧪 Testing the API

### Health Check
```bash
curl http://localhost:5000/health
```

### Basic Operations
```bash
# Get all properties
curl http://localhost:5000/api/properties

# Get all owners
curl http://localhost:5000/api/owners

# Create new property
curl -X POST http://localhost:5000/api/properties \
  -H "Content-Type: application/json" \
  -d '{"address":{"street":"123 Main St","city":"Miami","state":"FL","zipCode":"33101"},"price":500000,"ownerId":1}'
```

### Interactive Testing
- **Scalar UI**: http://localhost:5000/scalar - Full-featured interactive API testing and documentation

## 📚 Documentation

Complete project documentation is available at: http://localhost:3000

### Key Documentation Sections

- **[Getting Started](http://localhost:3000/docs/getting-started/installation)** - Setup and installation  
- **[Architecture](http://localhost:3000/docs/architecture/overview)** - Technical architecture details
- **[API Reference](http://localhost:3000/docs/api-reference)** - Interactive API documentation
- **[ADRs](http://localhost:3000/docs/adrs)** - Architecture Decision Records

## 🐳 Docker Configuration Explained

### Services Overview

The Docker setup includes **3 optimized services**:

1. **realestate-db** (SQL Server 2022)
   - Port: 1433
   - Credentials: sa/RealEstate123!
   - Volume: Persistent data storage
   - Health checks: SQL connectivity
   - Auto-database creation via EF Core

2. **realestate-api** (.NET 8 API)
   - Port: 5000
   - Features: Integrated Scalar UI at `/scalar`
   - Dependencies: Database availability
   - Environment: Production-optimized
   - Health checks: Application health endpoint

3. **realestate-docs** (Docusaurus)
   - Port: 3000  
   - Content: Complete project documentation
   - Server: Nginx with optimization
   - Features: ADRs, architecture docs, API reference

### Network Configuration
- Custom bridge network: `realestate-network`
- Subnet: 172.20.0.0/16
- Service discovery: Container names as hostnames

## ⚡ Performance & Security

### Performance Features
- Output caching with ETag support
- Connection pooling and retry policies
- Optimized Docker layers
- Gzip compression
- Static asset caching

### Security Features  
- Non-root container users
- Security headers (CSP, HSTS, etc.)
- Input validation and sanitization
- SQL injection protection
- CORS configuration
- JWT Bearer authentication ready

## 🤝 Company Information

**Million Realty LLC**  
- **Brand:** MILLION
- **Address:** 237 S Dixie Hwy, 4th Floor, Suite 465, Coral Gables, FL 33133
- **Licensed In:** Florida  
- **Recognition:** #1 Top Team - New construction sales in the U.S. ($2.1B+ in sales)
- **Specialization:** Luxury real estate in South Florida
- **Website:** https://www.millionluxury.com

## 👨‍💻 Developer Information

**Developed By:** Ing. Kevin Guillermo Ramirez  
**Role:** Senior Backend Developer .NET  
**Email:** kevinramirezf@outlook.com  
**Purpose:** Technical test for Million Realty Senior .NET developer interview

---

## 📝 License

This project is developed as a technical demonstration for Million Realty LLC.

---

*For additional support or questions, please contact the development team.*