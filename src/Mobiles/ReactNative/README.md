# React Native / Expo

See [mobile features and backend setup](../README.md). Uses Expo SDK 58; validated with Node 24. Android requires Android Studio/SDK/JDK; iOS requires macOS/Xcode.

```sh
npm ci
```

`npm ci` also applies a guarded Swift 6.2 workaround to the locked `expo-modules-jsi@58.0.22` dependency for [Expo issue #52754](https://github.com/expo/expo/issues/52754). It preserves the temporary buffer's lifetime while passing its address into the synchronous JavaScript actor closure. The patch is idempotent and rejects unexpected dependency versions or source changes. Review and remove `scripts/patch-expo-modules-jsi.cjs` and its `postinstall` hook once an upstream release fixes the issue. iOS CI selects Xcode 26.2; Xcode 16.4's Swift 6.1 cannot build this package.

Copy `.env.example` to `.env` and configure reachable, trusted backend URLs.

## Expo EAS cloud builds

The `React Native Expo EAS` job in [Build Mobile React Native Expo](../../../.github/workflows/build-mobile-reactnative-expo.yml) submits both platforms to Expo from Ubuntu and waits for their results. The `ci` profile in `eas.json` produces an Android APK and an iOS simulator app; it does not submit to app stores. The existing native Android and iOS jobs still run independently.

One-time setup from this directory:

```sh
npx eas-cli@latest login
npx eas-cli@latest init
npx eas-cli@latest build --platform android --profile ci
npx eas-cli@latest build --platform ios --profile ci
```

Link the existing Expo project with the `practical-clean-architecture` slug. Complete the initial interactive builds to configure Android signing credentials and verify project access. Simulator builds do not require Apple distribution credentials. Retain the generated `extra.eas.projectId` in `app.json` for local builds. The slug must match the project selected by `EXPO_PROJECT_ID`.

In GitHub repository Settings → Secrets and variables → Actions, add:

- Secret `EXPO_TOKEN`: an Expo access token with access to the project.
- Variable `EXPO_PROJECT_ID`: the UUID shown in the Expo project settings (matching `extra.eas.projectId`).

For an on-demand build, select **Build Mobile React Native Expo → Run workflow**. Pull requests never run the credentialed cloud job. CI links the configured project in its checkout without committing that change. Build logs and downloads are available on Expo; the `reactnative-expo-build-results` GitHub artifact contains build metadata and artifact URLs. Cloud builds use the Expo account's quota, and cancelling GitHub's waiting job may require cancelling the submitted builds separately on Expo.

Configure public backend URLs (`EXPO_PUBLIC_API_URL`, `EXPO_PUBLIC_AUTHORITY`, and `EXPO_PUBLIC_NOTIFICATION_URL`) in the Expo project's `preview` environment. The existing `postinstall` Swift compatibility patch also runs on Expo's builders. See [Expo's CI setup guide](https://docs.expo.dev/build/building-on-ci/) for token and initial-build setup.

## Local development

```sh
npm run android
# macOS:
npm run ios
npm run typecheck
npm run lint
npm test
npm run test:ui
npx expo export --platform android
```

Use a native development build, not Expo Go: login requires the app's `classifiedads.rn` callback. `expo run:android`/`expo run:ios` generate and install native projects from `app.json`. Generated `android/` and `ios/` directories are ignored; regenerate after native configuration changes. Environment values are public configuration, never secrets.

On Windows, keep the physical checkout path short for Android CMake/Ninja builds. A long checkout path caused native compilation failures during emulator testing. A `subst` drive alone did not solve this because Expo resolved dependencies back to the original path. When relocating a checkout, regenerate native projects and build caches rather than copying generated autolinking files containing absolute paths.

The `withAndroidBuildPaths` config plugin also sets CMake's object-path limit to 240 so generated C++ paths are shortened before Ninja encounters Windows path-length limits. Run prebuild after changing this plugin.

For local Android HTTPS testing, forward ports with `adb reverse tcp:44312 tcp:44312` and `adb reverse tcp:44367 tcp:44367`. Export the public certificate actually served by the backend and configure the debug-only plugin before generating/building Android (PowerShell):

```powershell
$env:MOBILE_DEBUG_CERTIFICATE = 'C:/path/to/server-public.pem'
npx expo prebuild --platform android --no-install
npm run android
```

The plugin trusts this certificate only for `localhost` in Android debug builds. It preserves Expo's debug HTTP access for Metro; API and issuer URLs should remain HTTPS. No certificate validation bypass is installed, and release builds do not include the debug resources. Remove the environment variable and rerun prebuild to remove the local certificate. The system login browser has its own trust store; see [Kotlin HTTPS setup](../Kotlin/README.md).

`src/app` contains thin Expo Router adapters. `src/features/workspace/WorkspaceScreen.tsx` handles navigation, session state, themes and notifications. Each resource folder owns its typed `model.ts`, `api.ts`, explicit form/detail components and screen. `src/core` separates authentication, configuration, HTTP, file transfers, notifications and appearance storage. `src/shared/ui` provides controls and reusable record-page lifecycle behavior without field schemas. Tests validate feature payloads and rendered workflows. See the parent README for dependency boundaries.

## Crash reporting

See [Firebase Crashlytics setup](../FIREBASE.md) for optional reporting, Firebase app registrations, local configuration, CI secrets, and verification. Ordinary builds work without Firebase configuration.
