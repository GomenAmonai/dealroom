# DealRoom

A B2B workspace where two companies manage a deal in one place — chat, documents, approvals, and status tracking.

## Problem

Small and medium businesses run deals across WhatsApp, email, Excel, and 1C. Context gets lost, documents are scattered, and there's no single source of truth for what was agreed and when. DealRoom puts the entire deal lifecycle in one workspace shared between counterparties.

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 (Minimal API) |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL 16 |
| Auth | JWT (access + refresh tokens) |
| Real-time | SignalR |
| File storage | MinIO (S3-compatible) |
| Frontend | Next.js 14 + TypeScript + Tailwind |
| Infrastructure | Docker Compose, GitHub Actions CI |

## Architecture

Clean Architecture with strict dependency direction:

```
Api  →  Infrastructure  →  Core
```

- **`DealRoom.Core`** — domain entities, repository and service interfaces. No external dependencies.
- **`DealRoom.Infrastructure`** — EF Core, repositories, MinIO client, JWT issuer.
- **`DealRoom.Api`** — HTTP endpoints, DI wiring, middleware pipeline.
- **`DealRoom.Tests`** — xUnit integration tests.

## Quickstart

```bash
git clone https://github.com/GomenAmonai/dealroom.git
cd dealroom
cp .env.example .env

docker compose up -d postgres minio
dotnet ef database update --project src/DealRoom.Infrastructure --startup-project src/DealRoom.Api
dotnet run --project src/DealRoom.Api
```

API will be available at `http://localhost:5089`. Swagger UI at `/swagger`.

## Status

MVP in progress. See [project plan](https://github.com/GomenAmonai/dealroom/issues) for current scope.

## License

MIT
