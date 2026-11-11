# Classified Ads Vue UI

Vue/TypeScript frontend built with Vite, with products, files, users, roles, settings, audit logs, themes and live notifications.

## Run locally

These frontends were last verified with Node.js 24 and npm. Start the API and Identity Server before testing authenticated features; their default local addresses are `https://localhost:44312` and `https://localhost:44367`. Use the backend's development certificate setup and an account with the required permissions.

From the repository root:

```powershell
cd src/UIs/vuejs
npm ci
npm run dev
```

Open `http://localhost:8080`. If another service uses that port, configure a free origin and its OIDC registration instead of opening the unrelated service.

## Configuration

[environments/environment.dev.ts](environments/environment.dev.ts) defines development settings; [environment.ts](environments/environment.ts) defines production settings. Both support these Vite environment overrides:

| Variable | Default | Purpose |
| --- | --- | --- |
| `VITE_APP_URL` | `http://localhost:8080/` | UI origin used for OIDC callbacks and logout |
| `VITE_OIDC_CLIENT_ID` | `ClassifiedAds.Vue` | Registered Identity Server client |

The API and hub addresses are also defined in these environment files. Match UI origin changes with Identity Server redirect/logout registrations and API CORS configuration. Vite variables are supplied when the server starts or the bundle is built.

## Build and checks

| Command | Behavior |
| --- | --- |
| `npm run build` | Vite production bundle in `dist/` |
| `npm run type-check` | Separate `vue-tsc --build` check |
| `npm run preview` | Local preview of the built bundle |
| `npm run lint` | ESLint with `--fix`; may change files |
| `npm run format` | Prettier rewrite of `src/` |

`build` does not run the separate type check. Static hosting must fall back to `index.html` for application routes. There is no npm unit-test script.

## End-to-end tests

The shared [TypeScript Playwright suite](../../e2e/ui/node/playwright/README.md) covers navigation, themes, products, files, users, roles, settings and audit logs. Follow its credential and service setup instructions, then run from the repository root:

```powershell
cd src/e2e/ui/node/playwright
$env:E2E_APPS = 'vuejs'
$env:E2E_START_SERVERS = '1'
npm run test:headed -- --project=vuejs
```

`E2E_START_SERVERS=1` manages the frontend development server. CRUD tests require `E2E_ALLOW_WRITES=1` against a disposable backend; PDF export also requires the conversion service and `E2E_PDF=1`. See the suite README for reports and the recorded verification results.

The suite README also documents running Vue on port 3000 with the existing React development OIDC registration when port 8080 is occupied. Browser-only asset forwarding does not reliably handle login callback redirects.

## SonarQube

Install SonarScanner CLI and run it from this project's directory. Set `SONAR_TOKEN` in the current terminal or CI environment without committing it. The existing SonarCloud project settings are:

```powershell
$env:SONAR_TOKEN = '<your-token>'
sonar-scanner.bat -D"sonar.organization=phongnguyend" -D"sonar.projectKey=UIs_Vue" -D"sonar.projectName=UIs Vue" -D"sonar.projectVersion=1.0.0.0" -D"sonar.sources=." -D"sonar.host.url=https://sonarcloud.io"
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
swa deploy ./dist --app-name vue --env production
```

Replace the app name with your Azure resource name. Register the deployed UI's callback/logout URLs with Identity Server and allow its origin in API CORS. Re-create or copy the fallback configuration after each build that clears the output directory. See [Microsoft's SWA CLI deployment guide](https://learn.microsoft.com/en-us/azure/static-web-apps/static-web-apps-cli-deploy).
