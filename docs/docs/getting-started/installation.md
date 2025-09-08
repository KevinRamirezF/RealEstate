# Installation Guide

This guide will help you set up the complete RealEstate application stack using Docker. This is the **recommended approach** for both development and evaluation.

## 🚀 Quick Start (Recommended)

The easiest way to run the entire application stack is using Docker Compose. This provides:

- ✅ **Zero manual configuration** - Everything configured automatically
- ✅ **Complete environment** - Database, API, and Documentation
- ✅ **Consistent setup** - Works the same across all machines
- ✅ **One command startup** - Get everything running instantly

## Prerequisites

### Required Software

You only need **Docker** installed on your system:

- **Docker Desktop** - [Download for your OS](https://www.docker.com/products/docker-desktop/)
  - Windows: Docker Desktop for Windows
  - macOS: Docker Desktop for Mac  
  - Linux: Docker Engine + Docker Compose

### Verify Docker Installation

```bash
# Check Docker version
docker --version
# Should show Docker version 20.0+ 

# Check Docker Compose
docker-compose --version
# Should show docker-compose version 2.0+

# Verify Docker is running
docker info
```

## 🏗️ Application Setup

### 1. Clone the Repository

```bash
git clone https://github.com/KevinRamirezF/RealEstate
cd realestate-api
```

### 2. Start the Application Stack

Choose your preferred startup method:

#### Option A: Automated Script (Recommended)

**Windows:**
```bash
.\start.bat
```

**Linux/macOS:**
```bash
./start.sh
```

#### Option B: Manual Docker Compose

```bash
docker-compose up -d --build
```

### 3. Wait for Services to Initialize

The startup scripts will:
- Build all Docker images
- Start SQL Server database
- Create database automatically (via Entity Framework)
- Start the .NET API with health checks
- Build and serve the documentation
- Verify all services are healthy

**Initial startup takes 2-5 minutes** depending on your internet connection and system performance.

## 🌐 Access the Application

Once startup is complete, access these URLs:

| Service | URL | Description |
|---------|-----|-------------|
| **📖 Documentation** | http://localhost:3000 | Complete project documentation |
| **🔗 API Server** | http://localhost:5000 | REST API with Swagger/Scalar UI |
| **📋 API Testing** | http://localhost:5000/scalar | Interactive API documentation |
| **🗄️ Database** | localhost:1433 | SQL Server (sa/RealEstate123!) |

## ✅ Verify Installation

### 1. Health Checks
```bash
# API health check
curl http://localhost:5000/health

# Documentation health check  
curl http://localhost:3000/health
```

### 2. Basic API Tests
```bash
# Get all properties
curl http://localhost:5000/api/properties

# Get all owners
curl http://localhost:5000/api/owners
```

### 3. Interactive Testing
- Open http://localhost:5000/scalar in your browser
- Try the API endpoints directly in the UI
- Explore the complete OpenAPI documentation

## 🔧 Development Commands

### Service Management
```bash
# View all service logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f realestate-api

# Stop all services
docker-compose down

# Restart with rebuild
docker-compose up -d --build

# Reset everything (removes data)
docker-compose down -v
docker-compose up -d --build
```

### Database Management
```bash
# Connect to database directly
docker-compose exec realestate-db sqlcmd -S localhost -U sa -P RealEstate123!

# View database logs
docker-compose logs -f realestate-db
```

## 🏗️ Architecture Overview

The Docker setup includes 3 services:

```
┌─────────────────┐    ┌─────────────────┐
│   Documentation │    │    .NET API     │
│   (Docusaurus)  │    │  (with Scalar)  │
│   Port: 3000    │    │   Port: 5000    │
└─────────────────┘    └─────────────────┘
         │                       │
         └───────────────────────┼─────────────────┐
                                 │                 │
                        ┌─────────────────┐       │
                        │   SQL Server    │       │
                        │   Port: 1433    │       │
                        └─────────────────┘       │
                                                   │
                    Network: realestate-network ──┘
```

## 📚 What's Included

The Docker environment provides:

### **Database (SQL Server)**
- Automatic database creation via Entity Framework
- Pre-seeded with sample data (properties and owners)
- Persistent storage (data survives container restarts)

### **API Server (.NET 8)**  
- Complete REST API with all endpoints
- Integrated Swagger/OpenAPI documentation
- Scalar UI for interactive testing
- Health check endpoints
- Automatic database migrations

### **Documentation (Docusaurus)**
- Complete project documentation
- Architecture Decision Records (ADRs)
- API reference with direct links to testing interface
- Built-in search and navigation

## 🚨 Troubleshooting

### Common Issues

**Docker not starting:**
```bash
# Windows: Restart Docker Desktop
# Linux: Restart Docker service
sudo systemctl restart docker
```

**Port conflicts:**
```bash
# Check what's using the ports
netstat -an | findstr ":3000\|:5000\|:1433"

# Modify docker-compose.yml ports if needed
```

**Database connection issues:**
```bash
# View database startup logs
docker-compose logs realestate-db

# Reset database
docker-compose down -v
docker-compose up -d
```

**Build failures:**
```bash
# Clean Docker cache and rebuild
docker system prune -f
docker-compose build --no-cache
docker-compose up -d
```

### Getting Help

- Check service logs: `docker-compose logs [service-name]`
- Verify service health: `docker-compose ps`
- Reset everything: `docker-compose down -v && docker-compose up -d --build`

## ⚡ Performance Notes

- **First run**: Takes 2-5 minutes (downloads images, builds projects)
- **Subsequent runs**: Takes 30-60 seconds (uses cached images)
- **Database**: SQL Server in Developer mode (no licensing restrictions)
- **API**: Production configuration with optimizations enabled

## Next Steps

- [Configuration Guide](./configuration) - Advanced configuration options
- [API Reference](../api-reference) - Interactive API documentation
- [Architecture Overview](../architecture/overview) - Technical architecture details