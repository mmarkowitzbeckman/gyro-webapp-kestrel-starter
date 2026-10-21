# Gyro Webapp Kestrel Starter

A starting point for custom web apps: an ASP.NET Core (Kestrel) + Hot Chocolate GraphQL API server, a React (Vite) client, both served over HTTPS from a single .NET process, backed by SQLite.

It is a server-side port of `gyroscope-webapp-starter` (Node/Express/GraphQL Yoga). From the outside nothing changes: the same ports, `.env` keys, endpoints (`/graphql`, `/health`), JSON response bodies, 308 HTTP→HTTPS redirects, startup log lines, npm commands, and the identical React client. `DB_KEY`-encrypted SQLite files are interchangeable between the two (both use SQLite3MultipleCiphers).

## Inventory

**Root config / tooling**
- `package.json` — npm scripts wrapping both the .NET server and the client (`dev`, `dev:all`, `dev:watch`, `dev:build-watch`, `build:server`, `start`, `client:*`, `certs:gen`, `db:reset`, `format*`, `test`)
- `gyro-webapp-kestrel-starter.slnx` — .NET solution (server + tests)
- `.env.example`, `.nvmrc`, `.gitignore`, `.prettierrc`, `.prettierignore` — environment template, pinned Node version, ignore rules, Prettier config
- `.vscode/`, `.husky/pre-commit`, `.github/instructions/` — editor, pre-commit, and AI-assistant conventions
- `scripts/dev-build-watch.sh` — clears the dev ports, then runs the build-watch dev flow
- `scripts/reset-db.js` — deletes the SQLite database files
- `CLAUDE.md` — guidance for Claude Code (or any AI assistant) working in this repo

**Server** (`src/Server/`)
- `Program.cs` — entry point, loads `.env`, starts the web + API apps
- `Hosting.cs` — builds one Kestrel app: HTTPS port + HTTP redirect port, startup log lines
- `HttpsRedirect.cs`, `Tls.cs`, `Config.cs` — HTTP→HTTPS redirect, PEM cert loading, env helpers
- `Api/ApiApp.cs` — mounts GraphQL, REST routes (`Routes/IndexRoutes.cs`), JSON 404 + error handler
- `Api/GraphQL/` — `GraphQLSchema.cs` registers one example type extension (`Queries/HealthQueries.cs`)
- `Api/Database/Db.cs` — SQLite connection (WAL, foreign keys, optional `DB_KEY` encryption), `CreateId()`
- `Api/Database/Schema.cs` — table definitions; an empty, documented template
- `Web/WebApp.cs` — serves the built React client, proxies `/graphql` to the API server, SPA fallback

**Client** (`src/web/client/`) — React SPA (Vite + PWA), its own `package.json`, Jest + Testing Library setup

**Tests**
- `tests/Server.Tests/` — xUnit: database, GraphQL resolver, API HTTP responses, HTTPS redirect
- `src/web/client/src/__tests__/App.test.jsx`

## Getting started

### Prerequisites
- .NET SDK **10** (`dotnet --version`)
- Node.js **26.7.0** (pinned in `.nvmrc`) and `npm`
- `openssl` (for generating local TLS certs)

### 1. Install dependencies
```bash
npm install                  # root tooling
npm run client:install       # client dependencies
```
NuGet packages restore automatically on first build.

### 2. Generate local TLS certs
```bash
npm run certs:gen
```

### 3. Configure environment variables
```bash
cp .env.example .env
```

### 4. Run it
```bash
npm run dev:all
```
This starts the server under `dotnet watch` (GraphQL at `https://localhost:5005/graphql`) and the Vite dev server, which proxies `/graphql` to it. Open the URL Vite prints.

### 5. Build for production
```bash
npm run client:build   # builds the React client to src/web/client/dist/
npm run build:server   # publishes the server to dist/
npm start              # dotnet dist/server.dll
```
`https://localhost:3003` then serves the built app, proxying `/graphql` to the API server internally.

### Running the tests
```bash
npm test                # server tests (xUnit)
npm run client:test     # client tests (Jest + React Testing Library)
```
