# Sentry for mobile apps

Sentry is optional in Kotlin, Flutter, React Native, MAUI, and Capacitor. It is independent of Firebase Crashlytics: either provider, both, or neither can be configured. No Sentry account is required to build or run the apps. Missing configuration leaves Sentry uninitialized.

## Create projects later

Create one Sentry project per framework (Kotlin/Android, Flutter, React Native, .NET MAUI, and Capacitor/React). Android and iOS can share the framework project's DSN. Kotlin has only an Android target.

Record each project's **client DSN**, organization slug, and project slug. The client DSN is public app configuration. The **organization auth token** is a build secret for symbol uploads: never put it in app settings, source control, or `EXPO_PUBLIC_*` variables.

## Local configuration

From the repository root, prepare an app's ignored `sentry.config.json`:

```powershell
$env:SENTRY_DSN = 'https://PUBLIC_KEY@HOST/PROJECT_ID'
$env:SENTRY_ENVIRONMENT = 'development'
$env:SENTRY_RELEASE = 'mobile-flutter@1.0.0+1'
python src/Mobiles/tools/prepare_sentry.py --app Flutter
```

Repeat with `Kotlin`, `ReactNative`, `MAUI`, or `Capacitor` and the corresponding DSN/release. The generator validates HTTPS DSNs and only writes four public settings: `SENTRY_ENABLED`, `SENTRY_DSN`, `SENTRY_ENVIRONMENT`, and `SENTRY_RELEASE`. Set `SENTRY_ENABLED=false` and rerun to disable reporting; rerunning without a DSN also overwrites stale configuration with a disabled configuration. Rebuild after changing settings.

| App | How configuration reaches the app | When reporting runs |
| --- | --- | --- |
| Kotlin | Gradle generates BuildConfig constants from the JSON | Configured release builds |
| Flutter | Pass `--dart-define-from-file=sentry.config.json` to `flutter build` | Configured release builds |
| React Native | Expo app config includes public settings in `extra.sentry` | Configured non-development builds; rebuild the native app after adding the SDK |
| MAUI | The optional JSON is embedded in the app assembly | Configured Release builds |
| Capacitor | Vite injects the public configuration at build time | Configured production web bundles running inside the native app; includes bundled debug APKs |

Flutter's Firebase and Sentry define files can be used together:

```powershell
flutter build apk --release --dart-define-from-file=firebase-defines.json --dart-define-from-file=sentry.config.json
```

For React Native, run `npx expo prebuild --platform android --no-install` or the equivalent iOS command before building. The Sentry Expo plugin is enabled when configuration is enabled. Metro uses Sentry's configuration for JavaScript debug IDs. This requires a native build, not Expo Go.

For Capacitor, run `npm run build` then `npx cap sync`. Both Android and iOS plugin registrations are included.

Keep Capacitor's `@sentry/capacitor` and `@sentry/react` versions pinned exactly and update them together. The Capacitor SDK's install check rejects version ranges for its React sibling, even when the lockfile resolves to the required version. The current compatible pair is `@sentry/capacitor@4.6.0` and `@sentry/react@10.76.2`.

Upload integrations are guarded by the presence of all required upload settings. React Native also accepts `SENTRY_DISABLE_AUTO_UPLOAD=true` to explicitly skip uploads; rerun prebuild after changing upload settings. Keep reporting disabled for ordinary UI automation; use a separate development Sentry project for deliberate reporting checks.

## GitHub Actions

All six mobile build workflows prepare optional configuration. Missing secrets remain valid, including fork pull requests. Existing Android/iOS jobs remain independent.

Repository secrets:

- `SENTRY_KOTLIN_DSN`
- `SENTRY_FLUTTER_DSN`
- `SENTRY_REACTNATIVE_DSN`
- `SENTRY_MAUI_DSN`
- `SENTRY_CAPACITOR_DSN`
- `SENTRY_AUTH_TOKEN` (organization upload token)

Repository variables:

- `SENTRY_ORG`
- `SENTRY_KOTLIN_PROJECT`, `SENTRY_FLUTTER_PROJECT`, `SENTRY_REACTNATIVE_PROJECT`, `SENTRY_MAUI_PROJECT`, `SENTRY_CAPACITOR_PROJECT`
- `SENTRY_ENVIRONMENT` (defaults to `ci`)

CI releases are `mobile-<framework>@<git SHA>`. Symbol uploads require the token, organization, and matching project variable. Upload failures with supplied credentials fail the build so broken symbolication is visible. Configuring a DSN does not change the workflow's build type: Kotlin and Flutter debug artifacts still do not report; use release builds to verify them.

