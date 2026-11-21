# Classified Ads - .NET MAUI

Native C# Android and iOS client. The app uses concrete feature forms, shared native controls, and the same API contracts and Android UI test scenarios as the other mobile clients.

## Features

- Browser OpenID Connect authorization-code/PKCE login, secure token storage, refresh, and logout.
- Products: search, pagination, details, CRUD, images/ratings, audit history, confirmed CSV processing, CSV/PDF sharing.
- Files: pick/upload with encryption choice, metadata editing, details, download/share, delete, audit history.
- Users: all account fields, CRUD, history, matching password confirmation, confirmed reset/confirmation email requests.
- Roles and configuration: CRUD, sensitive-value masking, confirmed Excel processing and export.
- Server-paged audit logs with local page search and expandable/copyable JSON.
- Foreground SignalR notifications, latest 100 messages, connection status and reconnect.
- Matching mobile colors, action icons, dialogs, destructive buttons, pagination, live home clock, persisted System/Light/Dark theme.

CSV/Excel import endpoints parse files; they do not persist the returned records. The app displays the processing result explicitly.

## Structure

- `App/Features`: concrete feature screens/forms.
- `App/Shared`: native buttons, cards, dialogs, JSON viewer and reusable record-list lifecycle.
- `App/Core`: platform services, secure authentication, notifications and file sharing.
- `Core/Features`: concrete feature models. `Core/Models` and `Core/Network`: platform-independent shared models, validation, API contracts and pagination.
- `Tests`: xUnit validation and HTTP contract tests; no MAUI workload required.
- `../tests`: shared Android UI Automator scenarios, including `MAUIUiTest`.

## Android

Install .NET SDK 10.0.302 (pinned in `global.json`), Android Studio SDK tools and JDK 21. Run commands from this directory:

```powershell
dotnet workload install maui-android --version 10.0.303.1
dotnet test Tests/ClassifiedAds.Mobile.Tests.csproj
dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-android
# With one emulator running:
dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-android -t:Run
```

If SDK/JDK discovery fails, supply `-p:AndroidSdkDirectory="<Android SDK path>" -p:JavaSdkDirectory="<JDK path>"`. Android Studio includes a compatible JDK under its `jbr` directory.

The application ID is `com.classifiedads.mobile.maui`. A Release APK is standalone and does not need a development server:

```powershell
dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-android -c Release -p:AndroidPackageFormats=apk
```

## iOS (macOS)

Use .NET SDK 10.0.302 and Xcode 26.6 with the pinned workload set. From this directory:

```sh
dotnet workload install maui-ios --version 10.0.303.1
DEVELOPER_DIR=/Applications/Xcode_26.6.app/Contents/Developer dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-ios -p:MauiTargetFramework=net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64 -p:EnableCodeSigning=false
```

Use `iossimulator-x64` on an Intel Mac. Physical-device deployment requires your Apple signing identity and provisioning profile.

`MauiTargetFramework` limits restore to the app's iOS target. Do not override `TargetFrameworks` on the command line: that global property also changes the shared Core project's restore targets, which must remain `net10.0`.

## Servers and sign-in

Default URLs match the other mobile apps:

- API: `https://localhost:44312/api/`
- Authority: `https://localhost:44367`
- Hub: `https://localhost:44312/hubs/notification`

Change URLs with **Server settings** on the signed-out screen. Use HTTPS endpoints with a certificate trusted by both the device and browser. No certificate-validation bypass is shipped. Android debug builds accept user-installed CAs; release builds use system trust only.

Alternatively, an Android Debug build can explicitly trust the server's public development certificate for `localhost` only:

```powershell
dotnet build App/ClassifiedAds.Mobile.csproj -f net10.0-android -p:EmbedAssembliesIntoApk=true -p:DebugCertificatePath="C:/path/to/localhost.cer"
```

Use the public certificate actually served by the API/Identity Server. This option is ignored in Release builds, does not trust other hosts, and does not change browser trust. `EmbedAssembliesIntoApk=true` makes the Debug APK independently installable for UI tests.

For an Android emulator, forward the host ports:

```sh
adb reverse tcp:44312 tcp:44312
adb reverse tcp:44367 tcp:44367
```

Restart the Identity Server to seed the `ClassifiedAds.Mobile.MAUI` public client (both OpenIddict and Duende seeders include it). Redirect URI: `classifiedads.maui:/oauthredirect`; logout URI: `classifiedads.maui:/logout`. Scope: `openid profile offline_access ClassifiedAds.WebAPI`. No client secret is used.

## UI tests

Install the APK, run the API/Identity Server, and sign in once. From the repository root:

```powershell
./src/Mobiles/tests/Run-UiTests.ps1 -App MAUI
./src/Mobiles/tests/Run-UiTests.ps1 -App MAUI -Feature Products
./src/Mobiles/tests/Run-UiTests.ps1 -App MAUI -IncludeCrud
```

The shared suite covers home layout, theme/session persistence, navigation, audit paging, notifications, form validation, discard confirmation, and optional temporary-product CRUD/history/cleanup. It requires a live signed-in device; unit tests use an in-memory HTTP handler. See [shared test documentation](../tests/README.md).

## GitHub Actions

[Build Mobile MAUI](../../../.github/workflows/build-mobile-maui.yml) runs on changes to this project, pull requests, and manual dispatch. Android runs on Ubuntu and iOS on macOS in independent parallel jobs. Android runs xUnit tests and uploads a debug-key-signed Release APK plus TRX reports. iOS uploads an unsigned simulator `.app` archive. These are verification artifacts; store distribution/signing is not configured. Live authenticated UI tests run separately using the commands above.

## Android verification

On 2026-11-21, all 13 shared UI scenarios passed on Medium_Phone (Android 17): 12 non-destructive cases in the full suite, then the optional product CRUD/history/cleanup case in a separate targeted run. The app retained its authenticated session across restarts and APK updates. Local HTTPS used `DebugCertificatePath`; release resources exclude this certificate.

## Crash reporting

See [Firebase Crashlytics setup](../FIREBASE.md) for optional reporting, Firebase app registrations, local configuration, CI secrets, and verification. Ordinary builds work without Firebase configuration.
