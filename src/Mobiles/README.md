# Mobile applications

Three independent native clients for the API used by `src/UIs`:

| Project | Toolkit | Platforms |
| --- | --- | --- |
| [Flutter](Flutter/README.md) | Flutter Material 3 | Android, iOS |
| [Kotlin](Kotlin/README.md) | Jetpack Compose Material 3 | Android |
| [ReactNative](ReactNative/README.md) | React Native / Expo | Android, iOS |

All three include browser OIDC login with PKCE, secure session storage/refresh, logout, persisted System/Light/Dark appearance, search, paging, loading/error states, confirmation dialogs, editors, expandable JSON inspection/copy, and foreground SignalR notifications.

## Features

| Area | Available in all three apps |
| --- | --- |
| Products | Name filtering, image visibility, ratings, readable details, create/edit/delete, highlighted audit history, confirmed CSV import, CSV/PDF exports |
| Files | List/details, multipart upload with encryption choice, metadata edit with read-only filename/encryption, delete, download/share, highlighted audit history |
| Users | List/details, all account fields, create/edit/delete, highlighted audit history, password with matching confirmation, confirmed password-reset and confirmation emails |
| Roles | List, view, create, edit, delete |
| Configuration | List/details, create/edit/delete, masked sensitive values, confirmed Excel import/export |
| Audit logs | Five-entry server paging, date/user/action, search within loaded page, JSON details/copy |
| Notifications | Live in-app messages, recent-message list (up to 100 during the session) |
| Home | Developer information, project link, live clock, connection status |

The existing CSV/Excel import endpoints parse and return rows without persisting them. Like the web clients, mobile clients ask for confirmation before processing, then reload the list; they report processing rather than claiming records were saved. Notifications use SignalR while foregrounded, not OS push delivery. Backend authorization remains authoritative.

The feature reference is the implementation in `src/UIs/angular`, `src/UIs/reactjs`, and `src/UIs/vuejs`. Native screens retain the same API operations and validation, with date/time pickers, platform file sharing, and mobile layouts. Details show labeled fields; history shows snapshots and changed fields, with JSON available for inspection. Editors distinguish unchanged cancellation from discarding edits.

## Backend setup

1. Start a Monolith, ModularMonolith, or Microservices backend and its identity server using that project's README.
2. Run identity-server seeding/startup after updating the repository. OpenIddict and Duende seeders register the clients below. Other providers need equivalent public clients with authorization-code + PKCE, refresh tokens, and no secret.
3. Configure API base URL (including `/api/`), OIDC issuer, and hub URL. Defaults match web development: `https://localhost:44312/api/`, `https://localhost:44367`, `https://localhost:44312/hubs/notification`.
4. Use addresses reachable from both device and system browser, with trusted HTTPS certificates. `localhost` refers to the device. Android emulator's host alias is `10.0.2.2`, but certificates and the advertised OIDC issuer must match the chosen hostname too. Configure backend listeners/issuer accordingly; merely replacing the app hostname is insufficient. A trusted HTTPS development hostname/tunnel is an alternative. No certificate-validation bypass is included.

| Client ID | Login redirect | Logout redirect |
| --- | --- | --- |
| `ClassifiedAds.Mobile.Flutter` | `classifiedads.flutter:/oauthredirect` | `classifiedads.flutter:/logout` |
| `ClassifiedAds.Mobile.Kotlin` | `classifiedads.kotlin:/oauthredirect` | `classifiedads.kotlin:/logout` |
| `ClassifiedAds.Mobile.ReactNative` | `classifiedads.rn:/oauthredirect` | `classifiedads.rn:/logout` |

Native integrations provide authentication, secure storage, file picking/sharing, and SignalR. Dialogs, JSON viewers, and notifications use framework UI primitives.

## Source organization

Each app separates application composition, feature code, infrastructure, and reusable UI:

| Responsibility | Flutter (`lib/`) | Kotlin (`com.classifiedads.mobile`) | React Native (`src/`) |
| --- | --- | --- | --- |
| Entry and composition | `main.dart`, `app/` | `MainActivity.kt` | `app/` route adapters |
| Feature code | `features/<feature>/` | `features.<feature>` | `features/<feature>/` |
| Platform services | `core/config/`, `core/services/` | `core.auth`, `core.network` | `core/auth/`, `network/`, `files/`, `notifications/`, `storage/`, `config/` |
| Reusable models and UI | `shared/models/`, `shared/widgets/` | `shared.model`, `shared.ui` | `shared/models/`, `shared/ui/` |

Each resource owns its typed model, validation, API/repository operations, form, details/history presentation, and screen. Navigation selects concrete screens explicitly. Shared UI does not import resource implementations or workspace screens; infrastructure does not import screens.

There are no field-definition arrays, feature registries, or schema interpreters. Forms declare their native inputs directly. Shared record-page components manage reusable loading, paging, editor/dialog state, and confirmations through typed callbacks; they do not choose fields, validate domain data, or construct resource endpoints. JSON maps remain at API and audit-log boundaries, where the server supplies JSON.

