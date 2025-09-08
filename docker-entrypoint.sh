#!/bin/bash
set -e

echo "Starting RealEstate API..."
echo "Environment: $ASPNETCORE_ENVIRONMENT"

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to be ready..."
max_attempts=30
attempt=1

while [ $attempt -le $max_attempts ]; do
    if /opt/mssql-tools18/bin/sqlcmd -S realestate-db -U sa -P RealEstate123! -Q "SELECT 1" -C &> /dev/null; then
        echo "SQL Server is ready!"
        break
    else
        echo "SQL Server is not ready yet. Attempt $attempt/$max_attempts. Retrying in 5 seconds..."
        sleep 5
        attempt=$((attempt + 1))
    fi
done

if [ $attempt -gt $max_attempts ]; then
    echo "Failed to connect to SQL Server after $max_attempts attempts"
    exit 1
fi

# Start the application (EF migrations will run automatically on first request)
echo "Starting the API application..."
exec dotnet RealEstate.API.dll