# Shared web UI end-to-end tests

One TypeScript suite runs against the **Angular**, **React**, **Vue**, **Next.js**, and **BFF React** applications using Playwright projects named `angular`, `reactjs`, `vuejs`, `nextjs`, and `bff-reactjs`. Test bodies and page objects contain no framework-specific branches. Tests use the real UI, Identity Server, API, database and storage; API requests are used for isolated fixture setup and guarded cleanup, not response mocking.

## Run locally

Requirements: Node.js 22.13+ (or a supported newer Node version), the selected UI projects' npm dependencies, and a running backend and Identity Server configured for these clients. Use a disposable development account with product, file, user, role, configuration and audit permissions.

From the repository root, install each UI's dependencies if needed (`npm ci --prefix src/UIs/angular`, and likewise for `reactjs` and `vuejs`). Then:

```powershell
cd src/e2e/ui/node/playwright
npm ci
npx playwright install chromium
Copy-Item .env.example .env
# Edit .env: enter the test credentials and environment URLs.
# Set E2E_ALLOW_WRITES=1 for full coverage against a disposable backend.
# Set E2E_IGNORE_HTTPS_ERRORS=1 only for local self-signed certificates.
npm test
```

`E2E_START_SERVERS=1` starts the selected development UIs (Angular, React and Vue by default) and stops the servers it started after the run. Existing servers are reused locally. With `0`, start the UIs yourself or point the URLs at deployed applications. API and Identity Server are never started or stopped by this suite. Their CORS settings and registered OIDC redirect URIs must match the UI origins.

```powershell
npm test -- --project=angular
npm test -- --project=reactjs
npm test -- --project=vuejs
npm test -- tests/products.spec.ts
npm test -- --grep "@writes"
npm run test:headed
npm run test:ui
npm run typecheck
npm run report
```

Each app has a dependent setup project that signs in through the Identity Server login form and saves a separate state file. Missing credentials, failed login or failed API requests fail the run rather than being silently skipped. `--no-deps` can reuse saved authentication while diagnosing failures, until the token expires.

### Next.js

Next.js runs the **same spec files**, with a cookie-authenticated API fixture that calls its real `/api/` proxy. It does not read or decrypt the HTTP-only access-token cookie. Login still uses the shared Identity Server setup.

Next.js and React default to the same registered origin, `http://localhost:3000`. Stop React first, install Next.js dependencies, then select Next.js explicitly:

```powershell
# From src/e2e/ui/node/playwright, with .env configured as above:
npm ci --prefix ../../../../UIs/nextjs
$env:E2E_APPS = 'nextjs'
npm test -- --project=nextjs
```

`E2E_APPS` is a comma-separated list controlling both registered test projects and managed development servers. The default remains `angular,reactjs,vuejs`. Duplicate origins and unknown app names are rejected. Set `E2E_NEXTJS_URL` for another deployment; for a different local port, also configure Next.js `NEXT_PUBLIC_CURRENT_URL` and register that redirect URI with Identity Server. Next.js does not support the optional origin-forwarding workaround below because its authentication and API live on the UI server.

Next.js forwards request and response bytes through its API proxy, including multipart uploads and empty responses. The shared file tests compare binary download contents for both plain and encrypted uploads.

The notification-hub connection scenario is explicitly skipped for Next.js: that app has a toast component but no SignalR client. Other scenarios remain enabled, subject to the same write/PDF opt-ins. Next.js server-side certificate trust is separate from Playwright's `E2E_IGNORE_HTTPS_ERRORS`; use the app's existing development configuration or a trusted development certificate.

### BFF React (YARP)

The `bff-reactjs` project uses the same specs through YARP, including notification checks. The API fixture uses the authenticated cookies and obtains the session-bound `PHONG-XSRF-TOKEN` from `/userinfor`, sending it as `X-XSRF-TOKEN` for real API requests. No access token is extracted.

Start the API and Identity Server, then run the BFF from the repository root in a separate terminal:

```powershell
dotnet run --project src/UIs/bff/ReverseProxy.Yarp --launch-profile https
```

In another terminal:

```powershell
cd src/e2e/ui/node/playwright
npm ci --prefix ../../../../UIs/bff/reactjs
$env:E2E_APPS = 'bff-reactjs'
$env:E2E_START_SERVERS = '1'
npm run test:headed -- --project=bff-reactjs
```

Configure credentials in `.env`. The browser URL is `E2E_BFF_REACTJS_URL` (default `https://localhost:44348`), **not** Vite's direct URL. `E2E_START_SERVERS=1` manages the BFF React Vite server only; YARP must already be running. Its current out-of-process configuration forwards frontend requests to port 3000, so stop standalone React/Next.js before this run. `E2E_BFF_REACTJS_DEV_URL` defaults to `http://localhost:3000`; changing it also requires updating YARP's frontend forwarding destination. For an in-process or deployed BFF, use `E2E_START_SERVERS=0`.

