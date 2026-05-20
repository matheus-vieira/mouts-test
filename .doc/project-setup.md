# Quick Start Guide

Get the **Ambev Developer Evaluation — Sales API** up and running in under 5 minutes.

---

## Prerequisites

Make sure these are installed before you begin:

| Tool | Version | Install |
|---|---|---|
| .NET SDK | **8.0+** | [download](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Docker | **20.10+** | [download](https://docs.docker.com/get-docker/) |
| Docker Compose | **v2+** | Included with Docker Desktop; Linux: [install plugin](https://docs.docker.com/compose/install/linux/) |
| Git | **2.x+** | [download](https://git-scm.com/) |

Verify everything is ready:

```bash
dotnet --version          # expect 8.0.x
docker --version          # expect 20.10+
docker compose version    # expect v2.x+
```

---

## 1 — Clone the Repository

```bash
git clone https://github.com/matheus-vieira/mouts-test.git
cd mouts-test/template/backend
```

> All commands below assume you are inside `template/backend/`.

---

## 2 — Restore Dependencies

```bash
dotnet restore Ambev.DeveloperEvaluation.sln
```

Expected output ends with `Restore completed in …`.

---

## 3 — Build the Project

```bash
dotnet build Ambev.DeveloperEvaluation.sln --configuration Release --no-restore
```

Look for `Build succeeded. 0 Warning(s) 0 Error(s)` at the end.

---

## 4 — Start Infrastructure Services

Spin up PostgreSQL, MongoDB, and Redis using Docker Compose:

```bash
docker compose up -d
```

Wait for PostgreSQL to become healthy (this usually takes 5–15 seconds):

```bash
docker compose ps
```

You should see all containers running and the database marked as **healthy**:

```
NAME                                    STATUS
ambev_developer_evaluation_database     running (healthy)
ambev_developer_evaluation_nosql        running
ambev_developer_evaluation_cache        running
ambev_developer_evaluation_webapi       running
```

### Services Started

| Service | Container | Internal Port | Credentials |
|---|---|---|---|
| PostgreSQL 13 | `ambev_developer_evaluation_database` | 5432 | user: `developer` · password: `ev@luAt10n` · db: `developer_evaluation` |
| MongoDB 8.0 | `ambev_developer_evaluation_nosql` | 27017 | user: `developer` · password: `ev@luAt10n` |
| Redis 7.4 | `ambev_developer_evaluation_cache` | 6379 | password: `ev@luAt10n` |

---

## 5 — Run the Application

You have **two options** — pick whichever fits your workflow:

### Option A: Run via Docker Compose (already done)

If you ran `docker compose up -d` in Step 4, the WebAPI container is already running. The API is available at:

| | URL |
|---|---|
| **API base** | `http://localhost:8080` |
| **Swagger UI** | `http://localhost:8080/swagger` |

Skip to **Step 6** to verify.

### Option B: Run locally with the .NET CLI

Use this if you prefer running the API outside Docker (e.g., for debugging). First, stop the WebAPI container so the local process has access to the same ports:

```bash
docker compose stop ambev.developerevaluation.webapi
```

Then start the API locally:

```bash
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi --launch-profile http
```

The API is available at:

| | URL |
|---|---|
| **API base** | `http://localhost:5119` |
| **Swagger UI** | `http://localhost:5119/swagger` |

> **Note:** When running locally, the app connects to PostgreSQL on `localhost:5432`. The `docker-compose.yml` assigns a random host port for the database by default. To fix this, either:
> - Edit `docker-compose.yml` and change the database ports to `"5432:5432"`, **or**
> - Override the connection string:
>   ```bash
>   export ConnectionStrings__DefaultConnection="Host=localhost;Port=$(docker compose port ambev.developerevaluation.database 5432 | cut -d: -f2);Database=developer_evaluation;Username=developer;Password=ev@luAt10n"
>   dotnet run --project src/Ambev.DeveloperEvaluation.WebApi --launch-profile http
>   ```

---

## 6 — Verify the Setup

### 6.1 Health Check

```bash
curl -s http://localhost:8080/health | head
```

A successful response returns HTTP `200 OK` with a JSON body indicating the health status.

> Replace `8080` with `5119` if running locally via .NET CLI.

### 6.2 Swagger UI

Open your browser and navigate to:

```
http://localhost:8080/swagger
```

You should see the interactive Swagger documentation listing all available endpoints (Users, Sales, Auth).

### 6.3 Quick Smoke Test — Create a User

```bash
curl -s -X POST http://localhost:8080/api/Users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test@1234",
    "phone": "+5511999999999",
    "email": "test@example.com",
    "status": "Active",
    "role": "Customer"
  }' | python3 -m json.tool
```

A `201 Created` response confirms the API, database connection, and migrations are all working.

---

## 7 — Stop Everything

### Stop all containers (preserves data)

```bash
docker compose down
```

### Stop and delete all data (volumes)

```bash
docker compose down -v
```

### Stop only the local .NET process

Press `Ctrl+C` in the terminal where `dotnet run` is executing.

---

## Troubleshooting

### Database connection refused

```
Npgsql.NpgsqlException: Failed to connect to 127.0.0.1:5432
```

**Cause:** PostgreSQL container is not running or hasn't finished starting.

**Fix:**
```bash
docker compose ps                          # check container status
docker compose logs ambev.developerevaluation.database   # check for errors
docker compose up -d ambev.developerevaluation.database  # restart if needed
```

---

### Port 8080 already in use

```
Error: failed to create endpoint ... bind: address already in use
```

**Fix:** Stop whatever is using port 8080, or change the mapping in `docker-compose.yml`:

```yaml
ports:
  - "9090:8080"   # map to host port 9090 instead
```

---

### Docker Compose fails to build the WebAPI image

```
ERROR: Service 'ambev.developerevaluation.webapi' failed to build
```

**Fix:** Make sure you are in the `template/backend/` directory (where `docker-compose.yml` and `Dockerfile` live):

```bash
cd template/backend
docker compose up --build -d
```

---

### Migrations fail on startup

The app retries database migrations up to 5 times automatically. If it still fails:

```bash
# Apply manually
dotnet ef database update \
  --project src/Ambev.DeveloperEvaluation.ORM \
  --startup-project src/Ambev.DeveloperEvaluation.WebApi
```

---

### Container name conflict

```
Conflict. The container name "/ambev_developer_evaluation_database" is already in use
```

**Fix:**
```bash
docker compose down
docker compose up -d
```

---

## What's Next?

- **API Documentation:** Open Swagger UI at `/swagger` to explore all endpoints
- **Running Tests:** See [./running-tests.md](./running-tests.md) for unit, integration, and functional test instructions
