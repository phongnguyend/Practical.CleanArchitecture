# Classified Ads BFF React UI

React/TypeScript frontend served through the [YARP BFF](../ReverseProxy.Yarp/Program.cs). The browser uses authentication cookies and an anti-forgery header; YARP attaches the access token when forwarding API requests. Open the app through YARP, not Vite directly.

## Run locally

These frontends were last verified with Node.js 24 and npm. Start the API and Identity Server before testing authenticated features; their default local addresses are `https://localhost:44312` and `https://localhost:44367`. Use the backend's development certificate setup and an account with the required permissions.

The BFF targets .NET 10. From the repository root, start Vite in one terminal:

```powershell
cd src/UIs/bff/reactjs
npm ci
npm run dev
```

In another terminal, from the repository root:

```powershell
dotnet run --project src/UIs/bff/ReverseProxy.Yarp --launch-profile https
```

Open **`https://localhost:44348`**. The current out-of-process BFF forwards frontend requests to Vite on `http://localhost:3000`. Standalone React and Next.js share that Vite port by default; stop them first.

## Configuration and authentication

- [src/environments/environment.dev.tsx](src/environments/environment.dev.tsx) and [environment.tsx](src/environments/environment.tsx) use same-origin `/api/` and `/hubs/notification` endpoints.
- [YARP appsettings.json](../ReverseProxy.Yarp/appsettings.json) configures OIDC, API/hub proxy routes and `FrontendHostingMode`. Its OIDC client defaults to `ReverseProxy.Yarp`.
- [Program.cs](../ReverseProxy.Yarp/Program.cs) currently contains the out-of-process frontend destination. Changing Vite's port also requires changing that destination.
- `/login`, `/logout` and `/userinfor` belong to the BFF. `/userinfor` issues `PHONG-XSRF-TOKEN`; API requests send it as `X-XSRF-TOKEN`.

YARP needs server-side trust of the API and Identity Server certificates. Browser certificate settings do not configure that trust. Keep client secrets in the backend's configuration rather than frontend environment files.

## Build and checks

```powershell
npm run build
```

Vite writes assets to `dist/`. `npm run strictbuild` adds a TypeScript check. `npm run preview` previews assets only and does not provide BFF authentication/API routes.

For in-process hosting, deploy the built assets into YARP's `wwwroot` and set `FrontendHostingMode` to `InProcess`; YARP serves static files and the SPA fallback. Building this React project alone does not copy those files or publish YARP.

The lint script still relies on legacy `.eslintrc.cjs` configuration with ESLint 10 and needs migration. No npm unit-test script is configured.

## End-to-end tests

The shared [TypeScript Playwright suite](../../../e2e/ui/node/playwright/README.md) covers navigation, themes, products, files, users, roles, settings and audit logs. Follow its credential and service setup instructions, then run from the repository root:

```powershell
cd src/e2e/ui/node/playwright
$env:E2E_APPS = 'bff-reactjs'
$env:E2E_START_SERVERS = '1'
npm run test:headed -- --project=bff-reactjs
```

`E2E_START_SERVERS=1` manages the frontend development server. CRUD tests require `E2E_ALLOW_WRITES=1` against a disposable backend; PDF export also requires the conversion service and `E2E_PDF=1`. See the suite README for reports and the recorded verification results.

For this project, Playwright starts Vite only. Keep YARP running separately throughout the test run.

## SonarQube

Install SonarScanner CLI and run it from this project's directory. Set `SONAR_TOKEN` in the current terminal or CI environment without committing it. The existing SonarCloud project settings are:

```powershell
$env:SONAR_TOKEN = '<your-token>'
sonar-scanner.bat -D"sonar.organization=phongnguyend" -D"sonar.projectKey=UIs_React" -D"sonar.projectName=UIs React" -D"sonar.projectVersion=1.0.0.0" -D"sonar.sources=." -D"sonar.host.url=https://sonarcloud.io"
```

For a self-hosted SonarQube server, change the host/project settings and omit `sonar.organization`. See the [SonarScanner CLI documentation](https://docs.sonarsource.com/sonarqube-server/analyzing-source-code/scanners/sonarscanner).

The retained project key is shared with standalone React. Use a separate Sonar project key if both applications need independent analysis histories.
