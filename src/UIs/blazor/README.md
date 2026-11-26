# Blazor

The two Blazor hosts share pages and components through `ClassifiedAds.Blazor.Modules`:

- `ClassifiedAds.BlazorServerSide`: server-hosted UI at `https://localhost:44331`.
- `ClassifiedAds.BlazorWebAssembly`: browser-hosted UI at `https://localhost:44348`.
- `ClassifiedAds.Blazor.Modules`: shared navigation, feature pages, dialogs, and notification playgrounds.

The projects reference application, domain, and infrastructure libraries under
`src/Monolith`. They have a separate solution and CI workflows. The Monolith Aspire app host
launches them as external .NET processes for local development.
Project names, authentication client IDs, and callback URLs are unchanged.

## Build and run

Use the .NET 10 SDK selected by `global.json`. From this directory:

```powershell
dotnet build ClassifiedAds.Blazor.slnx
dotnet run --project ClassifiedAds.BlazorServerSide --launch-profile https
# In another terminal:
dotnet run --project ClassifiedAds.BlazorWebAssembly --launch-profile https
```

Start the API and Identity Server first. The development settings use
`https://localhost:44312` and `https://localhost:44367` respectively.
To launch the complete Monolith environment, run from the repository root:

```powershell
dotnet run --project src/Monolith/ClassifiedAds.AspireAppHost
```

## UI tests

The shared [Playwright suite](../../e2e/ui/node/playwright/README.md) supports both hosts.
Build the solution above, configure the suite's `.env`, then run from the repository root:

```powershell
cd src/e2e/ui/node/playwright
$env:E2E_APPS = 'blazor-server,blazor-wasm'
npx playwright test navigation.spec.ts playground-tabs.spec.ts email-sms-playgrounds.spec.ts mobile-push.spec.ts --headed
```

## Docker

Both hosts have dedicated images and Compose services. Start the API and Identity
Server separately (for example with the Monolith Docker Compose stack), then run:

```powershell
cd src/UIs/blazor
docker compose up --build -d
```

Server: `http://host.docker.internal:9008`; WebAssembly: `http://localhost:9009`.
Compose defaults to Identity Server at `http://host.docker.internal:9000` and API
at `http://host.docker.internal:9002`. Set `IDENTITY_AUTHORITY`, `API_ENDPOINT`,
`WASM_ORIGIN`, `BLAZOR_SERVER_PORT`, and `BLAZOR_WASM_PORT` in a local `.env`
to override them. URLs must also be reachable from the browser. WebAssembly's
public settings are generated when the container starts; never put secrets there.

Register the chosen origins in your API CORS policy and Identity Server clients:

- Server client `ClassifiedAds.BlazorServerSide`: `/signin-oidc` and
  `/signout-callback-oidc` on the Server origin.
- Browser client `ClassifiedAds.BlazorWebAssembly`: `/authentication/login-callback`
  and `/authentication/logout-callback` on `WASM_ORIGIN`.

The development HTTPS registrations remain unchanged. Existing Identity databases
may need their clients updated for these Docker origins. HTTP settings in Compose
are for local development; use HTTPS, secure cookies, and
`REQUIRE_HTTPS_METADATA=true` for deployment.

Dockerfiles use `src` as their build context to include the referenced Monolith libraries.

## CI

Dedicated GitHub Actions build/publish both hosts, build both Docker images, and
scan NuGet dependencies with Markdown reports. Docker builds run on pull requests;
images publish to GHCR only on `master` or `build`, including manual runs. Each image gets a commit
SHA tag; `master` also updates `latest`. Publish artifacts are separate for each host.

Workflows: `.net-build-ui-blazor.yml`, `docker-build-ui-blazor.yml`, and
`scan-dependencies-ui-blazor.yml` under `.github/workflows`.
