# Development Guide — Running & Testing the Application

> **Ambev Developer Evaluation — Sales API**
>
> This document provides step-by-step instructions for setting up the development environment, running the application, executing tests (unit, integration, and functional), building for production, and troubleshooting common issues.

---

## Table of Contents

1. [Prerequisites](#1-prerequisites)
2. [Repository Structure Overview](#2-repository-structure-overview)
3. [Environment Setup](#3-environment-setup)
4. [Running the Application in Development Mode](#4-running-the-application-in-development-mode)
5. [Database Migrations & Seed Data](#5-database-migrations--seed-data)
6. [Running Tests](#6-running-tests)
   - [Unit Tests](#61-unit-tests)
   - [Integration Tests](#62-integration-tests)
   - [Functional Tests](#63-functional-tests)
   - [Running All Tests at Once](#64-running-all-tests-at-once)
   - [Code Coverage Report](#65-code-coverage-report)
7. [Building for Production](#7-building-for-production)
8. [Docker Deployment](#8-docker-deployment)
9. [Environment Variables & Configuration Reference](#9-environment-variables--configuration-reference)
10. [Troubleshooting Common Issues](#10-troubleshooting-common-issues)

---

## 1. Prerequisites

Before you begin, make sure the following tools are installed on your machine:

| Tool | Minimum Version | Purpose |
|---|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | **8.0** | Build and run the C# solution |
| [Docker](https://docs.docker.com/get-docker/) | 20.10+ | Container runtime for infrastructure services |
| [Docker Compose](https://docs.docker.com/compose/install/) | v2+ (or `docker compose` plugin) | Orchestrate multi-container setup |
| [Git](https://git-scm.com/) | 2.x | Clone the repository |

#### Optional but recommended

| Tool | Purpose |
|---|---|
| [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/) | Full IDE experience with debugging |
| [Visual Studio Code](https://code.visualstudio.com/) + C# Dev Kit | Lightweight alternative |
| [pgAdmin](https://www.pgadmin.org/) or [DBeaver](https://dbeaver.io/) | Database management / inspection |

#### Verify your environment

```bash
dotnet --version        # Should output 8.0.x
docker --version        # Should output 20.10+
docker compose version  # Should output v2.x+
```

---

## 2. Repository Structure Overview

```
mouts-test/
├── .doc/                         # Project documentation (tech stack, API specs, etc.)
├── docs/                         # Development guides (this file)
├── template/backend/             # Main backend solution root
│   ├── Ambev.DeveloperEvaluation.sln       # Visual Studio solution file
│   ├── docker-compose.yml                  # Docker Compose (PostgreSQL, MongoDB, Redis, WebAPI)
│   ├── docker-compose.override.yml         # Compose overrides (empty by default)
│   ├── Dockerfile                          # Multi-stage Dockerfile for the WebAPI
│   ├── coverage-report.sh / .bat           # Scripts to generate code coverage reports
│   ├── src/
│   │   ├── Ambev.DeveloperEvaluation.WebApi/         # ASP.NET Core 8 Web API (entry point)
│   │   ├── Ambev.DeveloperEvaluation.Application/    # Application layer (CQRS handlers, MediatR)
│   │   ├── Ambev.DeveloperEvaluation.Domain/         # Domain layer (entities, events, specifications)
│   │   ├── Ambev.DeveloperEvaluation.ORM/            # Infrastructure — EF Core, migrations, repositories
│   │   ├── Ambev.DeveloperEvaluation.IoC/            # Dependency injection / module initializers
│   │   └── Ambev.DeveloperEvaluation.Common/         # Cross-cutting concerns (auth, logging, validation)
│   └── tests/
│       ├── Ambev.DeveloperEvaluation.Unit/           # Unit tests (xUnit + NSubstitute + Bogus)
│       ├── Ambev.DeveloperEvaluation.Integration/    # Integration tests (Testcontainers + PostgreSQL)
│       └── Ambev.DeveloperEvaluation.Functional/     # Functional / E2E tests
└── README.md
```

### Key Technology Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8.0 / C# |
| Web Framework | ASP.NET Core 8 (Kestrel) |
| API Documentation | Swagger / Swashbuckle |
| ORM | Entity Framework Core 8 (Npgsql) |
| Database | PostgreSQL 13 |
| NoSQL (optional) | MongoDB 8.0 |
| Cache (optional) | Redis 7.4 |
| Mediator / CQRS | MediatR 12.4 |
| Object Mapping | AutoMapper 16.1 |
| Validation | FluentValidation 11.10 / 12.1 |
| Logging | Serilog (console sink) |
| Authentication | JWT Bearer Tokens (BCrypt password hashing) |
| Testing Framework | xUnit 2.9 |
| Mocking | NSubstitute 5.1 |
| Fake Data | Bogus 35.6 |
| Assertions | FluentAssertions 6.12 |
| Integration DB | Testcontainers.PostgreSql 3.10 |
| Code Coverage | Coverlet + ReportGenerator |

---

## 3. Environment Setup

### 3.1 Clone the Repository

```bash
git clone https://github.com/matheus-vieira/mouts-test.git
cd mouts-test
```

### 3.2 Navigate to the Backend Solution

All .NET commands below should be run from the solution root:

```bash
cd template/backend
```

### 3.3 Restore NuGet Packages

```bash
dotnet restore Ambev.DeveloperEvaluation.sln
```

### 3.4 Configuration Files

The application uses standard ASP.NET Core configuration. The relevant files are:

| File | Purpose |
|---|---|
| `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json` | Base configuration (connection strings, JWT, logging) |
| `src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json` | Development overrides (Kestrel port, AutoMapper license) |

#### Default connection string (in `appsettings.json`):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
  }
}
```

> **Note:** When running via Docker Compose, the connection string is overridden in `docker-compose.yml` to point to the internal container hostname (`ambev.developerevaluation.database`).

#### JWT configuration:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32BytesLong"
  }
}
```

> **Tip:** For production deployments, override these values via environment variables or user secrets. Never commit real secrets to source control.

### 3.5 Start Infrastructure Services (Database, Cache, NoSQL)

The easiest way to get all infrastructure running is via Docker Compose:

```bash
docker compose up -d ambev.developerevaluation.database ambev.developerevaluation.nosql ambev.developerevaluation.cache
```

This starts:

| Service | Image | Internal Port | Default Credentials |
|---|---|---|---|
| PostgreSQL | `postgres:13` | 5432 | `developer` / `ev@luAt10n` / DB: `developer_evaluation` |
| MongoDB | `mongo:8.0` | 27017 | `developer` / `ev@luAt10n` |
| Redis | `redis:7.4.1-alpine` | 6379 | Password: `ev@luAt10n` |

To verify the PostgreSQL container is healthy:

```bash
docker compose ps
# Look for "healthy" in the STATUS column for the database container
```

> **Note:** The `docker-compose.yml` does not expose fixed host ports for PostgreSQL (it uses a random host port). If you want to connect from your local machine with a fixed port, add a mapping like `"5432:5432"` under the database service ports, or use `docker compose port ambev.developerevaluation.database 5432` to discover the dynamically assigned port.

---

## 4. Running the Application in Development Mode

### Option A — Using the .NET CLI (Recommended for Development)

Make sure PostgreSQL is running (see [Section 3.5](#35-start-infrastructure-services-database-cache-nosql)), then:

```bash
cd src/Ambev.DeveloperEvaluation.WebApi
dotnet run --launch-profile http
```

The API will be available at:
- **HTTP:** `http://localhost:5119`
- **Swagger UI:** `http://localhost:5119/swagger`

Alternatively, use the `https` profile:

```bash
dotnet run --launch-profile https
```

- **HTTPS:** `https://localhost:7181`
- **HTTP:** `http://localhost:5119`

### Option B — Using Docker Compose (Full Stack)

From the `template/backend` directory:

```bash
docker compose up --build
```

This builds the WebAPI image and starts all services. The API will be available at:
- **HTTP:** `http://localhost:8080`
- **Swagger UI:** `http://localhost:8080/swagger`

To stop all services:

```bash
docker compose down
```

### Option C — Using Visual Studio / Rider

1. Open `template/backend/Ambev.DeveloperEvaluation.sln`
2. Set `Ambev.DeveloperEvaluation.WebApi` as the startup project
3. Select a launch profile (`http`, `https`, `Docker Compose`, or `Container (Dockerfile)`)
4. Press **F5** to run with debugging

---

## 5. Database Migrations & Seed Data

### Automatic Migrations

The application **automatically applies pending EF Core migrations** on startup. This is handled by the `MigrateDatabase()` extension method in `Program.cs`, which retries up to 5 times with a 3-second delay between attempts.

Current migrations:
1. **`20241014011203_InitialMigrations`** — Creates the `Users` table
2. **`20260519142226_CreateSalesTable`** — Creates `Sales` and `SaleItems` tables, adds `CreatedAt`/`UpdatedAt` to `Users`

### Manual Migration Commands

If you need to manage migrations manually:

```bash
# Navigate to the solution root
cd template/backend

# Apply all pending migrations
dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi

# Revert to a specific migration
dotnet ef database update 20241014011203_InitialMigrations \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi

# Add a new migration
dotnet ef migrations add YourMigrationName \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

### Seed Data

There is no built-in seed data mechanism. The database starts empty. You can create users and sales through the API endpoints after startup.

---

## 6. Running Tests

All test commands should be executed from the **solution root** (`template/backend`).

### 6.1 Unit Tests

Unit tests are located in `tests/Ambev.DeveloperEvaluation.Unit/` and test domain entities, validators, application handlers, and mapping profiles in isolation using **NSubstitute** for mocking and **Bogus** for fake data generation.

**No external dependencies required** (no database, no Docker).

```bash
# Run all unit tests
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj --verbosity normal

# Run with detailed output
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj --verbosity detailed

# Run a specific test class
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj \
  --filter "FullyQualifiedName~SaleTests"

# Run a specific test method
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj \
  --filter "FullyQualifiedName~CreateSaleHandlerTest"
```

#### What unit tests cover:

- **Domain Entities:** `Sale`, `SaleItem`, `User` — behavior, factory methods, validation
- **Domain Validation:** `EmailValidator`, `PasswordValidator`, `PhoneValidator`, `SaleValidator`, `SaleItemValidator`
- **Domain Specifications:** `ActiveUserSpecification`, `SaleFilterSpecification`
- **Application Handlers:** `CreateSale`, `UpdateSale`, `DeleteSale`, `CancelSale`, `GetSale`, `ListSales`, `CreateUser`
- **Application Validators:** Command validators for all CQRS commands
- **Application Events:** `SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled` event handlers
- **Mapping Profiles:** AutoMapper `SalesMappingProfile`

### 6.2 Integration Tests

Integration tests are located in `tests/Ambev.DeveloperEvaluation.Integration/` and test the data access layer (repositories, EF Core configurations, specifications) against a **real PostgreSQL database** spun up automatically via [Testcontainers](https://dotnet.testcontainers.org/).

**Requires Docker to be running** (Testcontainers manages the PostgreSQL container lifecycle automatically — no manual setup needed).

```bash
# Run all integration tests
dotnet test tests/Ambev.DeveloperEvaluation.Integration/Ambev.DeveloperEvaluation.Integration.csproj --verbosity normal

# Run a specific repository test
dotnet test tests/Ambev.DeveloperEvaluation.Integration/Ambev.DeveloperEvaluation.Integration.csproj \
  --filter "FullyQualifiedName~SaleCreateRepositoryTests"
```

> **Important:** Integration tests are configured to run **sequentially** (not in parallel) via `xunit.runner.json`:
> ```json
> { "parallelizeTestCollections": false, "maxParallelThreads": 1 }
> ```
> Each test collection uses its own schema for isolation.

#### What integration tests cover:

- **Repositories:** `SaleCreateRepository`, `SaleReadRepository`, `SaleUpdateRepository`, `SaleDeleteRepository`
- **Entity Mappings:** EF Core `SaleConfiguration` / `SaleItemConfiguration`
- **Specifications:** `SaleFilterSpecification` with real database queries

### 6.3 Functional Tests

Functional tests are located in `tests/Ambev.DeveloperEvaluation.Functional/`. These tests validate higher-level scenarios and behaviors.

**No external dependencies required** (same as unit tests — uses NSubstitute and Bogus).

```bash
# Run all functional tests
dotnet test tests/Ambev.DeveloperEvaluation.Functional/Ambev.DeveloperEvaluation.Functional.csproj --verbosity normal
```

### 6.4 Running All Tests at Once

```bash
# From the solution root (template/backend)
dotnet test Ambev.DeveloperEvaluation.sln --verbosity normal
```

> **Note:** Running all tests requires **Docker** to be running because integration tests use Testcontainers.

#### Filter by test category or name:

```bash
# Run only tests containing "Sale" in their name
dotnet test Ambev.DeveloperEvaluation.sln --filter "FullyQualifiedName~Sale"

# Run only tests from the Unit project
dotnet test Ambev.DeveloperEvaluation.sln --filter "FullyQualifiedName~Unit"
```

### 6.5 Code Coverage Report

The repository includes scripts to generate HTML coverage reports using **Coverlet** and **ReportGenerator**.

#### On Linux / macOS:

```bash
cd template/backend
chmod +x coverage-report.sh
./coverage-report.sh
```

#### On Windows:

```cmd
cd template\backend
coverage-report.bat
```

#### What the script does:

1. Installs `coverlet.console` and `dotnet-reportgenerator-globaltool` as global .NET tools
2. Restores and builds the solution in `Release` mode
3. Runs all tests with Coverlet collecting coverage data (Cobertura format)
4. Generates an HTML report at `TestResults/CoverageReport/index.html`
5. Cleans up temporary `bin`/`obj` folders

#### Manual coverage commands:

```bash
# Run tests with coverage collection
dotnet test Ambev.DeveloperEvaluation.sln --no-restore --verbosity normal \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=cobertura \
  /p:CoverletOutput=./TestResults/coverage.cobertura.xml \
  /p:Exclude="[*]*.Program,[*]*.Startup,[*]*.Migrations.*"

# Generate HTML report
reportgenerator \
  -reports:"./tests/**/TestResults/coverage.cobertura.xml" \
  -targetdir:"./TestResults/CoverageReport" \
  -reporttypes:Html
```

Then open `template/backend/TestResults/CoverageReport/index.html` in your browser.

---

## 7. Building for Production

### Build the solution in Release mode

```bash
cd template/backend
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release
```

### Publish a self-contained deployment

```bash
dotnet publish src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj \
  --configuration Release \
  --output ./publish
```

The published output will be in `./publish/`. Run it with:

```bash
cd publish
dotnet Ambev.DeveloperEvaluation.WebApi.dll
```

### Build the Docker image

```bash
cd template/backend
docker build -t ambev-developer-evaluation:latest -f src/Ambev.DeveloperEvaluation.WebApi/Dockerfile .
```

### Run the Docker image standalone

```bash
docker run -d \
  -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n" \
  -e "ASPNETCORE_ENVIRONMENT=Development" \
  ambev-developer-evaluation:latest
```

---

## 8. Docker Deployment

### Full-stack deployment with Docker Compose

```bash
cd template/backend

# Build and start all services (WebAPI + PostgreSQL + MongoDB + Redis)
docker compose up --build -d

# View logs
docker compose logs -f ambev.developerevaluation.webapi

# Stop and remove all containers
docker compose down

# Stop and remove all containers AND volumes (destroys data)
docker compose down -v
```

### Container inventory

| Container Name | Service | Ports |
|---|---|---|
| `ambev_developer_evaluation_webapi` | ASP.NET Core 8 Web API | 8080 (HTTP) |
| `ambev_developer_evaluation_database` | PostgreSQL 13 | 5432 (internal) |
| `ambev_developer_evaluation_nosql` | MongoDB 8.0 | 27017 (internal) |
| `ambev_developer_evaluation_cache` | Redis 7.4.1 | 6379 (internal) |

---

## 9. Environment Variables & Configuration Reference

The application reads configuration from `appsettings.json` and environment variables. Environment variables override JSON settings using the `__` (double underscore) separator.

| Variable | Default | Description |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Set to `Development` for Swagger UI and detailed errors |
| `ASPNETCORE_HTTP_PORTS` | `8080` (Docker) / `5119` (local) | HTTP port |
| `ASPNETCORE_HTTPS_PORTS` | `8081` | HTTPS port (Docker) |
| `ConnectionStrings__DefaultConnection` | See `appsettings.json` | PostgreSQL connection string |
| `Jwt__SecretKey` | (see `appsettings.json`) | JWT signing key (min 32 bytes) |
| `Logging__LogLevel__Default` | `Information` | Serilog default log level |

### Example: Override the connection string via environment variable

```bash
export ConnectionStrings__DefaultConnection="Host=myserver;Port=5432;Database=mydb;Username=myuser;Password=mypass"
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

---

## 10. Troubleshooting Common Issues

### ❌ `Npgsql.NpgsqlException: Failed to connect`

**Cause:** PostgreSQL is not running or the connection string is incorrect.

**Fix:**
1. Ensure Docker containers are running: `docker compose ps`
2. If running locally (not via Docker Compose), check that the `DefaultConnection` in `appsettings.json` points to the correct host and port.
3. If using Docker Compose, note that ports are randomized on the host. Use `docker compose port ambev.developerevaluation.database 5432` to find the actual host port.

---

### ❌ `docker: Error response from daemon: Conflict. The container name ... is already in use`

**Cause:** A container with the same name already exists (from a previous run).

**Fix:**
```bash
docker compose down
docker compose up --build -d
```

---

### ❌ Integration tests fail with `Docker is not running` or `Cannot connect to the Docker daemon`

**Cause:** Testcontainers requires a running Docker daemon to provision ephemeral PostgreSQL containers.

**Fix:**
1. Start Docker Desktop (Windows/macOS) or the Docker service (Linux: `sudo systemctl start docker`)
2. Ensure your user is in the `docker` group: `sudo usermod -aG docker $USER` (Linux only, then log out/in)

---

### ❌ `The configured user limit (128) on the number of inotify instances has been reached`

**Cause:** Too many file watchers on Linux (common with `dotnet watch`).

**Fix:**
```bash
echo fs.inotify.max_user_instances=524288 | sudo tee -a /etc/sysctl.conf
sudo sysctl -p
```

---

### ❌ `AutoMapper license key` warnings in logs

**Cause:** AutoMapper 16+ requires a license key. The development configuration includes a key in `appsettings.Development.json`.

**Fix:** Ensure `ASPNETCORE_ENVIRONMENT=Development` is set when running locally so the Development configuration is loaded.

---

### ❌ EF Core migration errors on startup

**Cause:** The application runs migrations automatically with up to 5 retry attempts. If the database container is still starting, the retries may not be enough.

**Fix:**
1. Wait for the PostgreSQL container to be healthy: `docker compose ps`
2. Alternatively, apply migrations manually before starting the app:
   ```bash
   dotnet ef database update \
     --project src/Ambev.DeveloperEvaluation.ORM \
     --startup-project src/Ambev.DeveloperEvaluation.WebApi
   ```

---

### ❌ `Port 5119 is already in use`

**Cause:** Another process is using the default Kestrel port.

**Fix:**
```bash
# Find the process using the port
lsof -i :5119   # Linux/macOS
netstat -ano | findstr :5119   # Windows

# Kill the process or change the port:
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi --urls "http://localhost:5200"
```

---

### ❌ Tests pass locally but fail in CI

**Common causes:**
1. **Docker not available in CI:** Integration tests need Docker. Ensure your CI runner supports Docker-in-Docker or has Docker installed.
2. **Parallel execution conflicts:** Integration tests are configured to run sequentially. Ensure the CI pipeline does not force parallel execution.
3. **Resource limits:** Testcontainers may exceed memory limits in constrained CI environments. Increase runner resources if needed.

---

### 💡 Useful Commands Quick Reference

```bash
# Restore packages
dotnet restore Ambev.DeveloperEvaluation.sln

# Build (Debug)
dotnet build Ambev.DeveloperEvaluation.sln

# Build (Release)
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release

# Run the API (development)
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi --launch-profile http

# Run all tests
dotnet test Ambev.DeveloperEvaluation.sln --verbosity normal

# Run only unit tests
dotnet test tests/Ambev.DeveloperEvaluation.Unit/Ambev.DeveloperEvaluation.Unit.csproj

# Run only integration tests (requires Docker)
dotnet test tests/Ambev.DeveloperEvaluation.Integration/Ambev.DeveloperEvaluation.Integration.csproj

# Run only functional tests
dotnet test tests/Ambev.DeveloperEvaluation.Functional/Ambev.DeveloperEvaluation.Functional.csproj

# Docker Compose — start all
docker compose up --build -d

# Docker Compose — stop all
docker compose down

# Docker Compose — view logs
docker compose logs -f
```
