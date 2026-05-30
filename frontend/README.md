# DealRoom Frontend

Next.js 14 (App Router) + TypeScript + Tailwind. Talks to the DealRoom API and
receives real-time chat and status updates over SignalR.

## Setup

```bash
npm install
npm run dev      # http://localhost:3000
```

## Configuration

Set the API base URL (defaults to `http://localhost:8080`):

```bash
# frontend/.env.local
NEXT_PUBLIC_API_URL=http://localhost:8080
```

The API's CORS policy allows `http://localhost:3000`, so run the frontend on
that port during development.

## Structure

- `src/lib/` — API client (`api.ts`), token storage (`auth.ts`), SignalR
  connection (`signalr.ts`), shared types.
- `src/app/` — routes: `login`, `register`, `deals` (list + create),
  `deals/[id]` (status, documents, chat).
- `src/components/` — `AppShell` (auth guard + header), `StatusBadge`,
  `DocumentsPanel`, `DealChat`.

Auth tokens are kept in `localStorage`; the API client refreshes the access
token automatically on a `401`.
