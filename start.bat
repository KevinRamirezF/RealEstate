@echo off
REM RealEstate Application Startup Script for Windows
REM Author: Ing. Kevin Guillermo Ramirez  
REM Purpose: One-command startup for the entire RealEstate application stack

setlocal

echo 🏠 RealEstate Application Stack Starting...
echo ========================================
echo.

echo 📋 Checking prerequisites...

REM Check if Docker is running
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker Desktop first.
    pause
    exit /b 1
)

REM Check if docker-compose is available
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ docker-compose is not installed or not in PATH.
    pause
    exit /b 1
)

echo ✅ Docker is running
echo ✅ docker-compose is available
echo.

echo 🧹 Cleaning up existing containers...
docker-compose -f docker-compose.dev.yml down -v --remove-orphans >nul 2>&1
echo ✅ Cleanup completed
echo.

echo 🔨 Building and starting services (DEVELOPMENT MODE)...
echo This may take a few minutes on the first run...
echo.

docker-compose -f docker-compose.dev.yml up -d --build

echo.
echo 🚀 Waiting for services to be ready...

REM Wait for database (up to 2 minutes)
echo    Checking Database... (this may take up to 2 minutes)
set /a attempt=1
set /a max_attempts=60

:db_wait_loop
docker-compose -f docker-compose.dev.yml exec -T realestate-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P RealEstate123! -Q "SELECT 1" -C >nul 2>&1
if %errorlevel% equ 0 (
    echo    Database ✅ Ready
    goto db_ready
)
echo|set /p="."
timeout /t 2 /nobreak >nul
set /a attempt+=1
if %attempt% leq %max_attempts% goto db_wait_loop

echo    Database ❌ Failed to start
echo Please check the logs with: docker-compose -f docker-compose.dev.yml logs realestate-db
pause
exit /b 1

:db_ready

REM Check API health
echo    Checking API Server...
set /a attempt=1
:api_wait_loop
curl -f http://localhost:5000/health >nul 2>&1
if %errorlevel% equ 0 (
    echo    API Server ✅ Ready
    goto api_ready
)
echo|set /p="."
timeout /t 2 /nobreak >nul
set /a attempt+=1
if %attempt% leq 30 goto api_wait_loop
echo    API Server ❌ Failed to start
goto services_check_done

:api_ready

REM Documentation service is started but we don't check health as it doesn't have a health endpoint
echo    Documentation service started (no health check available)
goto services_check_done

:services_check_done

echo.
echo 🎉 Services are starting up!
echo.
echo 📍 Service Endpoints:
echo ========================================
echo 📖 Documentation:       http://localhost:3000
echo 🔗 API Server:           http://localhost:5000  
echo 📋 API Reference:        http://localhost:5000/scalar
echo 🗄️  Database:            localhost:1433 (sa/RealEstate123!)
echo.
echo 🔧 Management Commands (DEVELOPMENT):
echo ========================================
echo View logs:              docker-compose -f docker-compose.dev.yml logs -f
echo Stop services:          docker-compose -f docker-compose.dev.yml down
echo Rebuild services:       docker-compose -f docker-compose.dev.yml up -d --build
echo Reset database:         docker-compose -f docker-compose.dev.yml down -v ^&^& docker-compose -f docker-compose.dev.yml up -d
echo.
echo 🚀 The RealEstate application is now running!
echo 💡 Start by visiting the documentation at: http://localhost:3000
echo.
echo Press any key to continue...
pause >nul