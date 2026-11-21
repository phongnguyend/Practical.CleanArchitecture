# Firebase Crashlytics

Crashlytics is optional in Flutter, Kotlin, React Native, and MAUI. Without configuration, the apps continue using the existing API and Identity Server and do not initialize reporting. Firebase Auth and Analytics are not required or installed by this integration.

## Create the Firebase apps

Create a Firebase project and open **Crashlytics** in the console. Register these seven apps in Project settings. Separate development and production Firebase projects are recommended. Use the identifiers exactly, including case.

| App | Platform | Package / bundle identifier |
| --- | --- | --- |
| Kotlin | Android | `com.classifiedads.mobile.kotlin` |
| Flutter | Android | `com.classifiedads.mobile.flutter` |
| Flutter | iOS | `com.classifiedads.mobile.flutter` |
| React Native | Android | `com.classifiedads.mobile.reactnative` |
| React Native | iOS | `com.classifiedads.mobile.reactnative` |
| MAUI | Android | `com.classifiedads.mobile.maui` |
| MAUI | iOS | `com.classifiedads.mobile.maui` |

Download each registration's `google-services.json` (Android) or `GoogleService-Info.plist` (iOS). These are client configuration files, not service-account credentials. Do not use service-account JSON here. Generated configuration is ignored by Git.

From the repository root, validate and copy a downloaded file:

```powershell
python src/Mobiles/tools/prepare_firebase.py --app Kotlin --platform android --config C:/Downloads/google-services.json --require-config
python src/Mobiles/tools/prepare_firebase.py --app Flutter --platform android --config C:/Downloads/flutter-google-services.json --require-config
python src/Mobiles/tools/prepare_firebase.py --app ReactNative --platform ios --config C:/Downloads/reactnative-GoogleService-Info.plist --require-config
python src/Mobiles/tools/prepare_firebase.py --app MAUI --platform ios --config C:/Downloads/maui-GoogleService-Info.plist --require-config
```

Repeat with the other registered platforms. Supported app names are `Flutter`, `Kotlin`, `ReactNative`, and `MAUI`; Kotlin supports Android only. Wrong app IDs, invalid files, and missing required values fail validation. No configuration contents are printed.

| App | Android destination | iOS destination |
| --- | --- | --- |
| Kotlin | `Kotlin/app/google-services.json` | - |
| Flutter | `Flutter/android/app/google-services.json` | `Flutter/ios/Runner/GoogleService-Info.plist` |
| React Native | `ReactNative/google-services.json` | `ReactNative/GoogleService-Info.plist` |
| MAUI | `MAUI/App/google-services.json` | `MAUI/App/GoogleService-Info.plist` |

Paths in this table are relative to `src/Mobiles`. For Flutter, the script also writes `Flutter/firebase-defines.json` with options for the **selected platform**. Run it again when switching between Android and iOS. Flutter initializes Firebase with these explicit options; its plist is retained for symbol uploads, rather than requiring an absent plist in every Xcode build.

## Build with reporting

Run commands from the corresponding app folder after preparing its configuration:

```powershell
# Kotlin (add -PcrashlyticsEnabled=true only for reporting builds)
./gradlew.bat :app:assembleDebug -PcrashlyticsEnabled=true

# Flutter: first prepare the correct platform's firebase-defines.json
flutter build apk --release --dart-define-from-file=firebase-defines.json
# On macOS, prepare the iOS configuration then:
flutter build ios --release --dart-define-from-file=firebase-defines.json

# React Native: native builds, not Expo Go. Restart Metro after config changes.
npx expo prebuild --platform android --no-install
npx expo run:android --variant release
# On macOS with iOS configuration:
npx expo run:ios --configuration Release

# MAUI
dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-android -p:EnableFirebaseCrashlytics=true
# On macOS:
dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-ios -p:MauiTargetFramework=net10.0-ios -p:EnableFirebaseCrashlytics=true
```

React Native enables each platform when its config exists. Provide both files before prebuilding both platforms. `FIREBASE_ANDROID_CONFIG` and `FIREBASE_IOS_CONFIG` can instead point to EAS file environment variables. RNFirebase v27 uses Swift Package Manager and dynamic iOS frameworks, configured by `app.config.js`. JavaScript and native crash reporting are active in release builds; debug builds are excluded.

For local UI tests, leave Kotlin's Gradle property and MAUI's MSBuild property unset, omit Flutter's `--dart-define-from-file`, and build React Native with `CRASHLYTICS_ENABLED=false`. Rebuild after changing configuration. When switching a previously reporting installation to tests, uninstall/clear app data first: Firebase collection preferences and unsent reports can persist between launches. This also clears the login session.

## Captured errors

