# FullStack.Api (ASP.NET Core Minimal API)

ASP.NET Identity, JWT in **HttpOnly cookies**, SOLID `IService`/`Service` layers, manual DTO mapping, Serilog, Swagger, **PostgreSQL** (Docker).

## Prerequisites
- .NET SDK 11
- Docker Desktop (for Postgres)

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
cd aspnet-api/FullStack.Api
dotnet run --launch-profile http
```
- Swagger: http://localhost:8081/swagger
- Frontend uses `withCredentials` against port `8081`

## Security
- Passwords via **ASP.NET Identity**
- Access + refresh JWTs as **HttpOnly** cookies (not in JSON / localStorage)
- CORS restricted + `AllowCredentials`

## Architecture
```
Domain/Entities
Application/DTOs + Mapping (manual) + Interfaces + Services
Infrastructure/Data + Auth + Storage
Endpoints/*Endpoints.cs
```

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
