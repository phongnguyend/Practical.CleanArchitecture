# React Native / Expo

See [mobile features and backend setup](../README.md). Uses Expo SDK 58; validated with Node 24. Android requires Android Studio/SDK/JDK; iOS requires macOS/Xcode.

```sh
npm ci
```

Copy `.env.example` to `.env` and configure reachable, trusted backend URLs.

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
