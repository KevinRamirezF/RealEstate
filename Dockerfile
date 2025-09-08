# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy solution file and project files
COPY *.sln ./
COPY RealEstate.API/*.csproj ./RealEstate.API/
COPY RealEstate.Application/*.csproj ./RealEstate.Application/
COPY RealEstate.Domain/*.csproj ./RealEstate.Domain/
COPY RealEstate.Infrastructure/*.csproj ./RealEstate.Infrastructure/
COPY RealEstate.UnitTests/*.csproj ./RealEstate.UnitTests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . ./

# Install Entity Framework global tool in build stage
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Build and publish the application
RUN dotnet publish RealEstate.API -c Release -o out

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Install curl and SQL Server tools for health checks and migrations
RUN apt-get update && apt-get install -y \
    curl \
    gnupg2 \
    wget \
    && wget -qO- https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
    && echo "deb [arch=amd64,arm64,armhf] https://packages.microsoft.com/debian/11/prod bullseye main" > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools18 \
    && rm -rf /var/lib/apt/lists/*

# Add SQL tools to PATH
ENV PATH="$PATH:/opt/mssql-tools18/bin"

# Copy EF tools from build stage
COPY --from=build-env /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy entry point script first
COPY docker-entrypoint.sh .
# Copy published application
COPY --from=build-env /app/out .
RUN chmod +x docker-entrypoint.sh

# Create a non-root user for security (but keep root for migrations)
RUN addgroup --gid 1001 --system appgroup && \
    adduser --uid 1001 --system --gid 1001 appuser && \
    chown -R appuser:appgroup /app
# Note: We'll switch to appuser after migrations in the entrypoint

# Expose port 8080 (non-privileged port)
EXPOSE 8080

# Configure ASP.NET Core to use port 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Start the application
ENTRYPOINT ["./docker-entrypoint.sh"]