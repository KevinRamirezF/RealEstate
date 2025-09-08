#!/bin/bash

# RealEstate Application Startup Script
# Author: Ing. Kevin Guillermo Ramirez
# Purpose: One-command startup for the entire RealEstate application stack

set -e

echo "🏠 RealEstate Application Stack Starting..."
echo "========================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if Docker is running
echo "📋 Checking prerequisites..."
if ! docker info > /dev/null 2>&1; then
    echo -e "${RED}❌ Docker is not running. Please start Docker Desktop first.${NC}"
    exit 1
fi

if ! command -v docker-compose > /dev/null 2>&1; then
    echo -e "${RED}❌ docker-compose is not installed or not in PATH.${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Docker is running${NC}"
echo -e "${GREEN}✅ docker-compose is available${NC}"
echo ""

# Clean up any existing containers
echo "🧹 Cleaning up existing containers..."
docker-compose down -v --remove-orphans > /dev/null 2>&1 || true
echo -e "${GREEN}✅ Cleanup completed${NC}"
echo ""

# Build and start services
echo "🔨 Building and starting services..."
echo "This may take a few minutes on the first run..."
echo ""

docker-compose up -d --build

echo ""
echo "🚀 Waiting for services to be ready..."

# Function to check if service is healthy
check_service_health() {
    local service_name=$1
    local url=$2
    local max_attempts=60
    local attempt=1

    echo -n "   Checking $service_name"
    
    while [ $attempt -le $max_attempts ]; do
        if curl -f "$url" > /dev/null 2>&1; then
            echo -e " ${GREEN}✅ Ready${NC}"
            return 0
        else
            echo -n "."
            sleep 2
            attempt=$((attempt + 1))
        fi
    done
    
    echo -e " ${RED}❌ Failed to start${NC}"
    return 1
}

# Wait for database
echo "   Checking Database... (this may take up to 2 minutes)"
attempt=1
max_attempts=60
while [ $attempt -le $max_attempts ]; do
    if docker-compose exec -T realestate-db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P RealEstate123! -Q "SELECT 1" > /dev/null 2>&1; then
        echo -e "   Database ${GREEN}✅ Ready${NC}"
        break
    else
        echo -n "."
        sleep 2
        attempt=$((attempt + 1))
    fi
done

if [ $attempt -gt $max_attempts ]; then
    echo -e "   Database ${RED}❌ Failed to start${NC}"
    echo "Please check the logs with: docker-compose logs realestate-db"
    exit 1
fi

# Check API
check_service_health "API Server" "http://localhost:5000/health"

# Check Documentation
check_service_health "Documentation" "http://localhost:3000/health"

echo ""
echo -e "${GREEN}🎉 All services are ready!${NC}"
echo ""
echo "📍 Service Endpoints:"
echo "========================================"
echo -e "${BLUE}📖 Documentation:${NC}       http://localhost:3000"
echo -e "${BLUE}🔗 API Server:${NC}           http://localhost:5000"
echo -e "${BLUE}📋 API Reference:${NC}        http://localhost:5000/scalar"
echo -e "${BLUE}🗄️  Database:${NC}            localhost:1433 (sa/RealEstate123!)"
echo ""
echo "🔧 Management Commands:"
echo "========================================"
echo "View logs:              docker-compose logs -f"
echo "Stop services:          docker-compose down"  
echo "Rebuild services:       docker-compose up -d --build"
echo "Reset database:         docker-compose down -v && docker-compose up -d"
echo ""
echo -e "${GREEN}🚀 The RealEstate application is now running!${NC}"
echo -e "${YELLOW}💡 Start by visiting the documentation at: http://localhost:3000${NC}"