YARP's registered OIDC client/redirects, upstream API address and server-side certificate trust must be configured. Browser `E2E_IGNORE_HTTPS_ERRORS=1` does not change YARP's trust of Identity Server or the API. Origin forwarding is disabled for this cookie-authenticated app. Write and PDF tests retain the common opt-ins.

### Local port conflicts

Use a free UI port with a registered OIDC redirect URI. Browser-only forwarding via `E2E_<APP>_SERVER_URL` can serve UI assets from another port, but it does **not** reliably handle Identity Server callback redirects. Do not use it to claim a complete authenticated run when the registered origin serves another application.

Vue supports `VITE_APP_URL` and `VITE_OIDC_CLIENT_ID` overrides. For local testing with the existing React development client (stop React/Next.js first):

```powershell
$env:E2E_APPS = 'vuejs'
$env:E2E_VUEJS_URL = 'http://localhost:3000'
$env:E2E_VUEJS_SERVER_URL = 'http://localhost:3000'
$env:VITE_APP_URL = 'http://localhost:3000/'
$env:VITE_OIDC_CLIENT_ID = 'ClassifiedAds.React'
npm test -- --project=vuejs
```

These overrides only affect the launched Vue process. The default Vue origin/client remain `http://localhost:8080/` and `ClassifiedAds.Vue`.


## Coverage

| File | Scenarios |
| --- | --- |
| `auth.setup.ts` | Real OIDC login and independent authenticated state for each app |
| `navigation.spec.ts` | Every resource route, direct reload, active navigation, Home, project link, persistent light/dark themes, system appearance, notification connection toast |
| `products.spec.ts` | Required fields, create/details/edit/delete, filtering and empty results, image toggle, cancelled deletion, audit history, CSV import validation, CSV/PDF downloads |
| `roles-settings.spec.ts` | Required fields, keyboard modal dismissal and focus restoration, role CRUD/details, configuration CRUD and sensitive-value masking, Excel import validation/export |
| `files.spec.ts` | Required fields, plain/encrypted upload, metadata editing, exact downloaded-byte comparison, history and deletion |
| `users.spec.ts` | Required username, user CRUD/details, password/reset-email/confirmation-email dialogs without sending emails or changing passwords |
| `audit.spec.ts` | Seeded pagination, JSON tree/formatted modes, collapse, clipboard contents, modal dismissal and restored focus |

`@writes` tests require `E2E_ALLOW_WRITES=1`. They create unique `pw-<random>` fixtures and delete only recorded IDs whose current name/key/username still matches that test's prefix. Cleanup runs on test failure too and reports failures. Never enable writes against production. Normal audit entries remain after fixture deletion. PDF export is separately opt-in with `E2E_PDF=1` because it requires the HTML-to-PDF service.

The suite does not send account emails, change the login account, test notification delivery, implement backend import persistence, or cover every permission combination. The default browser is Chromium; Firefox/WebKit projects can be added using the same tests after installing their browsers.

## Results and CI

`playwright-report/` contains the HTML report. `test-results/` contains JUnit XML and failure screenshots/videos/traces. All are ignored along with `.auth/` and `.env`. Authentication setup disables traces/video/screenshots to avoid capturing credentials. Application traces and stored browser state may contain tokens and development data: keep CI artifacts private and use short retention.

For CI, install the dependencies and browser (`npx playwright install --with-deps chromium` on Linux), supply credentials as secrets, set the application/API URLs, then run `npm test`. Use `E2E_START_SERVERS=0` for predeployed environments. Archive the HTML/JUnit results even on failure. No personal credentials or authentication state belong in Git.

## Dependency upgrade verification (2026-11-10)

All five frontend manifests and lockfiles were updated from npm's latest stable tags. TypeScript remains at `~6.0.3` because Angular 22 and TypeScript ESLint require `<6.1`; Next.js retains ESLint 9 because its React lint plugin does not yet support ESLint 10. Dependency-tree checks and production builds pass for all five apps.

Full suites were run with writes and PDF export enabled, followed by targeted reruns after fixes. Vue used the alternate registered origin described above. PDF export returned HTTP 500 from the shared API; these failures are retained, not skipped. The Next.js notification test remains explicitly skipped because that app has no hub client.

Final results below combine the full run and targeted reruns after fixes (including each project's login setup):

| Project | Passed | Failed | Skipped |
| --- | ---: | ---: | ---: |
| Angular | 30 | 1 | 0 |
| React | 30 | 1 | 0 |
| Vue | 30 | 1 | 0 |
| Next.js | 29 | 1 | 1 |
| BFF React | 30 | 1 | 0 |

All five failures are PDF export. The other exercised scenarios pass, including CRUD and plain/encrypted file round-trips. Local verification reports are retained under the repository's ignored `.tmp/upgrade-*` directories; the original React report is under `.tmp/upgrade-reactjs-first-report`.

Set `E2E_ARTIFACTS_DIR` to a separate directory per run when retaining multiple reports or running independent projects concurrently. It contains HTML, JUnit and failure artifacts; keep it outside tracked source or in an ignored directory. The default artifact locations are unchanged.
