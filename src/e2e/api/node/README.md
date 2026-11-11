# TypeScript API end-to-end tests

This package ports the scenarios in `../dotnet/ClassifiedAds.ApiIntegrationTests` to TypeScript using Playwright's API request runner. It calls the real Identity Server and API; no browser installation, UI server, or mocked responses are required.

## Run

Use Node.js 22.13+ and a running development/test API, Identity Server, database and storage service. The account needs the same permissions as the .NET suite. The registered client must permit the password grant used by that suite, or supply an existing access token.

From the repository root:

```powershell
cd src/e2e/api/node
npm ci
Copy-Item .env.example .env
# Edit .env with the API URL, OIDC client credentials and test account.
# Set API_ALLOW_WRITES=1 for the isolated CRUD scenarios.
# Set API_IGNORE_HTTPS_ERRORS=1 only for local self-signed certificates.
npm run typecheck
npm test
```

To explicitly reuse the existing .NET settings instead of copying credentials:

```powershell
$env:API_DOTNET_SETTINGS = '../dotnet/ClassifiedAds.ApiIntegrationTests/appsettings.json'
$env:API_ALLOW_WRITES = '1'
$env:API_IGNORE_HTTPS_ERRORS = '1'
npm test
```

Environment values take precedence over the optional settings file. `API_BASE_URL` includes the `/api/` suffix. Alternatively, `API_ACCESS_TOKEN` bypasses discovery and the password-token request. Tokens and credentials are never written into source files by the suite.

```powershell
npm test -- tests/products.spec.ts
npm test -- tests/configuration.spec.ts
npm test -- tests/files.spec.ts
npm test -- tests/users.spec.ts
npm run test:list
npm run report
```

## Coverage and .NET parity

| .NET scenario | TypeScript coverage |
| --- | --- |
| Products `AllInOne` | Create/list/read/update/delete; quoted/comma-containing names; exact CREATED/UPDATED audit counts; CSV export and re-import of exported content; PDF export |
| Configuration `AllInOne_NonSensiveValue` | Create/read/update ordinary values, toggle to sensitive, verify masking, export/import Excel, delete |
| Configuration `AllInOne_SensiveValue` | Create/read/update sensitive values, toggle to ordinary, export/import Excel, delete |
| Files `AllInOne` | Multipart encrypted upload, exact audit counts, list/read/update, download, delete; also repeated for an unencrypted file |
| Users `AllInOne` | Create/list/read/update/delete; weak-password rejection and valid password setting on the generated user; confirmation-email and reset-email requests |

The five original workflows become eight independently reported tests: PDF and email operations are separated, and plain-file coverage is added. PDF testing is enabled by default; set `API_TEST_PDF=0` to explicitly skip it when no conversion server is configured. HTTP errors fail the test instead of saving an error response as a successful download. PDF/Excel signatures and binary file bytes are checked.

Email requests run only with both `API_ALLOW_EMAILS=1` and `API_TEST_EMAIL` set to a test mailbox or mail sink. They target a newly generated test account and verify API acceptance, not eventual email delivery. No existing account's password is changed. The valid fixture password is randomly generated per test.

All mutating scenarios require `API_ALLOW_WRITES=1`. Every test uses a unique `api-e2e-` prefix. Teardown lists only the resources used by that test, verifies ownership again before deleting each ID, and runs even after an assertion fails. This also finds owned records when a create operation commits before returning an error. Cleanup failures are reported; audit history remains. Non-idempotent creates are not blindly retried.

The current import endpoints parse CSV/Excel and return models; persistence is a backend TODO. Tests preserve the .NET export/import behavior and verify the test record appears in the parsed result. Deletion accepts the API variants' 404, 204, or successful empty/null response, but never accepts a server error as proof of deletion. Audit assertions poll for asynchronous audit propagation while retaining the original expected event counts.

## Results

`playwright-report/` contains HTML, and `test-results/junit.xml` contains JUnit results. Downloaded fixtures/exports are saved beneath each test's output directory. These directories and `.env` are ignored. Exports can contain development data, so keep reports and downloads private. Traces are disabled to avoid recording authentication and password request bodies.

Verification on 2026-11-11 using the existing .NET development settings: **6 passed, 1 failed, 1 skipped**. The failure was PDF export returning HTTP 500 from the backend; the skipped test was email sending, which was not enabled. CRUD cleanup, CSV/Excel round-trips, audit counts, binary plain/encrypted downloads and password validation passed.
