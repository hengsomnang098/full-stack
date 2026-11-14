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
| `Database:InitMode` | `EnsureCreated` (default), `Migrate`, or `None` |
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

## Notes
- First run with `EnsureCreated` creates schema + seeds admin.
- Old MySQL `database/table.sql` is legacy reference only; schema is owned by EF now.
- `node-api` remains for reference.