- Kotlin: native JVM crashes and ANRs through the Android SDK; `CrashReporting.record(error)` is available for unexpected caught failures.
- Flutter: framework errors and uncaught asynchronous platform errors; `CrashReporting.record(error, stack)` for unexpected caught failures.
- React Native: RNFirebase's JavaScript exception handler and native crash handling; `recordUnexpectedError(error)` for unexpected caught failures.
- MAUI: native SDK reporting plus managed unhandled and unobserved task exception forwarding; `CrashReporting.Record(exception)` for unexpected caught failures. Managed reports are recorded as non-fatal by the community binding, even when the process is terminating.

The adapters attach the framework name. The explicit record helpers strip exception messages and preserve stack frames; they do not attach request bodies, tokens, account IDs, or user input. Native SDK automatic reports can contain exception messages, so do not put sensitive data in thrown exception messages or native logs. Expected validation errors, cancellations, and ordinary HTTP failures should not be reported as crashes. No deliberate crash is part of the normal UI test suite.

MAUI uses the community `Plugin.Firebase.Crashlytics` binding; it is not an official Firebase .NET SDK. Its native dependencies are included only when `EnableFirebaseCrashlytics=true`.

## GitHub Actions and Expo EAS

The mobile workflows call the same preparation script. Set the appropriate repository secrets to the **base64 contents of the downloaded file**:

- `FIREBASE_KOTLIN_ANDROID_CONFIG_BASE64`
- `FIREBASE_FLUTTER_ANDROID_CONFIG_BASE64`
- `FIREBASE_FLUTTER_IOS_CONFIG_BASE64`
- `FIREBASE_REACTNATIVE_ANDROID_CONFIG_BASE64`
- `FIREBASE_REACTNATIVE_IOS_CONFIG_BASE64`
- `FIREBASE_MAUI_ANDROID_CONFIG_BASE64`
- `FIREBASE_MAUI_IOS_CONFIG_BASE64`

For example, copy a local file's encoded contents to the secret value:

```powershell
[Convert]::ToBase64String([IO.File]::ReadAllBytes('C:/Downloads/google-services.json')) | Set-Clipboard
```

Missing secrets leave reporting disabled, including fork pull requests. Invalid supplied configuration fails the build. These changes do not change existing CI signing or simulator/debug build choices. React Native's existing release CI builds can report; its development builds do not.

The manually triggered Expo workflow prepares both files before submitting the cloud build. `.easignore` includes these two client config files in the build archive. For direct EAS builds outside GitHub Actions, prepare the files locally first, or supply the two EAS file environment variables above. Never upload service-account keys.

## Symbols and first verification

Android Kotlin/Flutter/RN builds with configuration apply the Crashlytics Gradle plugin, which handles build IDs and mapping uploads for applicable minified variants. MAUI uses the binding's documented Android mapping marker; it does not provide Gradle/R8 mapping uploads. Keep managed PDBs with MAUI release artifacts.

For production iOS device builds, generate and retain dSYMs and upload them with the Firebase Apple SDK's `Crashlytics/upload-symbols` utility for that SDK version:

```sh
/path/to/Crashlytics/upload-symbols -gsp /path/to/GoogleService-Info.plist -p ios /path/to/App.app.dSYM
```

Locate that utility in the Firebase SDK downloaded by SPM, CocoaPods, or the MAUI native package. The current CI iOS jobs build simulator apps, not signed device archives; they do not constitute production symbol-upload verification. Before distributing a device release, confirm there are no missing-dSYM warnings in Firebase. Flutter obfuscated builds also need the matching `--split-debug-info` output uploaded using the Firebase CLI as described in the Flutter guide below. Crashlytics native symbols do not replace JavaScript source maps; retain React Native bundle/source maps when diagnosing minified JavaScript stacks.

After real configuration is available:

1. Build and install a reporting-enabled app against the development Firebase project.
2. Call the corresponding record helper with a synthetic unexpected exception containing no user data. Confirm a non-fatal event and the framework key in Firebase.
3. For native fatal-crash verification, use the SDK's documented test crash in a temporary development-only action, without a debugger attached. Relaunch the app to upload the pending report.
4. Verify useful stacks and correct app/version in Firebase, then remove the temporary crash action. Repeat for every platform registration; iOS needs macOS/device or simulator testing.

Configuration preparation is tested with `python -m unittest discover -s src/Mobiles/tools -p test_prepare_firebase.py`. React Native's `npm test` also tests configured and disabled Expo settings. No reports can be verified in the Firebase console until a project and real configuration files exist.

References: [Android setup](https://firebase.google.com/docs/crashlytics/android/get-started), [Flutter setup](https://firebase.google.com/docs/crashlytics/flutter/get-started), [React Native Firebase](https://rnfirebase.io/crashlytics/usage), [MAUI binding](https://github.com/TobiasBuchholz/Plugin.Firebase/blob/master/docs/crashlytics.md).
