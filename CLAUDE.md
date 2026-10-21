# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Gyro Webapp Kestrel Starter — a starting point for custom web apps. A drop-in replacement for the Node/Express `gyroscope-webapp-starter`: same ports, `.env`, endpoints, response bodies, redirects, console output, npm commands, and the identical React client — only the server is ASP.NET Core (Kestrel). Two HTTPS servers (web + API) run from a single .NET process, proxying GraphQL requests from the React frontend to the API server.

## Architecture

```
Browser  →  Web server (:3003)  →  proxy /graphql  →  API server (:5005)
                │                                           │
          serves React SPA                       Hot Chocolate GraphQL + SQLite
          (Vite build output)
```

- **`src/Server/Program.cs`** — entry point; loads `.env`, starts both Kestrel apps. Each app (`Hosting.cs`) listens on its HTTPS port (HTTP/1.1, like Node) plus an HTTP port that only 308-redirects to HTTPS (`HttpsRedirect.cs`)
- **`src/Server/Api/`** — `ApiApp.cs` mounts Hot Chocolate at `/graphql`, REST routes (`Routes/`), a JSON 404, and a JSON error handler
- **`src/Server/Web/WebApp.cs`** — serves the built React client, proxies `/graphql` to the API server (YARP), SPA fallback to `index.html`
- **`src/web/client/`** — React SPA (Vite + PWA). Separate `package.json`; unchanged from the Node starter
- **`src/Server/Api/Database/`** — SQLite via `Microsoft.Data.Sqlite` + SQLite3MultipleCiphers (`Db.cs`, WAL mode, optional `DB_KEY` encryption — file-compatible with `better-sqlite3-multiple-ciphers`) and table definitions (`Schema.cs`, currently empty — add your own tables)
- **`tests/Server.Tests/`** — xUnit tests for the server
- **`data/`** — SQLite database files (gitignored)
- **`certs/`** — self-signed TLS certs (gitignored, generate with `npm run certs:gen`)

All paths (`.env`, `certs/`, `data/`, `src/web/client/dist`) resolve from the repo root; `Server.csproj` sets `RunWorkingDirectory` so `dotnet run` behaves the same from anywhere.

The only example wired up end-to-end is a `health` GraphQL query (`src/Server/Api/GraphQL/Queries/HealthQueries.cs`), fetched by the React client on mount (`src/web/client/src/App.jsx`) — it exists to prove the pipeline works, not as real functionality. Replace it with your own domain.

## Commands

Requires the .NET 10 SDK on `PATH` and Node.js 26.7.0 (for the client and npm scripts).

### Setup
```bash
npm install                  # root tooling (concurrently, prettier, husky, dotenv for db:reset)
npm run client:install       # client dependencies (separate package.json)
npm run certs:gen            # generate self-signed TLS certs
cp .env.example .env         # configure ports and TLS paths
```

### Development
```bash
npm run dev:all              # API/web server (dotnet watch) + Vite dev server concurrently
npm run dev                  # server only (dotnet watch, restarts on change)
npm run client:dev           # Vite dev server only
npm run build:server         # dotnet publish -> dist/server.dll
```

### Testing
```bash
npm test                     # server tests (dotnet test / xUnit)
npm run client:test          # client tests (Jest + @testing-library/react + jsdom)
```

### Production
```bash
npm run client:build         # build React client
npm run build:server         # publish server to dist/
npm start                    # dotnet dist/server.dll (serves built client + API)
```

## Code Style

- C#: `dotnet format` defaults (run `npm run format:fix-all`); file-scoped namespaces under `Gyro.*`, static classes with extension methods for wiring (`AddServices`/`Configure`/`Map*`)
- Client JS/CSS: unchanged from the Node starter — see `.github/instructions/`
- No auto-incrementing IDs — use `Db.CreateId()` (cuid2) for all primary keys
- Any mutable table gets `created_at`/`updated_at` (bump `updated_at` on every `UPDATE`); append-only/audit tables only get `created_at`
- All DB access goes through `Db.Use(db => ...)`, which serializes access to the single shared connection (Kestrel is multi-threaded; `SqliteConnection` is not thread-safe)

## GraphQL Pattern

Each feature area gets a class under `src/Server/Api/GraphQL/Queries/` (or `Mutations/`) marked `[ExtendObjectType(OperationTypeNames.Query)]` (or `Mutation`), registered with `.AddTypeExtension<T>()` in `GraphQLSchema.cs`. Method `GetFoo()` exposes field `foo`; non-nullable C# return types become non-null GraphQL types. Add `.AddMutationType(d => d.Name(OperationTypeNames.Mutation))` with your first mutation.

The client defines all queries/mutations as template literal strings in `src/web/client/src/api.js` and sends them through a thin `graphqlRequest()` wrapper that hits `/graphql` (proxied to the API server).

## Testing Convention

Whenever you manually confirm something works (a curl check, a build, a manual run), also add a test for it. Server tests live in `tests/Server.Tests/`, mirroring the `src/Server/` folder layout; they use an in-memory SQLite DB (`DB_PATH=':memory:'`, set in `TestSetup.cs`) and `Microsoft.AspNetCore.TestHost` for HTTP-level checks. Client tests use `@testing-library/react` with `fetch` mocked.
