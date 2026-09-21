# D&D Backend API

A modern, robust REST API for a Dungeons & Dragons web application built with **.NET 8**, **C# 12**, **Clean Architecture**, and **PostgreSQL (Supabase)**.

---

## 🏗️ Architecture

The solution follows **Clean Architecture** principles to ensure separation of concerns, testability, and maintainability:

```text
DndBackend.sln
│
└── src/
    ├── Dnd.Domain/          # Core entities, enums, business rules (Zero external dependencies)
    ├── Dnd.Application/     # Use cases, interfaces, DTOs, service contracts
    ├── Dnd.Infrastructure/  # EF Core, PostgreSQL (Supabase) configurations, migrations
    └── Dnd.Api/             # ASP.NET Core controllers, Swagger, Program.cs entry point
```

---

## 🚀 Getting Started

You can run the project in two ways:
- **Option A (Recommended)**: Run with **Docker** (no .NET SDK installation required).
- **Option B**: Run natively with the **.NET 8 SDK**.

---

### Option A: Running with Docker (Recommended)

#### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or Docker Engine + Docker Compose.

#### 1. Clone the repository
```bash
git clone <repository-url>
cd dnd-be
```

#### 2. Configure Environment Variables
Copy `.env.example` to `.env` and fill in your database password:
```bash
cp .env.example .env
```
*(The `.env` file is git-ignored so your password will never be committed).*

#### 3. Start the application
```bash
docker compose up -d --build
```

#### 4. Access Swagger UI
Open your browser and navigate to:
👉 **[http://localhost:5207/swagger](http://localhost:5207/swagger)**

#### Useful Docker Commands
```bash
# View live logs
docker compose logs -f

# Stop the container
docker compose down

# Rebuild after making changes
docker compose up -d --build
```

---

### Option B: Running Locally with .NET 8 CLI

#### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

#### 1. Clone the repository
```bash
git clone <repository-url>
cd dnd-be
```

#### 2. Configure Database Connection
Create or edit `src/Dnd.Api/appsettings.Development.json` and add your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-db-host;Port=5432;Database=your-db;Username=your-user;Password=your-password;"
  }
}
```

> **Note**: `appsettings.Development.json` is git-ignored to protect your credentials.

#### 3. Build & Run
Using the included development helper script:
```bash
# Build the entire solution
./dev.sh build

# Run the API project
./dev.sh run
```

*Or using standard `dotnet` CLI:*
```bash
# Restore & build
dotnet build DndBackend.sln

# Run API
dotnet run --project src/Dnd.Api
```

#### 4. Access Swagger UI
Once started, open:
👉 **[http://localhost:5207/swagger](http://localhost:5207/swagger)**

---

## ⚙️ Configuration & Environment Variables

| Variable | Description | Default |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Application environment (`Development` or `Production`) | `Development` |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | Configured in `appsettings.Development.json` or `docker-compose.yml` |
| `PORT` | Host port mapped by Docker Compose | `5207` |
| `EnableSwagger` | Enable Swagger UI in non-development environments | `false` |

---

## ☁️ Deploying to Google Cloud Run

This project is pre-configured for Google Cloud Run:
- **Port**: Listens on port `8080` (Cloud Run default).
- **SSL Termination**: Configured with `ForwardedHeaders` to avoid HTTPS redirect loops.
- **Health Checks**: Exposes `/health` returning `200 OK` for startup and liveness probes.

### Deploying via gcloud CLI

```bash
gcloud run deploy dnd-api \
  --source . \
  --region europe-west1 \
  --allow-unauthenticated \
  --set-env-vars "ConnectionStrings__DefaultConnection=Host=aws-1-eu-west-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.ibtmthglqgblnklalguh;Password=YOUR_PASSWORD;Pooling=true;,EnableSwagger=true"
```

*(Tip: For production, store `ConnectionStrings__DefaultConnection` in Google Cloud Secret Manager instead of plain environment variables).*

---

## 📜 Project Structure

```text
dnd-be/
├── DndBackend.sln                # Root solution file
├── Dockerfile                    # Multi-stage production Docker build
├── docker-compose.yml            # Local container orchestration
├── .dockerignore                 # Excluded Docker build context files
├── .gitignore                    # Git ignore file (excludes secrets & build artifacts)
├── dev.sh                        # Development helper runner
├── README.md                     # Project setup guide (this file)
└── src/
    ├── Dnd.Domain/               # Pure domain models & enums
    ├── Dnd.Application/          # DTOs, interfaces, business services
    ├── Dnd.Infrastructure/       # EF Core, DbContext, PostgreSQL
    └── Dnd.Api/                  # Web API controllers & startup
```

