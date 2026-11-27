# .NET UI end-to-end tests

The Playwright and Selenium projects compile the same feature scenarios from `Shared/Features`, with a native browser driver for each engine. They mirror `../node/playwright/tests`: neither .NET project launches Node tests. Each test opens a fresh browser and signs in through Identity Server. Tests run sequentially and use condition-based waits.

## Run

Start the API, Identity Server, and the UI you want to test. These .NET projects do not start application servers. Use each UI README for its start command. The test account needs the same permissions as the Node suite.

From the repository root:

```powershell
Copy-Item src/e2e/ui/dotnet/.env.example src/e2e/ui/dotnet/.env
# Fill E2E_USERNAME and E2E_PASSWORD in the ignored .env file.
$env:E2E_ENV_FILE = (Resolve-Path src/e2e/ui/dotnet/.env).Path
$env:E2E_APPS = 'reactjs'

dotnet build src/e2e/ui/dotnet/Playwright
powershell -File src/e2e/ui/dotnet/Playwright/bin/Debug/net10.0/playwright.ps1 install chromium
dotnet test src/e2e/ui/dotnet/Playwright --logger trx

dotnet test src/e2e/ui/dotnet/Selenium --logger trx
```

.NET 10 SDK/runtime and Chrome are required. Selenium Manager resolves ChromeDriver automatically; no hard-coded driver directory is needed. Password-manager prompts are disabled only in ChromeDriver's temporary test profile so they cannot cover the application. Playwright browser installation is an explicit one-time step, not repeated inside tests.

You can reuse the Node suite's ignored `.env` by setting `E2E_ENV_FILE` to its absolute path. Existing process environment variables take precedence. Browser windows are visible and maximized by default; responsive tests explicitly change the viewport. Set `E2E_HEADLESS=1` for headless CI. No credentials are stored in test source or reports.

`E2E_APPS` accepts `angular,reactjs,vuejs,nextjs,bff-reactjs,blazor-server,blazor-wasm`. Each is an independent xUnit theory row. React and Next.js share port 3000; run them separately. BFF React and Blazor WebAssembly share port 44348; run them separately. URL overrides are listed in `.env.example`. For BFF React, start both Vite and ReverseProxy.Yarp.

```powershell
$env:E2E_APPS = 'angular,reactjs,vuejs'
dotnet test src/e2e/ui/dotnet/Playwright --filter 'FullyQualifiedName~NavigationTests'
dotnet test src/e2e/ui/dotnet/Selenium --filter 'FullyQualifiedName~FilesTests'

# After stopping React and starting Next.js:
$env:E2E_APPS = 'nextjs'
dotnet test src/e2e/ui/dotnet/Playwright
```

## Coverage parity

| Node suite | Shared .NET test class | Coverage |
| --- | --- | --- |
| auth.setup | TestBase / Scenario.Login | Real Identity Server sign-in before every scenario |
| navigation | NavigationTests | All six sections and reload, Home link, Light/Dark persistence, System theme, notification toast |
| products | ProductsTests | Required fields, CRUD, detail/filter/images, delete cancellation, history, CSV import/reset/export, optional PDF |
| settings | SettingsTests | Modal validation/focus, CRUD, sensitive masking, Excel import/reset/export |
| roles | RolesTests | Modal validation/focus, create/detail/rename/delete |
| users | UsersTests | Validation, CRUD, password/reset/confirmation dialog cancellation without sending emails |
| files | FilesTests | Picker selection at two widths and themes, missing-file highlight/recovery, required fields, plain/encrypted upload/edit/download/history/delete |
| audit | AuditTests | Seeded JSON formatting/copy/tree/collapse/focus and pagination request verification |
| playground-tabs | PlaygroundTests | Nested tab state and persistence after reload |
| email-sms-playgrounds / mobile-push | PlaygroundTests | Field limits, required/recipient validation, Fake-provider queueing and refresh |
| workspace-layout | WorkspaceLayoutTests | Skip-link focus, overflow, themes, modal visibility, consistent active tabs, right-aligned Add/Upload actions |

## Write tests and diagnostics

- `E2E_ALLOW_WRITES=1` enables CRUD, seeded audit, and queue scenarios. Leave it disabled against non-disposable backends. Disabled cases are reported as skipped.
- Queue tests assert `Provider: Fake` before submitting. They fail safely if a real provider is configured, rather than delivering email, SMS, or device notifications. Queue records remain in the queue, matching the Node tests; there is no queue-delete API.
- CRUD records use a unique `e2e-` prefix. Cleanup runs even after failures, rechecks ownership, and deletes only this test's records. Cleanup failures fail the test.
- SPA fixture requests use the signed-in OIDC access token; BFF/Next.js use their real cookie session (including YARP anti-forgery). Blazor write-fixture seeding/cleanup additionally requires `E2E_API_TOKEN`, since its browser does not expose the server's bearer token. Read-only Blazor checks do not need it.
- `E2E_PDF=1` enables the converter-dependent PDF test. Notification-hub checks only apply to apps implementing that client.
- CSV/Excel imports exercise parsing; the backend intentionally does not persist imported rows.
- Failed tests capture `failure.png`; responsive checks also save screenshots. Downloads remain beside those artifacts. Use `E2E_ARTIFACTS_DIR` to change the default `TestResults/ui` location. Screenshots can contain test-environment data; keep artifacts private.
- `--logger trx` creates standard .NET test reports. Use `dotnet test --list-tests` to inspect the application/scenario matrix without opening browsers.
