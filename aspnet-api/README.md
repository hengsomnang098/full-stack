# FullStack.Api (ASP.NET Core Minimal API)

ASP.NET Identity, JWT in **HttpOnly cookies**, Clean Architecture layers, manual DTO mapping, Serilog, Swagger, **PostgreSQL** (Docker).

## Prerequisites
- .NET SDK 11
- Docker Desktop (for Postgres)

## Solution layout

```
aspnet-api/
  FullStack.slnx
  FullStack.Domain/           # Entities only (no outward deps)
  FullStack.Application/      # DTOs, interfaces, services, mapping
  FullStack.Infrastructure/   # EF Core, Identity, JWT, file storage
  FullStack.Api/              # Endpoints, Program, host DI/CORS/Swagger
```

Dependency rule:

```
Api → Infrastructure → Application → Domain
Api → Application
```

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Entities (`Category`, `Product`, `ApplicationUser`, …) |
| **Application** | Use cases (`*Service`), DTOs, mappers, `IAppDbContext` / port interfaces |
| **Infrastructure** | `AppDbContext`, JWT/cookie auth, uploads, Identity stores, `AddInfrastructure()` |
| **Api** | Minimal API endpoints, Serilog, CORS, Swagger, startup/seed |

## Start PostgreSQL
From the repo root:

```powershell
docker compose up -d
```

Defaults:
- Host `localhost:5432`
- Database `full_stack`
- User / password `fullstack` / `fullstack`

## Configure
`FullStack.Api/appsettings.json` already points at the Docker Postgres instance.

| Key | Description |
|-----|-------------|
| `ConnectionStrings:DefaultConnection` | Npgsql connection string |
| `Database:InitMode` | `Migrate` (default), `EnsureCreated`, or `None` |
| `Jwt:*` | Secrets (≥ 32 chars), cookie names, lifetimes |
| `Cors:AllowedOrigins` | Frontend origins (credentials allowed) |
| `Seed:*` | Admin tel `099998888` / password `Admin@12345` |

## Run API
```powershell
cd aspnet-api
dotnet run --project FullStack.Api --launch-profile http
```
- Swagger: http://localhost:8081/swagger
- Frontend uses `withCredentials` against port `8081`

Or open `FullStack.slnx` in your IDE and run the `FullStack.Api` project.

## Security
- Passwords via **ASP.NET Identity**
- Access + refresh JWTs as **HttpOnly** cookies (not in JSON / localStorage)
- CORS restricted + `AllowCredentials`

## Auth
| Method | Path |
|--------|------|
| POST | `/api/employee/login` |
| POST | `/api/employee/refreshtoken` |
| POST | `/api/employee/logout` |
| POST | `/api/employee/setPassword` |
| GET | `/api/employee/me` |

## EF Core migrations

Migrations live in **Infrastructure** (`FullStack.Infrastructure/Data/Migrations`), next to `AppDbContext`.

From `aspnet-api/`:

```powershell
# Add a migration after model changes
dotnet ef migrations add <Name> --project FullStack.Infrastructure --startup-project FullStack.Api --output-dir Data/Migrations

# Apply to the database (or just run the API with Database:InitMode=Migrate)
dotnet ef database update --project FullStack.Infrastructure --startup-project FullStack.Api

# Remove the last unapplied migration
dotnet ef migrations remove --project FullStack.Infrastructure --startup-project FullStack.Api
```

`AppDbContextFactory` supports design-time tooling without starting Kestrel. At runtime, `DbSeeder` calls `MigrateAsync()` when `Database:InitMode` is `Migrate`.

**Note:** Do not switch an existing DB that was created with `EnsureCreated` over to `Migrate` on the same database — drop it first (or baseline `__EFMigrationsHistory`). Fresh Docker volume + `Migrate` is the intended path.

## Notes
- First run with `Migrate` applies `InitialCreate` and seeds admin.
- Old MySQL `database/table.sql` is legacy reference only; schema is owned by EF now.
- `node-api` remains for reference.
