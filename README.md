# Melpominee

A Vampire: The Masquerade V5 character sheet application with real-time multi-user collaboration.

## Features

- Create and manage VTM V5 characters (attributes, skills, disciplines, merits/flaws, beliefs, profile)
- Real-time collaboration — multiple users can view and edit a character sheet simultaneously via SignalR
- Cookie-based authentication with email/password registration, password reset, and Discord OAuth2
- VTM V5 masterdata (clans, disciplines, predator types, blood potency, resonances) served from embedded JSON
- Redis-backed distributed caching and SignalR backplane for multi-instance deployments
- Swagger/OpenAPI documentation (development mode)

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core (.NET 9), Dapper, Npgsql (PostgreSQL), StackExchange.Redis, SignalR |
| Frontend | React 18, TypeScript 4.9, Vite 4, Redux Toolkit v1, SCSS |
| Infrastructure | Docker Compose, Traefik, Kubernetes (Azure AKS), Envoy Gateway, GitHub Actions |
| Container Registry | GHCR (GitHub Container Registry) |

## Getting Started

### Prerequisites

- Node.js (LTS)
- .NET 9 SDK
- Docker and Docker Compose (for full-stack or infrastructure services)
- PostgreSQL and Redis (or use Docker Compose to run them)

### Local Development

All root npm scripts delegate to `src/frontend/` via `--prefix`.

**Frontend:**

```bash
npm install
npm run dev        # Vite dev server with API proxy
npm run build      # TypeScript compile + Vite production build
npm run lint       # ESLint with auto-fix
npm run format     # Prettier
npm run preview    # Preview production build locally
```

A pre-commit hook (Husky + lint-staged) runs Prettier on staged `.js/.jsx/.ts/.tsx` files automatically. Hooks are installed via `npm install`.

**Backend:**

```bash
dotnet build src/backend/Melpominee.app.csproj
dotnet run --project src/backend/Melpominee.app.csproj
```

The backend reads secrets from JSON files in a `secrets/` directory at the project root. See [Secrets](#secrets) below.

### Docker Compose (Full Stack)

Starts Traefik (port 8010; dashboard on 8005), frontend, backend, PostgreSQL, Redis, and pgAdmin (port 8006):

```bash
docker compose up -d
```

Note: Docker Compose uses Traefik with `/backend` and `/frontend` path prefixes (e.g., `http://localhost:8010/backend/vtmv5/character`), unlike production which uses `/api`.

### Secrets

The backend loads secrets from `/etc/melpominee/secrets/` (Docker/Kubernetes) or `./secrets/` (local development). Create the following JSON files in `secrets/`:

| File | Keys |
|------|------|
| `pg-credentials.json` | `db_host`, `db_user`, `db_password`, `db_database` |
| `redis-credentials.json` | `redis_host`, `redis_port` |
| `discord-oauth.json` | `discord_clientid`, `discord_clientsecret` |
| `mail-secrets.json` | `mail_host`, `mail_address`, `mail_password` |

Docker Compose also requires `pg-password.txt` (plain-text PostgreSQL superuser password, used by the `database` container only — not loaded by the backend application). Note that `docker-compose.yaml` references secret files at `/secrets/` (absolute host path) — adjust these paths for your environment.

## Project Structure

```
melpominee/
├── src/
│   ├── backend/                  # ASP.NET Core (.NET 9) Web API
│   │   ├── Authorization/        # Custom auth handlers and policies
│   │   ├── Controllers/          # Auth + VTM V5 character endpoints
│   │   ├── Models/               # Domain models, DTOs, update classes
│   │   ├── Services/             # Database, auth, character, mail services
│   │   ├── Hubs/                 # SignalR hub for real-time updates
│   │   ├── Data/                 # Embedded JSON masterdata resources
│   │   └── Program.cs            # Entry point, DI, middleware pipeline
│   └── frontend/                 # React 18 SPA
│       └── src/
│           ├── assets/           # Static assets (images, fonts)
│           ├── components/       # Route and shared components
│           ├── redux/            # Redux Toolkit store, slices, thunks
│           ├── types/            # TypeScript type declarations
│           └── util/             # Helpers and utilities
├── manifests/                    # Kubernetes deployment + HTTPRoute
├── secrets/                      # Local dev secrets (gitignored)
├── docker-compose.yaml           # Full-stack local orchestration
├── DESIGN.md                     # Detailed architecture document
└── package.json                  # Root scripts (delegates to frontend)
```

## Architecture

```
Browser
  └── HTTPS :443
      └── Envoy Gateway (melpominee.app)
          ├── /api/*  (strip prefix) → Backend (ASP.NET Core)
          │                ├── PostgreSQL (character data, users)
          │                └── Redis (cache + SignalR backplane)
          └── /*      → Frontend (React 18 SPA via nginx)
```

The Envoy Gateway strips the `/api` prefix before forwarding requests to the backend. SignalR WebSocket connections route through `/api/vtmv5/watch` to the `CharacterHub`, which uses a Redis backplane to broadcast updates across all backend instances. The frontend connects via the SignalR client and receives granular update events for each character sheet section.

See [DESIGN.md](DESIGN.md) for the full architecture document.

## API Overview

| Area | Prefix | Description |
|------|--------|-------------|
| Auth | `/auth` | `GET /auth` returns current user; other routes use `/{action}` segments (login, register, etc.) |
| Characters | `/vtmv5/character` | CRUD for characters and their sections (attributes, skills, disciplines, etc.) |
| Masterdata | `/vtmv5/masterdata/{action}` | Static VTM V5 reference data (clans, disciplines, predator types) |
| Real-time | `/vtmv5/watch` | SignalR hub for live character sheet updates |

Prefixes are relative to the backend service root. In production, the gateway exposes them under `/api/` (e.g., `/api/vtmv5/character`). Swagger UI is available at `/swagger` when running in development mode.

## Deployment

GitHub Actions CI/CD builds and pushes Docker images to GHCR on push to `master` or `v*` tags, then applies manifests to Azure AKS and triggers a rollout restart to pick up new images. The pipeline runs on a self-hosted ARC runner (`arc-melpominee`). Production uses Envoy Gateway for ingress routing. Kubernetes manifests are in `manifests/`. See `docker-compose.yaml` for the local Docker Compose setup.

## License

ISC