### React Native Expo cloud builds

The manual Expo workflow includes the public JSON in the EAS archive. Also configure `SENTRY_AUTH_TOKEN`, `SENTRY_ORG`, and `SENTRY_PROJECT` in the Expo **preview** environment used by `eas.json`. The GitHub runner's environment is not automatically forwarded to cloud workers. Keep the token secret. Set `SENTRY_DISABLE_AUTO_UPLOAD=true` in that Expo environment if you intentionally omit upload credentials, or `false` when uploads are configured. The Sentry Expo plugin and Metro integration handle native symbols and JavaScript source maps in cloud builds.

## Debug symbols

- Kotlin: Sentry Gradle plugin uploads R8/ProGuard mappings and native symbols when credentials are available. Current debug builds are not minified.
- Flutter: Sentry Gradle plugin handles Android mappings; `dart run sentry_dart_plugin` uploads available Dart/native debug files after the build. Source-code upload is disabled. If adding obfuscation, preserve `--split-debug-info` output and configure the Dart symbol map as described in Sentry's guide.
- React Native: the Expo plugin adds native build upload steps; Metro supplies JavaScript debug IDs.
- Capacitor: the Vite plugin generates and uploads hidden source maps, then removes `.map` files before packaging. The iOS workflow uploads available native debug files. Android is currently not minified.
- MAUI: CI uploads available managed PDBs and native debug files from `App/bin/Release` and `App/obj/Release` (including trimmed PDBs and native build outputs) using the pinned Sentry CLI.

Simulator/debug builds do not generate all symbols available in signed device releases. Retain the matching release symbols when adding store deployment workflows.

## Errors and privacy

Automatic SDK handlers capture unhandled errors. Existing explicit error-reporting helpers also send unexpected caught failures to Sentry when enabled. Firebase automatic-handler forwarding avoids calling Sentry's explicit helper a second time. If both providers are enabled, each dashboard can receive the same failure by design.

Reporting starts with error monitoring only: tracing sampling is zero, replay is not enabled, default PII collection is off, and breadcrumb capacity is zero. Application-level event filters remove user/request/message data and replace exception messages while preserving stack frames. Native crash reports in hybrid SDKs do not necessarily pass through JavaScript or Dart filters: configure Sentry server-side scrubbing as well, and never include credentials or user input in native exception messages. This integration does not attach account identities or API bodies.

## Verification after provisioning

1. Build a configured release against a development Sentry project, with matching upload settings.
2. Invoke the app's caught-error helper with a fixed, non-sensitive test exception; confirm one event and readable stack frames in Sentry.
3. In a separate disposable test build, trigger a native crash, relaunch the app, and verify delivery. Do not include intentional crashes in the regular UI suite.
4. Verify both providers separately if enabling Sentry and Crashlytics together.
5. Rebuild without configuration and confirm normal startup and UI tests still pass.

Local compilation and mocked tests cannot prove dashboard delivery or symbol upload. Those checks need your Sentry project information.

Official setup references: [Android](https://docs.sentry.io/platforms/android/), [Flutter](https://docs.sentry.io/platforms/dart/guides/flutter/), [Expo](https://docs.expo.dev/guides/using-sentry/), [MAUI](https://docs.sentry.io/platforms/dotnet/guides/maui/), [Capacitor](https://docs.sentry.io/platforms/javascript/guides/capacitor/), [Flutter symbols](https://docs.sentry.io/platforms/dart/guides/flutter/debug-symbols/dart-plugin/).

## Implementation verification (2026-11-22)

- Android builds passed for Kotlin, Flutter (x86_64 debug), MAUI (Release), and Capacitor.
- Kotlin: 6 unit tests passed. Flutter: analyzer and 12 tests passed. MAUI: 11 tests passed.
- React Native: TypeScript, lint, 10 contract/config tests, and 10 Jest tests passed. Expo Android prebuild and Sentry Java compilation passed. Full APK and isolated Sentry native-library packaging were blocked by the documented Windows CMake/Ninja path-length limit in this checkout (worklets and Sentry C++ object paths). A full native build still needs verification in the Linux CI job or a physically shorter checkout.
- Capacitor: TypeScript, lint, 10 unit tests, and 16 browser UI tests passed.
- Four shared Sentry configuration tests passed; all six workflow YAML files were checked.
- iOS compilation was not run on this Windows host. Event delivery, symbol uploads, and simultaneous native crash delivery to both providers remain unverified until Sentry projects and credentials are supplied.
