# DealRoom

A B2B workspace where two companies manage a deal in one place — chat, documents, approvals, and status tracking.

## Problem

Small and medium businesses run deals across WhatsApp, email, Excel, and 1C. Context gets lost, documents are scattered, and there's no single source of truth for what was agreed and when. DealRoom puts the entire deal lifecycle in one workspace shared between counterparties.

## Features

- **Auth** — registration creates an organization + user; JWT access tokens (15 min) with rotating refresh tokens (7 days).
- **Deals** — create a deal between two organizations with a `Draft → Negotiation → Active → Closed/Cancelled` state machine. Only the two participating organizations can view or change a deal.
- **Documents** — upload files to MinIO; the counterparty (never the uploader) approves or rejects each one.
- **Real-time chat** — per-deal chat over SignalR; status changes are pushed to both sides live.
- **Hardening** — Serilog structured logging, per-IP rate limiting on auth, `/health` endpoint, CORS scoped to the frontend.

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
| Logging | Serilog |
| Tests | xUnit + Testcontainers |
| Infrastructure | Docker Compose, GitHub Actions CI |

## Architecture

Clean Architecture with strict dependency direction:

```
Api  →  Infrastructure  →  Core
```

- **`DealRoom.Core`** — domain entities, repository/service interfaces, domain exceptions. No external dependencies.
- **`DealRoom.Infrastructure`** — EF Core, repositories, MinIO client, JWT issuer.
- **`DealRoom.Api`** — HTTP endpoints, SignalR hub, DI wiring, middleware pipeline.
- **`DealRoom.Tests`** — xUnit unit tests (service logic via in-memory fakes) and Testcontainers integration tests (real Postgres + HTTP).

Authorization lives in the services: every service call takes the requesting organization id and verifies participation, so endpoints stay thin.

## API

| Method | Route | Purpose |
|---|---|---|
| POST | `/auth/register` · `/auth/login` · `/auth/refresh` | Auth (rate-limited) |
| GET | `/organizations/me` | Current organization |
| GET · POST | `/deals` | List / create deals |
| GET · PATCH | `/deals/{id}` · `/deals/{id}/status` | Detail / change status |
| GET · POST | `/deals/{id}/documents` | List / upload documents |
| GET · POST | `/documents/{id}/content` · `/approve` · `/reject` | Download / review |
| GET · POST | `/deals/{id}/messages` | Chat history / send |
| WS | `/hubs/chat` | SignalR: `JoinDeal`, `ReceiveMessage`, `DealStatusChanged` |
| GET | `/health` | Liveness probe |

## Quickstart

```bash
cp .env.example .env
docker compose up -d postgres minio
dotnet ef database update --project src/DealRoom.Infrastructure --startup-project src/DealRoom.Api
dotnet run --project src/DealRoom.Api          # Swagger at /swagger

cd frontend
npm install
npm run dev                                     # http://localhost:3000
```

Or run the whole stack (API + Postgres + MinIO) with `docker compose up -d`.

## Tests

```bash
dotnet test
```

Unit tests run everywhere. Integration tests spin up a throwaway PostgreSQL via Testcontainers and are skipped automatically when Docker isn't available.

## Deployment

- **API** — containerized (`Dockerfile`), deploys to Railway / Render / Fly. Configure via environment variables:
  `ConnectionStrings__Default`, `Jwt__Secret`, `Minio__Endpoint/AccessKey/SecretKey`, and `Cors__AllowedOrigins__0` (the deployed frontend URL).
- **Frontend** — deploys to Vercel; set `NEXT_PUBLIC_API_URL` to the deployed API.

## License

MIT
