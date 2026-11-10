# Flutter

See [mobile features and backend setup](../README.md). Requires Flutter stable with Dart >=3.13.7 (validated with Flutter 3.47.9), Android SDK/JDK, or macOS/Xcode for iOS.

```sh
flutter pub get
flutter run --dart-define=API_URL=https://api.example.com/api/ --dart-define=AUTHORITY=https://identity.example.com --dart-define=NOTIFICATION_URL=https://api.example.com/hubs/notification
flutter analyze
flutter test
flutter build apk --debug
```

Replace example hosts with reachable trusted addresses. Omitted values use localhost defaults. Configure the signing team in `ios/Runner.xcworkspace` for iOS deployment. Both platforms register `classifiedads.flutter` callbacks.

For local Android HTTPS testing, forward ports with `adb reverse tcp:44312 tcp:44312` and `adb reverse tcp:44367 tcp:44367`. Export the public certificate actually served by the backend, then use both Android and Dart debug trust settings (PowerShell):

```powershell
$debugCert = [Convert]::ToBase64String([IO.File]::ReadAllBytes('C:/path/to/server-public.pem'))
flutter run --debug "--dart-define=DEBUG_CERTIFICATE_BASE64=$debugCert"
```

The Android build derives its certificate from the same Dart define: AppAuth trusts that certificate for `localhost` only, while Dart's debug TLS context trusts the supplied certificate and retains hostname verification. Neither setting disables certificate verification. Use only your own local development certificate, never a private key. The Android resources are debug-only and Dart ignores this setting outside debug mode. Omit the define and rebuild when finished. The system login browser has independent certificate trust, as described in the [Kotlin HTTPS setup](../Kotlin/README.md).

`lib/app` owns app composition and theme setup. `lib/features/workspace` handles navigation, sessions and notifications. Each resource folder owns a typed `model.dart`, `repository.dart`, explicit `form.dart`, `details.dart` and `screen.dart`. `lib/core` contains configuration and platform services. `lib/shared/models` contains repository contracts and audit/error helpers; `lib/shared/widgets` contains reusable controls and record-page lifecycle behavior without field schemas. Tests cover feature validation, passwords, file restrictions and native rendering. Configure production signing before release.
