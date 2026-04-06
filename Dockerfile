# ─── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first (layer cache — only invalidated if .csproj changes)
COPY src/Academia.Domain/Academia.Domain.csproj             src/Academia.Domain/
COPY src/Academia.Application/Academia.Application.csproj   src/Academia.Application/
COPY src/Academia.Infrastructure/Academia.Infrastructure.csproj src/Academia.Infrastructure/
COPY src/Academia.WebAPI/Academia.WebAPI.csproj              src/Academia.WebAPI/

RUN dotnet restore src/Academia.WebAPI/Academia.WebAPI.csproj

# Copy everything else and publish
COPY . .
RUN dotnet publish src/Academia.WebAPI/Academia.WebAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ─── Stage 2: Runtime ────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create uploads directory with correct permissions
RUN mkdir -p /app/wwwroot/uploads && chmod 755 /app/wwwroot/uploads

COPY --from=build /app/publish .

# Runtime environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

# Health check built into the image
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Academia.WebAPI.dll"]
