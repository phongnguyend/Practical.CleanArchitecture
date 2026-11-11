# Classified Ads React UI

Standalone React/TypeScript frontend built with Vite and SWC. It calls the API directly, uses browser-based OIDC authentication, and connects to the notification hub. It is distinct from the [cookie-authenticated BFF React app](../bff/reactjs/README.md).

## Run locally

These frontends were last verified with Node.js 24 and npm. Start the API and Identity Server before testing authenticated features; their default local addresses are `https://localhost:44312` and `https://localhost:44367`. Use the backend's development certificate setup and an account with the required permissions.

From the repository root:

```powershell
cd src/UIs/reactjs
npm ci
npm run dev
```

Open `http://localhost:3000`. Next.js and BFF React also use port 3000 by default, so run them separately.

## Configuration

[src/environments/environment.dev.tsx](src/environments/environment.dev.tsx) contains development API, hub, OIDC authority/client and UI URL settings. [environment.tsx](src/environments/environment.tsx) contains production settings; [index.tsx](src/environments/index.tsx) selects them by build mode.

The default OIDC client is `ClassifiedAds.React`. Changing the UI origin requires matching redirect/logout registrations and API CORS configuration. The API base URL includes `/api/`.

## Build and checks

| Command | Behavior |
| --- | --- |
| `npm run build` | Vite production bundle in `dist/`; no TypeScript check |
| `npm run strictbuild` | TypeScript check followed by the Vite build |
| `npm run preview` | Local preview of the built bundle |

Static hosting must serve `index.html` for application routes. Configure production endpoints before building. The preview server is for local checks.

The existing `lint` script uses ESLint, but the repository still has a legacy `.eslintrc.cjs` alongside ESLint 10. Migrate that configuration before relying on the lint command. There is no npm unit-test script; leftover scaffold test files are not the shared E2E runner.

## End-to-end tests

The shared [TypeScript Playwright suite](../../e2e/ui/node/playwright/README.md) covers navigation, themes, products, files, users, roles, settings and audit logs. Follow its credential and service setup instructions, then run from the repository root:

```powershell
cd src/e2e/ui/node/playwright
$env:E2E_APPS = 'reactjs'
$env:E2E_START_SERVERS = '1'
npm run test:headed -- --project=reactjs
```

`E2E_START_SERVERS=1` manages the frontend development server. CRUD tests require `E2E_ALLOW_WRITES=1` against a disposable backend; PDF export also requires the conversion service and `E2E_PDF=1`. See the suite README for reports and the recorded verification results.

## SonarQube

Install SonarScanner CLI and run it from this project's directory. Set `SONAR_TOKEN` in the current terminal or CI environment without committing it. The existing SonarCloud project settings are:

```powershell
$env:SONAR_TOKEN = '<your-token>'
sonar-scanner.bat -D"sonar.organization=phongnguyend" -D"sonar.projectKey=UIs_React" -D"sonar.projectName=UIs React" -D"sonar.projectVersion=1.0.0.0" -D"sonar.sources=." -D"sonar.host.url=https://sonarcloud.io"
```

For a self-hosted SonarQube server, change the host/project settings and omit `sonar.organization`. See the [SonarScanner CLI documentation](https://docs.sonarsource.com/sonarqube-server/analyzing-source-code/scanners/sonarscanner).

## Deploy to Azure Static Web App

From this project's directory, configure production API/OIDC URLs and build:

```powershell
npm run build
```

Create `staticwebapp.config.json` in `./dist` after building:

```json
{
  "navigationFallback": {
    "rewrite": "/index.html"
  }
}
```

Install the Azure Static Web Apps CLI, set the deployment token for the target Static Web App, and deploy the build output:

```powershell
npm install -g @azure/static-web-apps-cli
$env:SWA_CLI_DEPLOYMENT_TOKEN = '<deployment-token>'
swa deploy ./dist --app-name react --env production
```

Replace the app name with your Azure resource name. Register the deployed UI's callback/logout URLs with Identity Server and allow its origin in API CORS. Re-create or copy the fallback configuration after each build that clears the output directory. See [Microsoft's SWA CLI deployment guide](https://learn.microsoft.com/en-us/azure/static-web-apps/static-web-apps-cli-deploy).
