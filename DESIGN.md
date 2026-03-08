# Melpominee Design Document

Melpominee is a Vampire: The Masquerade V5 character sheet web application with real-time multi-user collaboration. Multiple authenticated users can view and edit a character sheet simultaneously; all changes propagate instantly to every connected session via SignalR.

## Table of Contents

1. [Tech Stack](#tech-stack)
2. [System Architecture](#system-architecture)
3. [Backend Architecture](#backend-architecture)
4. [Data Model](#data-model)
5. [Frontend Architecture](#frontend-architecture)
6. [Real-Time Synchronization Flow](#real-time-synchronization-flow)
7. [Infrastructure & Deployment](#infrastructure--deployment)
8. [Potential Improvements](#potential-improvements)

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend framework | ASP.NET Core (.NET 9) |
| Data access | Dapper + Npgsql (PostgreSQL) — no Entity Framework |
| Caching / pub-sub | StackExchange.Redis |
| Real-time | ASP.NET Core SignalR (Redis backplane) |
| Frontend framework | React 18 + TypeScript |
| Bundler | Vite 4 |
| State management | Redux Toolkit v1 + redux-thunk |
| Styling | SCSS (custom, no Tailwind) |
| Container registry | GHCR (GitHub Container Registry) |
| Orchestration | Kubernetes (Azure AKS) + Envoy Gateway |

---

## System Architecture

```
Browser
  └── HTTPS :443
      └── Envoy Gateway (HTTPRoute: melpominee.app)
          ├── /api/*  ──(strip /api prefix)──► Backend Pod (ASP.NET Core :8080)
          │                                        ├── PostgreSQL (12 tables)
          │                                        └── Redis (cache + SignalR backplane)
          └── /*      ──────────────────────────► Frontend Pod (nginx :80)
                                                      └── React 18 SPA (index.html)
```

WebSocket (SignalR): Browser → `/api/vtmv5/watch` → `CharacterHub` → Redis backplane → All backend pods → All subscribed clients

The `/api` prefix exists only at the gateway layer. The backend receives requests with the prefix stripped; controllers define routes without it (e.g., `vtmv5/character/{id}`).

**Status**: Implemented.

---

## Backend Architecture

### Middleware Pipeline

Order in `src/backend/Program.cs`:

1. Swagger (Development environment only)
2. `UseAuthentication`
3. `UseAuthorization`
4. `UseSession`
5. `MapHub<CharacterHub>("/vtmv5/watch")`
6. `MapControllers`

Session middleware runs after auth because session state is secondary to identity. `MapHub` must precede `MapControllers` to ensure the SignalR endpoint takes precedence over any wildcard controller routes.

### Service Registrations

| Service | Lifetime | File |
|---------|----------|------|
| `DataContext` | Singleton (lazy via `Lazy<T>`) | `src/backend/Services/Database/DataContext.cs` |
| `SecretManager` | Singleton | `src/backend/Services/SecretManager.cs` |
| `MailService` | Singleton | `src/backend/Services/MailService.cs` |
| `ConnectionService` | Singleton | `src/backend/Services/Hubs/ConnectionService.cs` |
| `CharacterService` | Scoped | `src/backend/Services/Characters/CharacterService.cs` |
| `UserManager` | Scoped | `src/backend/Services/Auth/UserManager.cs` |

`ConnectionService` is singleton because it holds shared in-memory state (connection and group maps) that must survive across requests and SignalR hub calls. `CharacterService` and `UserManager` are scoped because they hold per-request `IDistributedCache` dependencies.

### Controllers & Routes

All routes listed below are relative to the backend process root. The Envoy gateway strips the `/api` prefix before forwarding.

**AuthController** (`src/backend/Controllers/AuthController.cs`) — `[Route("[controller]/[action]")]`

| HTTP | Route | Auth | Purpose |
|------|-------|------|---------|
| GET | `/auth/` | None | Get current user profile from active session |
| POST | `/auth/login` | None | Email/password login; issues auth cookie |
| GET, POST | `/auth/logout` | `[Authorize]` | Invalidate session and sign out |
| POST | `/auth/reset-password` | None | Begin password reset (sends email with hashed token) |
| POST | `/auth/reset-password/confirmation` | None | Confirm reset with plaintext token; updates password |
| POST | `/auth/register` | None | Create account; sends activation email |
| GET | `/auth/register/confirmation` | None | Activate account via email link; redirects to login |
| GET | `/auth/oauth/discord` | None | Discord OAuth2 flow (redirect or callback with `?code=`) |

**CharacterController** (`src/backend/Controllers/VTMV5/CharacterController.cs`) — `[Route("vtmv5/[controller]")]`, all `[Authorize]`

| HTTP | Route | Policy | Purpose |
|------|-------|--------|---------|
| GET | `/vtmv5/character/{charId}` | `CanViewCharacter` | Get single character (full) |
| GET | `/vtmv5/character` | `[Authorize]` | List characters for current user; `?adminView=true` lists all |
| POST | `/vtmv5/character` | `[Authorize]` | Create new character |
| PUT | `/vtmv5/character/{charId}` | `CanViewCharacter` | Update character header; fires `OnHeaderUpdate` |
| GET | `/vtmv5/character/attributes/{charId}` | `CanViewCharacter` | Get attributes |
| PUT | `/vtmv5/character/attributes/{charId}` | `CanViewCharacter` | Update attributes; fires `OnAttributeUpdate` |
| GET | `/vtmv5/character/skills/{charId}` | `CanViewCharacter` | Get skills |
| PUT | `/vtmv5/character/skills/{charId}` | `CanViewCharacter` | Update skills; fires `OnSkillUpdate` |
| GET | `/vtmv5/character/stats/{charId}` | `CanViewCharacter` | Get secondary stats |
| PUT | `/vtmv5/character/stats/{charId}` | `CanViewCharacter` | Update secondary stats; fires `OnSecondaryUpdate` |
| GET | `/vtmv5/character/disciplines/{charId}` | `CanViewCharacter` | Get disciplines |
| PUT | `/vtmv5/character/disciplines/{charId}` | `CanViewCharacter` | Update disciplines; fires `OnDisciplineUpdate` |
| GET | `/vtmv5/character/powers/{charId}` | `CanViewCharacter` | Get discipline powers |
| PUT | `/vtmv5/character/powers/{charId}` | `CanViewCharacter` | Update discipline powers; fires `OnPowersUpdate` |
| GET | `/vtmv5/character/beliefs/{charId}` | `CanViewCharacter` | Get beliefs |
| PUT | `/vtmv5/character/beliefs/{charId}` | `CanViewCharacter` | Update beliefs; fires `OnBeliefsupdate` |
| GET | `/vtmv5/character/backgrounds/{charId}` | `CanViewCharacter` | Get backgrounds |
| PUT | `/vtmv5/character/backgrounds/{charId}` | `CanViewCharacter` | Update backgrounds; fires `OnBackgroundMeritFlawUpdate` |
| GET | `/vtmv5/character/merits/{charId}` | `CanViewCharacter` | Get merits |
| PUT | `/vtmv5/character/merits/{charId}` | `CanViewCharacter` | Update merits; fires `OnBackgroundMeritFlawUpdate` |
| GET | `/vtmv5/character/flaws/{charId}` | `CanViewCharacter` | Get flaws |
| PUT | `/vtmv5/character/flaws/{charId}` | `CanViewCharacter` | Update flaws; fires `OnBackgroundMeritFlawUpdate` |
| GET | `/vtmv5/character/profile/{charId}` | `CanViewCharacter` | Get profile/biography |
| PUT | `/vtmv5/character/profile/{charId}` | `CanViewCharacter` | Update profile; fires `OnProfileUpdate` |

**MasterdataController** (`src/backend/Controllers/VTMV5/MasterdataController.cs`) — `[Route("vtmv5/[controller]/[action]")]`, all `[Authorize]`

Serves static game rules data from in-memory dictionaries populated at startup from embedded JSON resources. No database dependency.

| HTTP | Route | Purpose |
|------|-------|---------|
| GET | `/vtmv5/masterdata/bloodpotency` | Single Blood Potency entry by `?id=` |
| GET | `/vtmv5/masterdata/bloodpotency/list` | All Blood Potency tiers |
| GET | `/vtmv5/masterdata/clan` | Single Clan by `?id=` |
| GET | `/vtmv5/masterdata/clan/list` | All Clans |
| GET | `/vtmv5/masterdata/discipline` | Single Discipline Power by `?id=` |
| GET | `/vtmv5/masterdata/discipline/list` | All Disciplines (grouped from `DisciplinePowers.json` at runtime) |
| GET | `/vtmv5/masterdata/predatortype` | Single Predator Type by `?id=` |
| GET | `/vtmv5/masterdata/predatortype/list` | All Predator Types |
| GET | `/vtmv5/masterdata/resonance` | Single Resonance by `?id=` |
| GET | `/vtmv5/masterdata/resonance/list` | All Resonances |

**Status**: Implemented.

### Authentication & Authorization

- **Cookie scheme**: `"Melpominee.app.Auth.V2"`, 24-hour sliding expiration, HttpOnly. Unauthorized requests return 401; forbidden requests return 403 (redirect events suppressed).
- **Claims**: `ClaimTypes.NameIdentifier` = email address, `ClaimTypes.Role` = `"user"` or `"admin"`.
- **Custom policy — `CanViewCharacter`** (`src/backend/Authorization/CanViewCharacter.cs`): succeeds if the authenticated user's email matches the character's `Owner` field, or if the user has the `"admin"` role. Loads character from cache or database to perform the ownership check.
- **Password hashing**: PBKDF2-HMACSHA512, 10,000 iterations, 128-bit salt (random per hash), 512-bit output. Output byte layout: `[0x02][512-bit hash][128-bit salt]`, Base64-encoded. The version byte (`0x02`) enables future algorithm migration.
- **Session**: `Melpominee.app.Session` cookie, 24-hour idle timeout, HttpOnly, essential. Used only for session clearing on login/logout; identity state lives in the auth cookie.

**Status**: Implemented.

### SignalR Hub

**File**: `src/backend/Hubs/VTMV5/CharacterHub.cs`
**Hub endpoint**: `/vtmv5/watch` (accessible via browser at `/api/vtmv5/watch` through the gateway)
**Authorization**: `[Authorize]` — unauthenticated connections are rejected before the hub is reached.

**Server methods (client → server):**

| Method | Parameters | Behavior |
|--------|-----------|---------|
| `WatchCharacter` | `charId: int` | Adds the connection to SignalR group `character_{charId}`, registers in `ConnectionService.GroupMap`, then broadcasts an updated watcher list to the group |

**Client events (server → client)** via `ICharacterClient` (`src/backend/Hubs/Clients/VTMV5/ICharacterClient.cs`):

| Event | Payload |
|-------|---------|
| `WatcherUpdate` | `charId: int, watchers: List<string>` |
| `OnHeaderUpdate` | `charId, updateId, timestamp, VampireCharacterUpdate` |
| `OnAttributeUpdate` | `charId, updateId, timestamp, VampireAttributeUpdate` |
| `OnSkillUpdate` | `charId, updateId, timestamp, VampireSkillUpdate` |
| `OnSecondaryUpdate` | `charId, updateId, timestamp, VampireStatsUpdate` |
| `OnDisciplineUpdate` | `charId, updateId, timestamp, VampireDisciplineUpdate` |
| `OnPowersUpdate` | `charId, updateId, timestamp, VampirePowersUpdate` |
| `OnBeliefsupdate` | `charId, updateId, timestamp, VampireBeliefsUpdate` *(note: lowercase 'u' — see improvement #18)* |
| `OnBackgroundMeritFlawUpdate` | `charId, updateId, timestamp, VampireBackgroundMeritFlawUpdate` |
| `OnProfileUpdate` | `charId, updateId, timestamp, VampireProfileUpdate` |

**Redis backplane**: channel prefix `"Melpominee"`, configured in `src/backend/Program.cs`. The backplane ensures that a client connected to pod A receives broadcasts fired from pod B, enabling horizontal scaling.

**Status**: Implemented.

### ConnectionService

**File**: `src/backend/Services/Hubs/ConnectionService.cs`

Three `ConcurrentDictionary` structures maintained in-process:

| Field | Type | Purpose |
|-------|------|---------|
| `ConnectionMap` | `ConcurrentDictionary<string, string>` | Maps SignalR connection ID → user email |
| `UserMap` | `ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>` | Maps user email → set of active connection IDs |
| `GroupMap` | `ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>` | Maps group ID (e.g., `character_42`) → set of connection IDs in that group |

On disconnect, `OnDisconnect` iterates `GroupMap` to find every group the disconnecting connection belonged to, removes it, then broadcasts an updated watcher list to each affected group. Because this state is in-process, a multi-pod deployment requires all clients for a given character to route to the same pod, or the watcher list becomes inconsistent across pods.

**Status**: Implemented.

### CharacterService

**File**: `src/backend/Services/Characters/CharacterService.cs`

Uses reflection to invoke `T.Load(args...)` static methods on character model types, enabling polymorphic loading without duplicating cache logic across every type:

```csharp
// Resolves and invokes e.g. VampireV5Attributes.Load(charId, characterService)
var loadMethod = typeof(T).GetMethod("Load", typeList.ToArray());
character = await (Task<T?>)loadMethod.Invoke(null, methodArgs)!;
```

Cache check precedes the `Load` invocation. On a cache miss, the loaded object is written back to Redis with a 1-minute sliding TTL. Cache key format: `melpominee:character:{charId}:{TypeName}`.

`SaveCharacterProperty<T>()` is a stub — it always returns `true` and is never called. (See improvement #11.)

**Status**: Implemented.

### Character Update Flow

**Directory**: `src/backend/Models/Characters/VTMV5/CharacterUpdate/`

Nine update classes, one per character section:

| Class | Section |
|-------|---------|
| `VampireCharacterUpdate` | Header |
| `VampireAttributeUpdate` | Attributes |
| `VampireSkillUpdate` | Skills |
| `VampireStatsUpdate` | Secondary stats |
| `VampireDisciplineUpdate` | Disciplines |
| `VampirePowersUpdate` | Discipline powers |
| `VampireBeliefsUpdate` | Beliefs |
| `VampireBackgroundMeritFlawUpdate` | Backgrounds, merits, flaws |
| `VampireProfileUpdate` | Profile/biography |

Each `Apply(character, cache)` method:

1. Opens a PostgreSQL connection and begins a transaction
2. Uses reflection to enumerate non-null properties and builds parameterized Dapper upsert statements
3. Executes the UPDATE within the transaction
4. Refreshes the domain object from the database within the same transaction (ensures the returned object reflects the committed state)
5. Commits the transaction
6. Awaits `Task.WhenAll(cache.RemoveAsync(...))` for the character and section cache keys

The controller then broadcasts the update to the SignalR group and returns the updated data.

**Status**: Implemented.

### UserManager

**File**: `src/backend/Services/Auth/UserManager.cs`

All user lifecycle operations use explicit database transactions. Redis is used as a 1-minute sliding cache for user profiles (`melpominee:user:{email}`). Operations:

| Method | Description |
|--------|-------------|
| `GetUser` | Cache-first lookup; falls back to PostgreSQL |
| `SaveUser` | Updates mutable user fields; refreshes cache |
| `Login` | Loads user bypassing cache; verifies PBKDF2 hash |
| `BeginResetPassword` | Generates random token, stores hashed form in `melpominee_users_rescue`, sends email |
| `FinishResetPassword` | Verifies hashed token, updates password within transaction, nulls rescue key |
| `Register` | Creates user with activation key (plaintext — see improvement #8), sends activation email |
| `RegistrationFinish` | Validates activation key (plain string compare), activates account |
| `OAuthRegister` | Creates or upserts user for Discord OAuth flow; skips password and activation |
| `HashPassword` | PBKDF2-HMACSHA512 with random salt |
| `VerifyPassword` | Extracts salt from stored hash, re-derives, compares |

**Status**: Implemented.

---

## Data Model

### PostgreSQL Schema

Schema created via `CREATE TABLE IF NOT EXISTS` at application startup (`DataContext.Initalize()`). Schema changes require manual SQL — no migration tooling is used. All character section tables cascade-delete from their parent character row via foreign key constraints.

| Table | Primary Key | Notes |
|-------|-------------|-------|
| `melpominee_users` | `email` (TEXT) | User profiles; PBKDF2 password hash; activation key stored plaintext |
| `melpominee_users_rescue` | `id` (BIGSERIAL) | Password reset tokens; rescue key stored as PBKDF2 hash; FK → users |
| `melpominee_characters` | `id` (BIGSERIAL) | Character header (name, clan, hunger, XP, etc.); FK → users |
| `melpominee_character_attributes` | `id` (BIGSERIAL) | One row per attribute per character; UNIQUE(CharId, Attribute); FK → characters |
| `melpominee_character_skills` | `id` (BIGSERIAL) | One row per skill per character; UNIQUE(CharId, Skill); FK → characters |
| `melpominee_character_secondary` | `id` (BIGSERIAL) | Health/Willpower/Humanity damage pools; UNIQUE(CharId, StatName); FK → characters |
| `melpominee_character_disciplines` | `id` (BIGSERIAL) | Per-discipline dot rating; UNIQUE(CharId, Discipline); FK → characters |
| `melpominee_character_discipline_powers` | `id` (BIGSERIAL) | Learned power IDs; UNIQUE(CharId, PowerId); FK → characters |
| `melpominee_character_beliefs` | `id` (BIGSERIAL) | Tenets, Convictions, Touchstones (JSON text); UNIQUE(CharId); FK → characters |
| `melpominee_character_meritflawbackgrounds` | `id` (BIGSERIAL) | Multi-row: ItemType discriminator (merit/flaw/background), SortOrder, Name, Score; FK → characters |
| `melpominee_character_profile` | `id` (BIGSERIAL) | Biographical text fields; UNIQUE(CharId); FK → characters |
| `melpominee_users_favorites` | `id` (BIGSERIAL) | User-to-character favorites mapping; FK → users and characters |

### Masterdata

Game rules data (Blood Potency, Clans, Discipline Powers, Predator Types, Resonance) is stored as embedded JSON resources in `src/backend/Data/`:

| File | Loaded Into |
|------|-------------|
| `BloodPotency.json` | `BloodPotency.BloodPotencyDict` |
| `Clans.json` | `VampireClan.ClanDict` |
| `DisciplinePowers.json` | `VampirePower.PowerDict` (also grouped by school to serve discipline lists) |
| `PredatorTypes.json` | `VampirePredatorType.TypeDict` |
| `Resonance.json` | `Resonance.ResonanceDict` |

There is no separate `Disciplines.json`. The discipline list endpoint (`/vtmv5/masterdata/discipline/list`) constructs discipline groupings at runtime by iterating `PowerDict` and grouping powers by their `School` field.

### Redis Cache Key Patterns

| Key | TTL | Purpose |
|-----|-----|---------|
| `melpominee:user:{email}` | 1 min sliding | User profile |
| `melpominee:character:{charId}:{TypeName}` | 1 min sliding | Character section data (e.g., `VampireV5Character`, `VampireV5Attributes`) |
| `DataProtectionKeys` | Persistent | ASP.NET Core data protection key ring (AES-256-CBC + HMACSHA256) |

**Status**: Implemented.

---

## Frontend Architecture

### Redux Store

**Store file**: `src/frontend/src/redux/store.ts`
**Typed hooks**: `src/frontend/src/redux/hooks.ts` (`useAppDispatch`, `useAppSelector`)

```
RootState
├── user: UserState         — email, role, ready
├── masterdata: MasterdataState — bloodPotencies, clans, disciplines, disciplinePowers, predatorTypes, resonances, loaded
└── tooltip: TooltipState   — tooltipType, tooltipId
```

| Reducer file | Slice name | Note |
|---|---|---|
| `src/frontend/src/redux/reducers/userReducer.ts` | `user` | `createSlice` name `'user'` |
| `src/frontend/src/redux/reducers/masterdataReducer.ts` | `user` | `createSlice` name is `'user'` (bug — see improvement #20) |
| `src/frontend/src/redux/reducers/tooltipReducer.ts` | `tooltip` | |

Thunks: `src/frontend/src/redux/thunk/initial.ts` (`initialThunk`), `src/frontend/src/redux/thunk/masterdata.ts` (`masterdataThunk`).

**Status**: Implemented.

### Component Hierarchy & Routes

Routes defined in `src/frontend/src/components/App.tsx`:

```
BrowserRouter
└── Layout
    ├── /                    (RequireAuth) → CharacterList
    ├── /character/:id       (RequireAuth) → CharacterSheet
    ├── /login               → Login
    ├── /forgot-password     → ForgotPassword
    └── /register            → Register
```

`RequireAuth` (`src/frontend/src/components/shared/Auth/RequireAuth.tsx`) guards protected routes by reading `selectUserEmail` from Redux. Unauthenticated users redirect to `/login` with the `from` location preserved.

CharacterSheet section components (all under `src/frontend/src/components/routes/CharacterSheet/`):

`HeaderSection`, `AttributeSection`, `SkillsSection`, `SecondarySection`, `DisciplineSection`, `BeliefsSection`, `MeritFlawSection`, `TheBloodSection`, `ProfileSection`, `HealthTracker`, `HumanityTracker`, `StatDots`, `HeaderBrand`, `CharacterSheetSection`

**Status**: Implemented.

### App Bootstrap Sequence

```
1. Mount → dispatch initialThunk()      [GET /api/auth/]
           → setUserdata(email, role)
           → userReady = true
2. userReady = true → render app (BrowserRouter + routes)
   userReady = false → render LoadingSpinner
3. Watch [userEmail, masterdataLoaded]:
   if userEmail !== '' && !masterdataLoaded → dispatch masterdataThunk()
       masterdataThunk() → Promise.all([
           GET /api/vtmv5/masterdata/bloodpotency/list,
           GET /api/vtmv5/masterdata/clan/list,
           GET /api/vtmv5/masterdata/discipline/list,
           GET /api/vtmv5/masterdata/predatortype/list,
           GET /api/vtmv5/masterdata/resonance/list,
       ]) → setMasterdataLoaded(true)
```

If `initialThunk` fails, `setUserdata` is never dispatched, `userReady` stays `false`, and the app renders an indefinite loading spinner. (See improvement #13.)

**Status**: Implemented.

### SignalR Client

**File**: `src/frontend/src/components/routes/CharacterSheet/CharacterSheet.tsx`

- `HubConnectionBuilder` → `/api/vtmv5/watch`, `withAutomaticReconnect()`
- Built and started in `useEffect([id])` — stops and rebuilds whenever the character ID route parameter changes
- 10 event handlers registered per connection (matching the `ICharacterClient` interface events)
- Updates routed through `handleUpdate()` in `src/frontend/src/util/character.ts`

**Deduplication**: `UpdateQueue: Map<charId, string[]>` holds the last 50 update IDs sent by this client. When a SignalR event arrives, `handleUpdate` checks the queue and skips the update if the `updateId` matches a locally-generated one (preventing echo application).

**Debounce**: `DebounceMap: Map<key, timeoutId>` delays outgoing PUT requests:

| Section | Debounce delay |
|---------|---------------|
| Header, Skills, Beliefs, Profile, MeritFlawBackgrounds | 100 ms |
| Secondary stats | 300 ms |
| Attributes, Disciplines, Powers | 0 ms (immediate) |

**Optimistic updates**: Local state is updated immediately via `setCurrCharacter` before the PUT response arrives. The server response is not used to update state; SignalR echoes (from other clients) are the source of truth for remote changes.

**Status**: Implemented.

### SCSS Design Tokens

**File**: `src/frontend/src/global.scss`

```scss
$text-primary:    rgba(255, 255, 255, 0.87)
$text-secondary:  rgba(255, 255, 255, 0.6)
$text-disabled:   rgba(255, 255, 255, 0.38)
$background-active: #aa2e25   // red accent
```

Responsive `font-size` scale applied to `:root`:

| Viewport | Base font-size |
|----------|---------------|
| 414px | 11px |
| 768px | 18px |
| 1024px | 24px |

Custom fonts: CaslonAntique, GoudyOldStyle (serif/gothic aesthetic matching VTM visual identity).

**Status**: Implemented.

---

## Real-Time Synchronization Flow

Full round-trip for a character attribute update:

```
1. User edits a field → optimistic local state update (setCurrCharacter)
2. Debounce timer fires (0 ms for attributes)
3. Frontend: PUT /api/vtmv5/character/attributes/{id}
   Body: { updateId: "uuid-v4", updateData: { strength: 3 } }

4. Backend CharacterController:
   a. Validates authorization (CanViewCharacter policy)
   b. Loads character from cache or DB (CharacterService.GetCharacterProperty)
   c. Calls VampireAttributeUpdate.Apply(character, cache):
      i.   Opens DB connection + begins transaction
      ii.  Enumerates non-null properties via reflection
      iii. Executes Dapper UPSERT (INSERT ... ON CONFLICT DO UPDATE)
      iv.  Refreshes character.Attributes from DB within same transaction
      v.   Commits transaction
      vi.  Awaits Task.WhenAll(cache.RemoveAsync(charKey), cache.RemoveAsync(propKey))
   d. Discards broadcast Task (fire-and-forget — see improvement #1):
      _ = hub.Clients.Group("character_{id}").OnAttributeUpdate(...)
   e. Returns 200 with updated attributes

5. All clients in "character_{id}" group receive OnAttributeUpdate
6. Each client: handleUpdate() checks updateId against UpdateQueue
   - Own updateId found → skip (already applied optimistically)
   - Foreign updateId → merge update into local character state via deepMerge
```

The broadcast happens after `Apply()` returns but the `Task` is discarded, so the HTTP response and the broadcast are not sequenced relative to each other. See improvement #1 for the risk this creates.

**Status**: Implemented.

---

## Infrastructure & Deployment

### Docker Compose

**File**: `docker-compose.yaml` — local and self-hosted deployment.

| Service | Image | Purpose |
|---------|-------|---------|
| `lb` | Traefik | Reverse proxy / TLS termination |
| `frontend` | `ghcr.io/imsilvz/melpominee-frontend` | nginx serving React SPA |
| `backend` | `ghcr.io/imsilvz/melpominee-backend` | ASP.NET Core API |
| `database` | `postgres` | PostgreSQL |
| `redis` | `redis` | Cache + SignalR pub-sub backplane |
| `pgadmin` | `dpage/pgadmin4` | Database admin UI |

Secrets loaded from `./secrets/*.json` files on the host.

**Status**: Implemented.

### Kubernetes

**Production deployment on Azure AKS.**

**`manifests/deploy.yaml`:**

| Resource | Details |
|----------|---------|
| Frontend Deployment | 1 replica; `ghcr.io/imsilvz/melpominee-frontend:master`; nginx :80; 256Mi/250m resources |
| Frontend Service | ClusterIP; port 80 → targetPort 80 |
| Backend Deployment | 1 replica; `ghcr.io/imsilvz/melpominee-backend:master`; :8080; 256Mi/250m resources |
| Backend Service | ClusterIP; port 80 → targetPort 8080 |
| ServiceAccount | `melpominee`; annotated with Azure Workload Identity client ID |

Backend pod specifics:
- `serviceAccountName: melpominee` binds Azure Workload Identity
- Init container (`busybox:1.36`) validates the CSI secrets-store volume mounts before the main container starts
- CSI driver mounts secrets from Azure Key Vault via `melpominee-secrets` SecretProviderClass
- Projected volume aggregates four Kubernetes secrets (discord, email, postgres, redis), all marked `optional: true`, mounted at `/etc/melpominee/secrets`

**`manifests/httproute.yaml`:**

| Resource | Details |
|----------|---------|
| Gateway | `melpominee-gateway`; class `eg` (Envoy Gateway); HTTPS :443; cert-manager TLS (`cluster-issuer`); hostname `melpominee.app` |
| HTTPRoute rule 1 | `PathPrefix: /api/` → URL rewrite (`ReplacePrefixMatch: /`) → `melpominee-backend:80` |
| HTTPRoute rule 2 | `PathPrefix: /` → `melpominee-frontend:80` |

The URL rewrite on rule 1 strips the `/api` prefix so backend controllers receive requests rooted at `/`.

**Status**: Implemented.

### CI/CD

**File**: `.github/workflows/deploy.yaml`
**Trigger**: Push to `master` branch or any `v*` tag
**Runner**: Self-hosted (`arc-melpominee`)

Steps:
1. Login to GHCR
2. Build and push backend Docker image (multi-stage: .NET SDK build → ASP.NET runtime)
3. Build and push frontend Docker image (multi-stage: Node build → nginx)
4. Azure login via Workload Identity (OIDC)
5. Deploy manifests via `Azure/k8s-deploy@v5` to `melpominee` namespace
6. Rollout restart both deployments (forces re-pull of `:master` tag)

**Status**: Implemented.

---

## Potential Improvements

> **Constraint**: The frontend layout must not change. All improvements below are architectural and behavioral changes only. Test coverage additions are a separate concern and are not addressed here.

### High Severity

#### 1. Fire-and-Forget SignalR Broadcasts

**File**: `src/backend/Controllers/VTMV5/CharacterController.cs` (all 11 PUT handlers)

All SignalR broadcasts use the `_ =` discard pattern:

```csharp
_ = _characterHub.Clients.Group($"character_{charId}").OnHeaderUpdate(...);
```

The returned `Task` is discarded, so broadcast failures are silent and unobservable. Additionally, the HTTP response returns before the broadcast completes — a client can receive a 200 OK and re-fetch from a cache entry that has already been invalidated but before other clients have received the update.

**Recommendation**: Await each broadcast call before returning the HTTP response.

#### 2. Cache Invalidation Timing Race

**Files**: `src/backend/Models/Characters/VTMV5/CharacterUpdate/*.cs` (all 9 `Apply` methods)

The transaction commits, and then `Task.WhenAll(cache.RemoveAsync(...))` is awaited within `Apply()` before returning to the controller. The race window is between `trans.Commit()` and the completion of the `await Task.WhenAll(...)` call. Any client that fetches character data during this narrow window hits PostgreSQL (cache miss) and gets the new data — this is actually safe. The real risk is that a client hitting the cache during this window gets stale pre-commit data.

The more significant risk is item #1: because the broadcast is fire-and-forget, other clients may not receive the SignalR event at all if the broadcast task faults.

**Recommendation**: Ensure `await Task.WhenAll(...)` is called and awaited before the controller dispatches the HTTP response (currently satisfied within `Apply()`). Address item #1 to ensure broadcast reliability.

#### 3. Concurrent Dictionary Iteration Without Snapshot

**File**: `src/backend/Services/Hubs/ConnectionService.cs`

`OnDisconnect` iterates `GroupMap` with `foreach(var groupKvp in GroupMap)` while other concurrent `OnGroupAdd` calls can modify the dictionary. `ConcurrentDictionary` iteration is safe against exceptions, but modifications during enumeration can result in the disconnecting connection being missed in newly-added groups.

**Recommendation**: Snapshot the dictionary keys before iterating:

```csharp
var groups = GroupMap.Keys.ToList();
foreach (var groupId in groups) { ... }
```

#### 4. Missing Rate Limiting on Auth Endpoints

**File**: `src/backend/Controllers/AuthController.cs`

`/auth/login`, `/auth/register`, and `/auth/reset-password` have no rate limiting. This enables brute-force password attacks, account enumeration via timing differences, and email spam via repeated reset requests.

**Recommendation**: Apply ASP.NET Core's built-in rate limiting middleware (`AddRateLimiter`) with a fixed-window or token-bucket policy scoped to these endpoints.

#### 5. AdminView Authorization Bypass

**File**: `src/backend/Controllers/VTMV5/CharacterController.cs` — `GetList()` method

Any authenticated user (not only admins) can pass `?adminView=true` and receive all character records:

```csharp
if (adminView == true)
{
    charList = await VampireV5Character.GetCharacters(); // all characters
}
```

Owner emails are masked for non-admin callers, but character names, clans, concepts, and all other header fields are fully exposed.

**Recommendation**: Gate the admin path on the user's actual role:

```csharp
bool showAdminView = adminView == true && user.Role == "admin";
if (showAdminView) { ... }
```

---

### Medium Severity

#### 6. Mail Service Fire-and-Forget

**File**: `src/backend/Services/Auth/UserManager.cs`

`Services.MailService.Instance.SendMail(...)` is called without `await` and the return value is not checked in both `BeginResetPassword` and `Register`. Failed sends are silent — users attempting registration or password reset receive a success response but no email.

**Recommendation**: Make `SendMail` async, await it, and surface send failures as error responses to the calling controller.

#### 7. HttpClient Created Per Request Without Disposal

**File**: `src/backend/Controllers/AuthController.cs` — `DiscordOAuth` method

`new HttpClient()` is instantiated on each OAuth callback and never disposed. This exhausts socket descriptors under concurrent load.

**Recommendation**: Inject `IHttpClientFactory` and use a named client, or register a singleton `HttpClient` for Discord API calls.

#### 8. Registration Activation Key Stored Plaintext

**File**: `src/backend/Services/Auth/UserManager.cs` — `Register()` and `RegistrationFinish()`

Password reset tokens are stored as PBKDF2 hashes. Registration activation keys are stored in plaintext in `melpominee_users.ActivationKey` and compared with plain string equality:

```csharp
if (key != user.ActivationKey) { ... }
```

A database breach exposes all pending activation keys, allowing an attacker to activate arbitrary accounts.

**Recommendation**: Hash the activation key before storage using the same PBKDF2 pattern used for password reset tokens, and use `VerifyPassword` for comparison in `RegistrationFinish`.

#### 9. No Input Validation on Character Scores

**File**: `src/backend/Controllers/VTMV5/CharacterController.cs` (all PUT handlers)

Character attribute scores, skill ratings, discipline levels, and secondary stat values are persisted without server-side range validation. A client can store arbitrary integers.

**Recommendation**: Add range checks before calling `Apply()` — for example, attribute scores must be 1–5, Humanity 0–10, Hunger 0–5.

#### 10. Transaction Isolation Level Not Specified

**Files**: `src/backend/Services/Auth/UserManager.cs`, `src/backend/Models/Characters/VTMV5/CharacterUpdate/*.cs`

Transactions use PostgreSQL's default `READ COMMITTED` isolation. For character update transactions that execute a Dapper UPDATE and then re-read the updated row within the same transaction, a concurrent update by another user between the write and the re-read can produce a domain object that does not reflect the latest committed state.

**Recommendation**: Specify `IsolationLevel.RepeatableRead` on character update transactions to prevent non-repeatable reads between the UPDATE and the subsequent SELECT.

#### 11. SaveCharacterProperty Stub Left in Production Code

**File**: `src/backend/Services/Characters/CharacterService.cs`

```csharp
public bool SaveCharacterProperty<T>(int charId, T property)
{
    return true;
}
```

This method always returns `true`, is never called by any controller or service, and creates misleading surface area.

**Recommendation**: Remove the stub or replace with `throw new NotImplementedException()` until a real implementation is required.

#### 12. SignalR Updates Discarded During Initial Character Fetch

**File**: `src/frontend/src/components/routes/CharacterSheet/CharacterSheet.tsx`

Two `useEffect` hooks race on mount: one builds and starts the SignalR connection, the other fetches the initial character state via HTTP. SignalR events that arrive before the HTTP fetch completes are applied to `setCurrCharacter` with `char.id === charId` guard — if `currCharacter` is still `null` at that point, the guard fails and the update is silently dropped.

**Recommendation**: Buffer incoming SignalR events until the initial HTTP fetch completes, then replay them against the fetched state.

#### 13. Frontend Thunks Have No Error Handling

**Files**: `src/frontend/src/components/App.tsx`, `src/frontend/src/redux/thunk/initial.ts`, `src/frontend/src/redux/thunk/masterdata.ts`

If `initialThunk` fails, `setUserdata` is never dispatched, `userReady` stays `false`, and the app displays an indefinite loading spinner with no error message. If any single endpoint within `masterdataThunk`'s `Promise.all` rejects, the entire masterdata load fails silently.

**Recommendation**: Add try/catch in both thunks. On `initialThunk` failure, dispatch `setUserdata` with empty state and `ready: true` so the UI can show an error or redirect to login. On `masterdataThunk` failure, set an error flag so affected UI sections can show a meaningful message.

#### 14. Tooltip Redux State Not Cleared on Unmount

**File**: `src/frontend/src/components/shared/Tooltip/Tooltip.tsx`

When a component that opened a tooltip unmounts while the tooltip is visible, the tooltip state in Redux is not cleared. The tooltip can persist visually or reappear unexpectedly after navigation.

**Recommendation**: Add a cleanup function in the tooltip's `useEffect` that dispatches a clear action on unmount.

---

### Low Severity

#### 15. Cache Stampede on User Cache Miss

**File**: `src/backend/Services/Auth/UserManager.cs` — `GetUser()`

Multiple concurrent requests for the same user arriving after cache expiry all miss the cache simultaneously and issue parallel PostgreSQL queries.

**Recommendation**: Use a `SemaphoreSlim` keyed by email to serialize cache population for a given user, or accept the behavior as acceptable given current traffic volume.

#### 16. No Health Check Endpoint

No `/health` or `/readyz` endpoint exists. Kubernetes liveness/readiness probes cannot distinguish a degraded pod from a healthy one, and load balancers have no signal for pod removal.

**Recommendation**: Add `app.MapHealthChecks("/health")` in `Program.cs` with checks for PostgreSQL connectivity and Redis availability.

#### 17. No Audit Logging

No structured log events record who modified which character, when authorization was denied, or when authentication failed. Debugging collaborative editing conflicts and security incidents requires full log reconstruction.

**Recommendation**: Add `ILogger<T>` structured logging to character PUT handlers (recording user, character ID, and section) and to auth failure paths.

#### 18. Event Name Typo in SignalR Hub Interface

**File**: `src/backend/Hubs/Clients/VTMV5/ICharacterClient.cs`

`ICharacterClient` declares `OnBeliefsupdate` (lowercase `u`) instead of `OnBeliefsUpdate`, inconsistent with every other event in the interface. The frontend registers the handler as `onBeliefsUpdate` (capital `U`). Because SignalR method name matching is case-insensitive on the hub side but the frontend `conn.on(...)` registration is case-sensitive, the frontend handler registered as `'onBeliefsUpdate'` will not fire from a server-side call to `OnBeliefsupdate`.

**Recommendation**: Rename to `OnBeliefsUpdate` in `ICharacterClient.cs`. The frontend handler registration string must be updated in `CharacterSheet.tsx` to match. This requires a coordinated backend and frontend change.

#### 19. Hardcoded Discord OAuth URLs

**File**: `src/backend/Controllers/AuthController.cs`

Discord API base URL, authorize URL, token URL, and revoke URL are hardcoded string constants:

```csharp
private const string DISCORD_AUTHORIZE_URL = "https://discord.com/oauth2/authorize";
private const string DISCORD_API_URL = "https://discord.com/api/v10";
```

**Recommendation**: Move to `appsettings.json` under a `Discord:AuthorizeUrl`, `Discord:TokenUrl`, and `Discord:ApiUrl` configuration section, bound via the options pattern.

#### 20. masterdataSlice Incorrectly Named `userSlice`

**File**: `src/frontend/src/redux/reducers/masterdataReducer.ts`

The `createSlice` call names the slice `'user'`:

```typescript
export const userSlice = createSlice({
  name: 'user',   // should be 'masterdata'
  ...
});
```

Both `userReducer.ts` and `masterdataReducer.ts` export a variable named `userSlice` with `name: 'user'`. In Redux DevTools, the masterdata slice appears as a second `user` slice, making it impossible to distinguish the two by name.

**Recommendation**: Rename the slice variable and `name` field in `masterdataReducer.ts` to `masterdataSlice` / `'masterdata'`.

---

## Out of Scope

- **Frontend layout changes** — The visual layout of the character sheet and all other pages is stable and intentionally excluded from improvements.
- **Test coverage** — Adding unit, integration, or end-to-end tests is a separate workstream and not addressed here.
