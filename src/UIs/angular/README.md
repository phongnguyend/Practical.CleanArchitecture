# Classified Ads Angular UI

Angular frontend for products, files, users, roles, configuration entries, audit logs and live notifications. Dependency versions are defined in [package.json](package.json) and locked in `package-lock.json`.

## Run locally

These frontends were last verified with Node.js 24 and npm. Start the API and Identity Server before testing authenticated features; their default local addresses are `https://localhost:44312` and `https://localhost:44367`. Use the backend's development certificate setup and an account with the required permissions.

From the repository root:

```powershell
cd src/UIs/angular
npm ci
npm start
```

Open `http://localhost:4200`. The npm script uses the project's installed Angular CLI; no global CLI installation is needed.

## Configuration

Development settings are in [src/environments/environment.ts](src/environments/environment.ts). They define the API and notification endpoints, Identity Server authority, `ClassifiedAds.Angular` client, and current UI URL. The production configuration substitutes [environment.prod.ts](src/environments/environment.prod.ts).

When changing the UI origin, update its OIDC redirect/logout registrations and the API's allowed CORS origins as well as these settings.

## Build and checks

```powershell
npm run build
npm run build -- --configuration production
```

The first command uses the default configuration in [angular.json](angular.json), which currently has optimization disabled. Use the explicit production configuration for deployment. Browser assets are written to `dist/angular/browser`; static hosting must fall back to `index.html` for application routes.

`angular.json` declares a Jest test target, not Karma. There is no Protractor/e2e target or npm `test` script. The `lint` script currently invokes `ng lint`, but no lint target is configured; it needs configuration before use.

## End-to-end tests

The shared [TypeScript Playwright suite](../../e2e/ui/node/playwright/README.md) covers navigation, themes, products, files, users, roles, settings and audit logs. Follow its credential and service setup instructions, then run from the repository root:

```powershell
cd src/e2e/ui/node/playwright
$env:E2E_APPS = 'angular'
$env:E2E_START_SERVERS = '1'
npm run test:headed -- --project=angular
```

`E2E_START_SERVERS=1` manages the frontend development server. CRUD tests require `E2E_ALLOW_WRITES=1` against a disposable backend; PDF export also requires the conversion service and `E2E_PDF=1`. See the suite README for reports and the recorded verification results.

## SonarQube

Install SonarScanner CLI and run it from this project's directory. Set `SONAR_TOKEN` in the current terminal or CI environment without committing it. The existing SonarCloud project settings are:

```powershell
$env:SONAR_TOKEN = '<your-token>'
sonar-scanner.bat -D"sonar.organization=phongnguyend" -D"sonar.projectKey=UIs_Angular" -D"sonar.projectName=UIs Angular" -D"sonar.projectVersion=1.0.0.0" -D"sonar.sources=." -D"sonar.host.url=https://sonarcloud.io"
```

For a self-hosted SonarQube server, change the host/project settings and omit `sonar.organization`. See the [SonarScanner CLI documentation](https://docs.sonarsource.com/sonarqube-server/analyzing-source-code/scanners/sonarscanner).

## Deploy to Azure Static Web App

From this project's directory, configure production API/OIDC URLs and build:

```powershell
npm run build -- --configuration production
```

Create `staticwebapp.config.json` in `./dist/angular/browser` after building:

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
swa deploy ./dist/angular/browser --app-name angular --env production
```

Replace the app name with your Azure resource name. Register the deployed UI's callback/logout URLs with Identity Server and allow its origin in API CORS. Re-create or copy the fallback configuration after each build that clears the output directory. See [Microsoft's SWA CLI deployment guide](https://learn.microsoft.com/en-us/azure/static-web-apps/static-web-apps-cli-deploy).
