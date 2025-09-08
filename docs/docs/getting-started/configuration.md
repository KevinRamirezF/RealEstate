# Configuration Guide

Learn how to configure and customize the RealEstate application when running with Docker Compose.

## 🐳 Docker Configuration (Default)

The Docker Compose setup provides **zero-configuration startup** with sensible defaults. All services are pre-configured to work together automatically.

### Default Service Configuration

| Service | Configuration | Details |
|---------|---------------|---------|
| **Database** | SQL Server 2022 | Auto-created with sample data |
| **API** | Production mode | All features enabled, optimized |
| **Documentation** | Static build | Nginx-served, optimized |

### Default Endpoints

- **API Server**: http://localhost:5000
- **API Documentation**: http://localhost:5000/scalar  
- **Project Documentation**: http://localhost:3000
- **Database**: localhost:1433 (sa/RealEstate123!)

## ⚙️ Customization Options

### Environment Variables

You can customize the setup using environment variables in `docker-compose.yml`:

```yaml
# API Service Environment Variables
environment:
  - ASPNETCORE_ENVIRONMENT=Production
  - ASPNETCORE_URLS=http://+:8080
  - DatabaseSettings__ConnectionString=Server=realestate-db;Database=RealEstateDB;User Id=sa;Password=RealEstate123!;TrustServerCertificate=true;MultipleActiveResultSets=true
  - DatabaseSettings__Provider=SqlServer
  - DatabaseSettings__CommandTimeout=30
  - Logging__LogLevel__Default=Information
  - Logging__LogLevel__Microsoft.AspNetCore=Warning
```

### Database Configuration

#### Change Database Password

Edit `docker-compose.yml`:

```yaml
services:
  realestate-db:
    environment:
      - MSSQL_SA_PASSWORD=YourNewPassword123!
  
  realestate-api:
    environment:
      - DatabaseSettings__ConnectionString=Server=realestate-db;Database=RealEstateDB;User Id=sa;Password=YourNewPassword123!;TrustServerCertificate=true;MultipleActiveResultSets=true
```

#### Use External Database

To use an existing SQL Server instance:

```yaml
services:
  realestate-api:
    environment:
      - DatabaseSettings__ConnectionString=Server=your-server;Database=RealEstateDB;User Id=your-user;Password=your-password;TrustServerCertificate=true;
```

Then remove the `realestate-db` service from `docker-compose.yml`.

### Port Configuration  

#### Change Service Ports

Edit the `ports` section in `docker-compose.yml`:

```yaml
services:
  realestate-api:
    ports:
      - "8080:8080"  # Change from 5000 to 8080
      
  realestate-docs:
    ports:
      - "4000:8080"  # Change from 3000 to 4000
```

#### Handle Port Conflicts

If ports are in use, modify `docker-compose.yml`:

```bash
# Check what's using your ports
netstat -an | findstr ":3000\|:5000\|:1433"

# Then update docker-compose.yml accordingly
```

### Development Mode

For development with hot reload and detailed logging:

```yaml
services:
  realestate-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development  # Enable development mode
      - Logging__LogLevel__Default=Debug    # More verbose logging
```

### Production Optimizations

For production deployment:

```yaml
services:
  realestate-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Logging__LogLevel__Default=Warning     # Less verbose
      - DatabaseSettings__CommandTimeout=60    # Longer timeouts
    restart: always                           # Always restart
    deploy:
      resources:
        limits:
          memory: 1G                          # Memory limit
        reservations:
          memory: 512M                        # Reserved memory
```

## 🔧 Advanced Configuration

### Custom Configuration File

Create a custom `docker-compose.override.yml` for local overrides:

```yaml
version: '3.8'

services:
  realestate-api:
    environment:
      - Logging__LogLevel__Default=Debug
      - ApiSettings__EnableSwagger=true
    volumes:
      - ./logs:/app/logs  # Mount local logs directory
  
  realestate-db:
    ports:
      - "1434:1433"  # Use different port
```

Run with: `docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d`

### Environment File (.env)

Create `.env` file in the project root:

