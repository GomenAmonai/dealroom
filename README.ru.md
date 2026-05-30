> 🇬🇧 English version: [README.md](README.md)

# DealRoom

Рабочее пространство для B2B-сделок: две компании ведут одну сделку в одном месте — документы, согласования, чат и статусы.

## Проблема

Малый и средний бизнес ведёт сделки вперемешку: WhatsApp, почта, Excel, 1С. Контекст теряется, документы разбросаны, нет единой точки правды о том, что и когда согласовали. DealRoom собирает весь жизненный цикл сделки в одно пространство, общее для обеих сторон.

## Возможности

- **Аутентификация** — при регистрации создаётся организация + пользователь; JWT access-токен (15 мин) с ротацией refresh-токена (7 дней).
- **Сделки** — между двумя организациями, с конечным автоматом статусов `Draft → Negotiation → Active → Closed/Cancelled`. Видеть и менять сделку могут только две стороны-участницы.
- **Документы** — загрузка в MinIO; согласует или отклоняет **контрагент** (не тот, кто загрузил).
- **Чат в реальном времени** — на сделку, через SignalR; смена статуса прилетает обеим сторонам мгновенно.
- **Прод-обвязка** — структурное логирование Serilog, rate-limit на `/auth` по IP, `/health`, CORS под фронтенд.

## Стек

| Слой | Технология |
|---|---|
| Backend | ASP.NET Core 8 (Minimal API) |
| ORM | Entity Framework Core 8 |
| База | PostgreSQL 16 |
| Auth | JWT (access + refresh) |
| Real-time | SignalR |
| Файлы | MinIO (S3-совместимое) |
| Frontend | Next.js 14 + TypeScript + Tailwind |
| Логи | Serilog |
| Тесты | xUnit + Testcontainers |
| Инфраструктура | Docker Compose, GitHub Actions CI |

## Архитектура

Clean Architecture со строгим направлением зависимостей:

```
Api  →  Infrastructure  →  Core
```

- **`DealRoom.Core`** — доменные сущности, интерфейсы репозиториев/сервисов, доменные исключения. Без внешних зависимостей.
- **`DealRoom.Infrastructure`** — EF Core, репозитории, клиент MinIO, выпуск JWT.
- **`DealRoom.Api`** — HTTP-эндпоинты, SignalR-хаб, DI, пайплайн middleware.
- **`DealRoom.Tests`** — юнит-тесты сервисов (на in-memory фейках) и интеграционные тесты на Testcontainers (реальный Postgres + HTTP).

Авторизация живёт в сервисах: каждый вызов принимает id организации-инициатора и проверяет участие в сделке, поэтому эндпоинты остаются тонкими.

## API

| Метод | Путь | Назначение |
|---|---|---|
| POST | `/auth/register` · `/auth/login` · `/auth/refresh` | Аутентификация (rate-limited) |
| GET | `/organizations/me` | Текущая организация |
| GET · POST | `/deals` | Список / создать сделку |
| GET · PATCH | `/deals/{id}` · `/deals/{id}/status` | Детали / сменить статус |
| GET · POST | `/deals/{id}/documents` | Список / загрузить документ |
| GET · POST | `/documents/{id}/content` · `/approve` · `/reject` | Скачать / согласовать |
| GET · POST | `/deals/{id}/messages` | История чата / отправить |
| WS | `/hubs/chat` | SignalR: `JoinDeal`, `ReceiveMessage`, `DealStatusChanged` |
| GET | `/health` | Проба живости |

## Быстрый старт

```bash
cp .env.example .env
docker compose up -d postgres minio
dotnet ef database update --project src/DealRoom.Infrastructure --startup-project src/DealRoom.Api
dotnet run --project src/DealRoom.Api          # Swagger на /swagger

cd frontend
npm install
npm run dev                                     # http://localhost:3000
```

Или поднять весь стек (API + Postgres + MinIO) одной командой: `docker compose up -d`.

## Тесты

```bash
dotnet test
```

Юнит-тесты идут везде. Интеграционные поднимают временный PostgreSQL через Testcontainers и автоматически пропускаются, если Docker недоступен.

## Деплой

- **API** — контейнеризован (`Dockerfile`), под Railway / Render / Fly. Конфиг через переменные окружения:
  `ConnectionStrings__Default`, `Jwt__Secret`, `Minio__Endpoint/AccessKey/SecretKey`, `Cors__AllowedOrigins__0` (URL задеплоенного фронта).
- **Frontend** — под Vercel; `NEXT_PUBLIC_API_URL` указывает на задеплоенный API.

## Ограничения (честно: это MVP)

Это вертикальный срез вокруг окна сделки. Сознательно **не сделано**:

- личный профиль, настройки, управление организацией и её участниками, дашборд;
- роли внутри организации (сейчас все участники равны);
- подпись документов, эскроу/гарант, уведомления, audit-log;
- пагинация списков и системная валидация DTO;
- токены хранятся в `localStorage` (риск XSS) — для прода нужен httpOnly-cookie.

## Статус

Учебный / портфолио-проект. MVP завершён; дальнейшее развитие приостановлено.

## Лицензия

MIT