For example, product changes belong in `Flutter/lib/features/products/`, Kotlin's `features.products` package, and `ReactNative/src/features/products/`. Modify the product model/validation, form, repository, and tests as needed rather than adding schema entries. File upload/encryption, user password/email actions, and import/export controls stay in their respective feature folders.

## Integration checklist

Repeatable Android UI tests for all three apps are available in [tests/README.md](tests/README.md). Run `./src/Mobiles/tests/Run-UiTests.ps1 -App All` from the repository root after installing and signing in to the apps. The suite uses AndroidX UI Automator.

With a reachable backend and seeded account, verify in each app:

1. Sign in, background/resume, restore session and refresh tokens.
2. Product CRUD/history, image toggling, ratings, CSV/PDF sharing, CSV import confirmation/cancellation.
3. File upload/download/metadata, picker cancellation and unsaved-edit cancellation.
4. User password/email actions, role CRUD, configuration CRUD and Excel import/export.
5. Audit paging, JSON copy, live notification, theme switching.
6. Disconnected backend, denied permission, expired session, and logout clearing protected records.

iOS requires macOS/Xcode. Configure production signing before distribution.

## Validation performed

- Flutter: analyzer clean; eight widget/payload tests passed, covering feature validation, password confirmation, file edit restrictions, sensitive values, history rendering, signed-out access and the product editor.
- Kotlin: six feature workflow tests and Android debug APK build passed.
- React Native: lint, TypeScript checks, five contract tests, eight rendered-screen tests and Android JavaScript/Hermes export passed. Tests cover feature-specific validation/payloads, details/history, passwords, import confirmation, navigation, role creation, sensitive configuration editing and discard confirmation.
- OpenIddict and Duende projects containing the client registrations built successfully (existing analyzer warnings remain).
- Flutter and React Native APK packaging has not been revalidated for these changes. An earlier attempt exhausted disk space during native dependency extraction; repair/install any interrupted SDK components in Android Studio before building. React Native requires NDK `27.1.12297006`.
- Kotlin, Flutter and React Native Android authentication and selected UI/API flows were exercised as described below. iOS builds remain unverified. Follow the integration checklist above for remaining coverage.

### Live backend checks — 2026-11-10

Tested the running ModularMonolith API at `https://localhost:44312` and OpenIddict at `https://localhost:44367` using the supplied development account. These were host-side HTTP integration tests, not native UI tests.

- All three mobile client registrations completed authorization-code login with S256 PKCE, token exchange, refresh-token exchange, and an API call using the refreshed token.
- Authenticated product, file, user, role, configuration and paged audit lists returned HTTP 200. Unauthenticated requests returned HTTP 401.
- Product, role and configuration create/update/read/delete checks passed. Configuration value whitespace was preserved.
- File upload, download content comparison, metadata update, history and deletion passed.
- Product CSV export, configuration Excel export and SignalR negotiation passed. PDF conversion, import processing and live notification delivery were not exercised.
- All temporary product, role, configuration and file records were deleted. Their normal audit events remain. The supplied user was not modified; no email actions were invoked.
- **Backend gap found during this run:** `GET /api/users/{id}/auditlogs` returned HTTP 404. The route has since been implemented in ModularMonolith with regression coverage; restart the API after rebuilding to load it. It returns existing user audit snapshots, or an empty list when there are none.
- **Native emulator follow-up:** Medium_Phone now connects as ADB `device`. Kotlin APK installation, launch, signed-out screen rendering, dark-theme switching and theme persistence after process restart passed. Sign-in displayed `Network error`; AppAuth logged `SSLHandshakeException` / `Trust anchor for certification path not found` while fetching Identity Server discovery. Port forwarding for 44312 and 44367 was active. Android certificate trust blocks native login and authenticated UI checks; Flutter and React Native emulator UI checks remain unverified. Dark mode also leaves dark status-bar icons against a dark background, making them hard to read.
- **Kotlin fixes verified:** system-bar icon appearance now follows the selected theme; light icons were verified against the dark background on Medium_Phone. Six workflow tests and the debug APK build passed. The optional `debugCertificatePath` build property trusts a specific public development certificate for `localhost` only, using debug-only generated resources; the release manifest excludes this trust configuration. With the certificate actually served by the API/Identity Server (which differed from the Windows certificate-store certificate), discovery succeeded and opened Chrome. Chrome then displayed `NET::ERR_CERT_AUTHORITY_INVALID`; browser trust and authenticated UI checks remain blocked. See [Kotlin HTTPS setup](Kotlin/README.md) for the separate browser trust requirement.

### Kotlin authenticated UI follow-up — 2026-11-10

After the user bypassed Chrome's local certificate warning and completed login, testing continued on Medium_Phone. This enabled testing but did not establish browser certificate trust.

