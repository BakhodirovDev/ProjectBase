# ============================================
# ProjectBase - Production-Ready Dockerfile
# .NET 10 | Multi-stage | Security-hardened
# ============================================

ARG BUILD_CONFIGURATION=Release
ARG DOTNET_VERSION=10.0

# ===========================================
# Stage 1: Restore
# ===========================================
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore
WORKDIR /src

COPY ["ProjectBase.Web.sln", "./"]
COPY ["ProjectBase.Web/ProjectBase.WebApi.csproj", "ProjectBase.Web/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "ProjectBase.Web.sln"

# ===========================================
# Stage 2: Build
# ===========================================
FROM restore AS build
ARG BUILD_CONFIGURATION

COPY . .

WORKDIR /src
RUN dotnet build "ProjectBase.Web.sln" \
    -c ${BUILD_CONFIGURATION} \
    --no-restore

# ===========================================
# Stage 3: Test (optional)
# ===========================================
FROM build AS test
WORKDIR /src

RUN dotnet test "ProjectBase.Web.sln" \
    --no-build \
    --configuration ${BUILD_CONFIGURATION} \
    --logger "trx;LogFileName=test_results.trx"

# ===========================================
# Stage 4: Publish
# ===========================================
FROM build AS publish
ARG BUILD_CONFIGURATION

WORKDIR /src/ProjectBase.Web
RUN dotnet publish "ProjectBase.WebApi.csproj" \
    -c ${BUILD_CONFIGURATION} \
    -o /app/publish \
    --no-restore \
    --no-build \
    /p:UseAppHost=false
# ===========================================
# Stage 5: Runtime
# ===========================================
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-alpine AS final
WORKDIR /app

RUN apk add --no-cache \
    icu-libs \
    icu-data-full \
    curl \
    wget \
    tzdata \
    ca-certificates \
    && update-ca-certificates

RUN addgroup -g 1000 appuser && \
    adduser -D -u 1000 -G appuser appuser && \
    chown -R appuser:appuser /app

COPY --from=publish --chown=appuser:appuser /app/publish .

USER appuser

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_GCServer=1 \
    DOTNET_GCConcurrent=1 \
    DOTNET_ThreadPool_MinThreads=100 \
    DOTNET_EnableDiagnostics=0 \
    TZ=Asia/Tashkent

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

LABEL maintainer="your-email@projectbase.com" \
      version="1.0" \
      description="ProjectBase Web API" \
      org.opencontainers.image.source="https://github.com/BakhodirovDev/ProjectBase"

ENTRYPOINT ["dotnet", "ProjectBase.WebApi.dll"]