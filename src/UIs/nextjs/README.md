# Classified Ads Next.js UI

Next.js App Router frontend with server-side OIDC handling and a same-origin API proxy. It stores encrypted tokens in HTTP-only cookies, rather than using the standalone React app's browser OIDC storage. Routes are under [src/app](src/app).

## Run locally

These frontends were last verified with Node.js 24 and npm. Start the API and Identity Server before testing authenticated features; their default local addresses are `https://localhost:44312` and `https://localhost:44367`. Use the backend's development certificate setup and an account with the required permissions.

From the repository root, on Windows:

```powershell
cd src/UIs/nextjs
npm ci
npm run dev
```

Open `http://localhost:3000`. Stop standalone React/BFF React first because their development server uses the same port.

The current `dev` script contains Windows `SET` syntax and disables Node TLS verification for local self-signed services. On another shell, use `npx next dev --turbopack` with the Node certificate trust configured for your local services. Do not reuse the development TLS bypass for a deployment.

## Configuration

Configure environment variables in an untracked `.env.local` or in the server environment:

| Variable | Default / requirement |
| --- | --- |
| `NEXT_PUBLIC_OAUTH_AUTHORITY` | `https://localhost:44367` |
| `NEXT_PUBLIC_OAUTH_CLIENT_ID` | `ClassifiedAds.React` |
| `NEXT_PUBLIC_CURRENT_URL` | `http://localhost:3000/`; retain the trailing slash |
| `NEXT_PUBLIC_RESOURCE_SERVER_ENDPOINT` | `https://localhost:44312`; the proxy appends `/api/` |
| `ENCRYPTION_SECRET` | Set a private random secret of at least 32 characters; do not use the development fallback for deployment |

The UI origin must match the client's registered callback and logout URLs. The server exchanges the OIDC code, handles `/login` and `/logout`, and forwards `/api/*` requests to the backend. Unlike the other UIs, this app currently has no notification-hub client.

## Build and checks

```powershell
npm run build
npm start
```

The build creates `.next/` output and includes TypeScript checking. Deploy with a Next.js server and HTTPS: the cookie authentication and API routes require server execution, and production authentication cookies are secure. This app is not a static SPA export. The layout uses `next/font/google`, so building requires access to its configured font sources.

The existing `lint` script still invokes `next lint`, which the installed Next.js CLI no longer provides. Its ESLint configuration needs review before documenting a working lint command. There is no npm unit-test script.

## End-to-end tests

The shared [TypeScript Playwright suite](../../e2e/ui/node/playwright/README.md) covers navigation, themes, products, files, users, roles, settings and audit logs. Follow its credential and service setup instructions, then run from the repository root:

```powershell
cd src/e2e/ui/node/playwright
$env:E2E_APPS = 'nextjs'
$env:E2E_START_SERVERS = '1'
npm run test:headed -- --project=nextjs
```

`E2E_START_SERVERS=1` manages the frontend development server. CRUD tests require `E2E_ALLOW_WRITES=1` against a disposable backend; PDF export also requires the conversion service and `E2E_PDF=1`. See the suite README for reports and the recorded verification results.

The notification-hub scenario is explicitly skipped for Next.js. API fixture setup uses its real cookie-authenticated proxy, including byte-preserving file uploads/downloads.