```env
# Database
DB_PASSWORD=CustomPassword123!
DB_NAME=CustomRealEstateDB

# API
API_PORT=8080
DOCS_PORT=4000
DB_PORT=1434

# Logging
LOG_LEVEL=Information
```

Reference in `docker-compose.yml`:
```yaml
services:
  realestate-db:
    environment:
      - MSSQL_SA_PASSWORD=${DB_PASSWORD}
```

### SSL/HTTPS Configuration

For HTTPS in production:

```yaml
services:
  realestate-api:
    environment:
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_HTTPS_PORT=443
      - ASPNETCORE_Kestrel__Certificates__Default__Password=cert-password
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certificates/cert.pfx
    ports:
      - "443:443"
      - "80:80"
    volumes:
      - ./certificates:/app/certificates:ro
```

### Resource Limits

Set resource constraints:

```yaml
services:
  realestate-db:
    deploy:
      resources:
        limits:
          memory: 2G
          cpus: "1.0"
        reservations:
          memory: 1G
          cpus: "0.5"
          
  realestate-api:
    deploy:
      resources:
        limits:
          memory: 512M
          cpus: "0.5"
```

## 📊 Monitoring and Logging

### View Service Logs

```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f realestate-api

# Last 100 lines
docker-compose logs --tail=100 realestate-db
```

### Health Monitoring

Check service health:

```bash
# View service status
docker-compose ps

# Check health endpoints
curl http://localhost:5000/health
curl http://localhost:3000/health
```

### Log Configuration

Mount local directory for persistent logs:

```yaml
services:
  realestate-api:
    volumes:
      - ./logs:/app/logs
    environment:
      - Serilog__WriteTo__1__Args__path=/app/logs/api-.log
```

## 🚨 Troubleshooting

### Common Configuration Issues

**Database Connection Fails:**
```bash
# Check database is running
docker-compose logs realestate-db

# Verify connection string
docker-compose exec realestate-api printenv | grep DatabaseSettings
```

**Port Conflicts:**
```bash
# Find conflicting processes
netstat -an | findstr ":5000"

# Use different ports in docker-compose.yml
```

**Memory Issues:**
```bash
# Check container resource usage
docker stats

# Increase memory limits in docker-compose.yml
```

### Reset Configuration

Return to defaults:

```bash
# Stop and remove everything
docker-compose down -v

# Remove override file
rm docker-compose.override.yml

# Start fresh
docker-compose up -d --build
```

## 🔐 Security Configuration

### Production Security Checklist

- [ ] Change default database password
- [ ] Use environment variables for secrets
- [ ] Enable HTTPS with valid certificates
- [ ] Set resource limits
- [ ] Configure proper CORS origins
- [ ] Disable development features
- [ ] Enable security headers
- [ ] Set up log monitoring

### Secrets Management

**Development**: Use `docker-compose.override.yml`
**Production**: Use Docker secrets or external secret management

```yaml
# Using Docker secrets
services:
  realestate-api:
    secrets:
      - db_password
    environment:
      - DatabaseSettings__ConnectionString=Server=realestate-db;Database=RealEstateDB;User Id=sa;Password_File=/run/secrets/db_password;

secrets:
  db_password:
    file: ./secrets/db_password.txt
```

## 📈 Performance Tuning

### Database Performance

```yaml
services:
  realestate-db:
    environment:
      - MSSQL_MEMORY_LIMIT_MB=2048
    command: >
      /bin/bash -c "
      /opt/mssql/bin/mssql-conf set memory.memorylimitmb 2048 &&
      /opt/mssql/bin/sqlservr
      "
```

### API Performance

```yaml
services:
  realestate-api:
    environment:
      - DOTNET_GCServer=1
      - DOTNET_gcConcurrent=1
      - ApiSettings__EnableOutputCaching=true
      - ApiSettings__CacheExpirationMinutes=10
```

## Next Steps

- [Installation Guide](./installation) - Set up the complete environment
- [API Reference](../api-reference) - Explore the API endpoints  
- [Architecture Overview](../architecture/overview) - Understand the technical architecture

---

**Need Help?** Check the logs with `docker-compose logs -f` or [open an issue](https://github.com/your-org/realestate/issues) on GitHub.