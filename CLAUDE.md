# DealRoom

B2B platform where two companies manage a deal in one place: chat, documents, approvals.

## Commands

```bash
dotnet build                                    # build solution
dotnet test                                     # run all tests
dotnet test --filter "FullyQualifiedName~Name"  # run specific test
dotnet run --project src/DealRoom.Api           # start API (http://localhost:8080)
dotnet format                                   # format code
dotnet build --warnaserror                      # treat warnings as errors

docker compose up -d                            # start postgres + minio + api
docker compose down                             # stop all services
docker compose logs -f api                      # stream API logs
```

## Architecture

Clean Architecture — dependency direction: Api → Infrastructure → Core.

- `src/DealRoom.Core/` — domain entities, interfaces, no external dependencies
- `src/DealRoom.Infrastructure/` — EF Core, repositories, MinIO client
- `src/DealRoom.Api/` — ASP.NET Core Minimal API, endpoints, DI wiring
- `tests/DealRoom.Tests/` — xUnit integration tests

## Key Decisions

- .NET 8 LTS, pinned via `global.json`
- Minimal API (not MVC) — less ceremony
- PostgreSQL via EF Core (code-first, migrations)
- MinIO for file storage (S3-compatible, runs in Docker)
- JWT: access token (15m) + refresh token (7d)
- SignalR for real-time chat

## Don'ts

- Don't add business logic to Api layer — it belongs in Core services
- Don't access DbContext directly from Api — go through repositories
- Don't modify `obj/` or `bin/`
- Don't commit `.env` — use `.env.example` as template
