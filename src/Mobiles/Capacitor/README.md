# Classified Ads - React + Capacitor

A standalone React/TypeScript application bundled into Capacitor Android and iOS apps. It uses the React web application's feature implementations, adapted for touch screens and native authentication, secure storage, downloads and sharing. It does not load the website from a remote server.

## Features

- Products: search/paging, details, create/edit/delete, validation, images/rating, audit history, CSV import and CSV/PDF export.
- Files: upload with encryption choice, edit metadata, download/share, delete and audit history.
- Users: account fields, create/edit/delete, password actions, confirmation/reset emails and history.
- Roles and configuration: CRUD, sensitive-value masking, Excel import/export for configuration.
- Audit logs: server pagination, JSON viewing and copying.
- Notifications: foreground SignalR connection and recent-message history; these are in-app notifications, not OS push notifications.
- Shared mobile appearance, light/dark/system themes, custom dialogs/toasts, icons, touch targets and responsive record cards.

The API controls permissions. Import endpoints currently parse files without persisting records, matching the existing web clients.

## Requirements and development

Node 24, Java 21, Android SDK/Studio with SDK 36; macOS with Xcode 26.2 for iOS. Native projects are committed under `android` and `ios`; do not recreate them with `cap add`.

```powershell
cd src/Mobiles/Capacitor
npm ci
Copy-Item .env.example .env.local
npm run dev
```

Browser development runs on port 3000 and uses `ClassifiedAds.React`, so stop the React/Next.js development server first. Native builds use `ClassifiedAds.Mobile.Capacitor`. Restart Identity Server after updating the repository to seed this public PKCE client. Its callbacks are `classifiedads.capacitor:/oauthredirect` and `classifiedads.capacitor:/logout`.

Configure `VITE_API_URL`, `VITE_AUTHORITY`, and `VITE_NOTIFICATION_URL` in `.env.local` before building. These are public bundled settings, never secrets. Defaults are localhost ports 44312 and 44367. Native sign-in uses the system browser; OIDC state and tokens use Keychain/Keystore-backed storage. Browser preview uses browser storage.

## Android

```powershell
npm run build
npx cap sync android
$env:JAVA_HOME = 'C:/Program Files/Android/Android Studio/jbr'
$env:ANDROID_HOME = "$env:LOCALAPPDATA/Android/Sdk"
./android/gradlew.bat -p android assembleDebug
& "$env:ANDROID_HOME/platform-tools/adb.exe" reverse tcp:44312 tcp:44312
& "$env:ANDROID_HOME/platform-tools/adb.exe" reverse tcp:44367 tcp:44367
& "$env:ANDROID_HOME/platform-tools/adb.exe" install -r android/app/build/outputs/apk/debug/app-debug.apk
```

Package ID: `com.classifiedads.mobile.capacitor`. Open `android` in Android Studio, or use `npm run android`. The JavaScript bundle is included; Metro is not needed.

For a local self-signed HTTPS server, export its **public certificate only** and build with `-PdebugCertificatePath=C:/path/localhost.cer`. This adds trust for localhost only in debug builds; it does not disable certificate checks or affect release builds. The system browser has its own certificate trust and may require handling its development-certificate prompt separately. Production needs publicly trusted HTTPS URLs.

## iOS

```sh
npm run build
npx cap sync ios
npx cap open ios
# Unsigned simulator build:
xcodebuild -project ios/App/App.xcodeproj -scheme App -configuration Debug \
  -sdk iphonesimulator -destination 'generic/platform=iOS Simulator' \
  -derivedDataPath build/ios CODE_SIGNING_ALLOWED=NO build
```

Bundle ID: `com.classifiedads.mobile.capacitor`. Install the localhost development certificate in the simulator when using the local HTTPS backend. Device distribution requires normal Apple signing/provisioning. Swift Package Manager dependencies are synchronized by Capacitor.

## Tests

```powershell
npm run lint
npm run typecheck
npm test
npm run test:ui
# From repository root, after native installation and sign-in:
./src/Mobiles/tests/Run-UiTests.ps1 -App Capacitor
./src/Mobiles/tests/Run-UiTests.ps1 -App Capacitor -Feature Products -IncludeCrud
```

Unit tests cover callback allowlisting, return URLs, secure storage, file sharing and optional crash reporting. Playwright tests render the mobile layout with isolated API fixtures; they do not prove native authentication or backend behavior. Android uses the **same AndroidX UI Automator feature suites** as the other mobile apps, against the installed app and real backend. It requires a signed-in emulator session. CRUD is opt-in and uses a uniquely named temporary product. See [shared test runner](../tests/README.md) for reports and selection options.

## Firebase Crashlytics

Register Android and iOS apps with identifier `com.classifiedads.mobile.capacitor`, then prepare their downloaded configuration:

```powershell
python ../tools/prepare_firebase.py --app Capacitor --platform android --config C:/Downloads/google-services.json --require-config
python ../tools/prepare_firebase.py --app Capacitor --platform ios --config C:/Downloads/GoogleService-Info.plist --require-config
npm run build
npx cap sync
```

Native plugin inclusion is platform-specific and conditional on the configuration file. Without configuration, ordinary builds work without initializing Firebase. Android applies the Crashlytics Gradle plugin when configured; iOS copies the plist into the app and uploads dSYMs for Release device builds. JavaScript global errors/rejections are forwarded as sanitized non-fatal categories; native SDKs capture native failures. No passwords, tokens, request bodies or user-entered values are attached. Browser preview does not use Crashlytics.

CI secrets: `FIREBASE_CAPACITOR_ANDROID_CONFIG_BASE64` and `FIREBASE_CAPACITOR_IOS_CONFIG_BASE64`. See [Firebase setup and verification](../FIREBASE.md). Keep configuration files out of Git; omit them for ordinary UI test runs. A Firebase console smoke test still requires real Firebase registrations and configuration.

## CI

[Build Mobile Capacitor](../../../.github/workflows/build-mobile-capacitor.yml) has independent Android (Ubuntu) and iOS (macOS) jobs. Android runs lint, typecheck, unit and isolated mobile browser tests before building a bundled debug APK. iOS builds an unsigned simulator app. Artifacts are retained for 14 days. Authenticated Android device tests run separately because they require the backend and an interactive sign-in session.

## Verification

On 2026-11-22, the Android debug build installed and signed in on Medium_Phone (Android 17). All 13 shared UI Automator scenarios passed with `-IncludeCrud`, including temporary-product creation, update, audit history, deletion and cleanup. Reports: `../tests/artifacts/20261122-123907-Capacitor-All/reports/debug/index.html` (local, ignored by Git).

Lint, TypeScript checking, 8 unit tests, 14 mobile browser UI tests, and all 6 Firebase configuration tests passed. Action groups use equal-width columns with 12px gaps, and product searches survive editor navigation. The iOS workflow and property lists were checked structurally; an iOS build cannot be run on this Windows host. Firebase console delivery remains unverified until real configuration files are supplied.

## Sentry

Optional Sentry reporting is available independently of Crashlytics. See [Sentry setup](../SENTRY.md) for client configuration, CI/Expo upload settings, and verification. Missing configuration leaves reporting disabled.