- Passed: browser redirect back to the native workspace; product, file (empty state), user, role and configuration lists; product details; product history's empty state; required-name validation; unsaved-edit discard confirmation and cancellation of the editor without saving.
- Passed: audit JSON expansion/collapse and server pagination from page 1 to page 2; Notifications displayed `Connected` (actual message delivery was not tested).
- Passed: force-stop/relaunch restored the signed-in session and dark theme; products loaded again without another login. No AndroidRuntime crash was logged during the checks. Token expiry/refresh was not forced in this UI run.
- Reproduced: user History displays `Request failed (404)`, matching the missing backend endpoint.
- UI defect: after an empty product save, `Name is required` also appears in the discard-confirmation dialog. Validation feedback should remain scoped to the editor.
- No backend records were created, changed or deleted in this Kotlin UI run; no password or email actions were invoked. Kotlin native save/delete workflows, file transfers, imports/exports, logout and live notification delivery remain to be tested. Earlier host-side API checks do not substitute for those native UI checks.

### Flutter authenticated UI follow-up — 2026-11-10

- Built and installed the Android debug APK on Medium_Phone. Debug-only trust for the actual localhost server certificate covers both Dart HTTP and Android AppAuth. Removing the empty Android `taskAffinity` fixed the browser return reporting a cancelled login. Release builds do not enable this certificate trust.
- Passed: login return; product, file, user, role and configuration lists; required product fields; product create/edit/history/delete; audit JSON expansion/collapse and pagination; notification hub connection; dark theme.
- Passed: native file picker, required description validation, upload, download to the Android share sheet, and deletion. The downloaded 44-byte test file matched the uploaded contents. Product CSV export also produced a file and opened the share sheet; the product image toggle showed placeholders for records without images. No file was shared to another application.
- Passed: force-stop/relaunch restored the session and dark theme; authenticated products loaded without another login.
- Temporary product and file records were deleted; normal audit events remain. Password/email actions, import processing, PDF/Excel exports, forced token expiry, logout, actual notification delivery and iOS were not tested in this native run.

### React Native Android follow-up — 2026-11-10

- Android debug APK built and installed on Medium_Phone after repairing the incomplete local NDK installation and resolving Windows CMake/Ninja path-length failures. The Expo config plugin limits CMake object paths to 240 characters; testing used a short physical project path.
- Lint, TypeScript, five contract tests, eight rendered-screen tests and the Android Metro bundle passed. The installed app displayed its signed-out screen and opened Identity Server in Chrome after Android local-network permission was granted by the user.
- Fixed and verified readable status-bar icons in light and dark themes. A stalled, blank Chrome custom tab recovered after restarting Chrome and retrying sign-in.
- Passed on the emulator: login return; product, file (empty state), user, role and configuration lists; required-name validation; product create/update/history/delete; audit JSON expansion; notification hub connection. The temporary product was deleted and its normal audit events remain.
- Passed: force-stop/relaunch restored the signed-in session and dark theme. Native React Native file transfers, imports/exports, forced token expiry, logout, audit pagination and actual notification delivery remain unverified. No password/email actions were invoked.

### Shared mobile appearance

All three apps use the same visual tokens with native controls: indigo primary actions (`#4f46e5`), outlined secondary actions, compact horizontal navigation, section headings, rounded cards and inputs, and separate dialog header/body/footer areas. Light backgrounds use `#f3f6fb` with white cards; dark backgrounds use `#0b1220` with `#172033` cards. Status-bar icons follow the selected theme.

Keep styling aligned in Flutter's `lib/shared/widgets/app_theme.dart`, Kotlin's `shared/ui/AppTheme.kt`, and React Native's `src/shared/ui/ui.tsx`. Feature forms remain explicit native components; these themes do not introduce field definitions or a runtime UI schema.

Home navigation, Sign out, and the `Practical.CleanArchitecture` project link use a dedicated native `HomeButton`: full width, minimum height 48, corner radius 10, neutral 14-point semibold text, and an outlined surface. Navigation buttons have 16-unit vertical gaps; the project link fills its welcome card. Destructive actions and delete confirmations use the same light/dark red text, background, and border colors across all three apps.

Navigation and common actions pair their text labels with matching outline icons. The icons inherit each control's normal, selected, disabled or destructive color and are decorative for screen readers. Regenerate the bundled assets for all three apps with `python src/Mobiles/tools/generate_icons.py` from the repository root; no runtime icon dependency is required.

Check action-label coverage and identical assets across all three apps with `python src/Mobiles/tools/check_icons.py`. This includes pagination, file/date pickers, account actions, theme choices, popup footers and import/export buttons.

Record lists and Audit logs share a pagination layout in each app: a centered page/record summary above equal-width Previous and Next buttons, 12-unit spacing and a minimum 48-unit height. Both buttons remain visible and are disabled while loading or at the relevant page boundary.
