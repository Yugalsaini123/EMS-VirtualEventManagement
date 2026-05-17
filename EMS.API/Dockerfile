# Multi-stage build for EMS.API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy project files
COPY ["EMS.API/EMS.API.csproj", "EMS.API/"]
COPY ["EMS.DAL/EMS.DAL.csproj", "EMS.DAL/"]
COPY ["EMS.Services/EMS.Services.csproj", "EMS.Services/"]
COPY ["EMS.Tests/EMS.Tests.csproj", "EMS.Tests/"]

# Restore dependencies
RUN dotnet restore "EMS.API/EMS.API.csproj"

# Copy remaining source code
COPY . .

# Build the project
RUN dotnet build "EMS.API/EMS.API.csproj" -c Release -o /app/build

# Publish the project
RUN dotnet publish "EMS.API/EMS.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app from builder
COPY --from=builder /app/publish .

# Use .NET 8's built-in non-root 'app' user
USER app

# ✅ EXPOSE 5000 - matches docker-compose port mapping
EXPOSE 5000

# ✅ HEALTHCHECK on port 5000
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=5 \
  CMD curl -f http://localhost:5000/health || exit 1

# ✅ ASPNETCORE_URLS must match EXPOSE and docker-compose
ENV ASPNETCORE_URLS=http://+:5000

# Run the application
ENTRYPOINT ["dotnet", "EMS.API.dll"]
