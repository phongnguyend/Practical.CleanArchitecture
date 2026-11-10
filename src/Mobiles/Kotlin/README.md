# Kotlin / Jetpack Compose

See [mobile features and backend setup](../README.md). Open this folder in Android Studio with SDK 36 and JDK 17+ (validated with JDK 21). Set `ANDROID_HOME` or let Android Studio generate an untracked `local.properties`.

```sh
./gradlew :app:testDebugUnitTest
./gradlew :app:assembleDebug -PapiUrl=https://api.example.com/api/ -Pauthority=https://identity.example.com -PnotificationUrl=https://api.example.com/hubs/notification
./gradlew :app:installDebug -PapiUrl=https://api.example.com/api/ -Pauthority=https://identity.example.com -PnotificationUrl=https://api.example.com/hubs/notification
```

Windows: use `gradlew.bat`. Replace example hosts; omitted properties use localhost defaults. Android Studio users can set these properties in their user Gradle properties file.

For local HTTPS on an Android emulator, forward ports with `adb reverse tcp:44312 tcp:44312` and `adb reverse tcp:44367 tcp:44367`. Debug builds trust user-installed CA certificates only for `localhost`; release builds do not include this configuration. Keep the URLs as `localhost` so certificate hostname checks and the OIDC issuer continue to match.

For a self-signed .NET development certificate, export its public certificate (never the private key) and build with `-PdebugCertificatePath=C:/path/to/localhost-dev.cer`. This embeds that certificate in generated debug resources and trusts it only for `localhost`, without disabling TLS or hostname verification. Omit the property to return to system/user CA trust. The certificate is never added to release resources. The API and Identity Server must use the matching certificate.

The system login browser has its own trust store: app configuration does not fix browser certificate errors. For full native login, use a development CA installed through Android Settings and configure the servers with a localhost certificate signed by that CA, or use a publicly trusted HTTPS development hostname with matching issuer/client URLs. A .NET end-entity certificate may be rejected by Android's CA installer; do not treat it as a CA certificate.

APK: `app/build/outputs/apk/debug/`. Requires Android 8/API 26+. AppAuth uses `classifiedads.kotlin` callbacks. Tokens are encrypted with Android Keystore; FileProvider shares private cached downloads.

`MainActivity.kt` is the Android entry point. `features.workspace` handles navigation, sessions and notifications. Each resource package owns a typed data class, `Repository.kt`, explicit `Form.kt`, `Details.kt` and `Screen.kt`. `core.auth` contains session and token storage; `core.network` contains API transport. `shared.model` contains repository contracts and audit/error helpers; `shared.ui` contains reusable Compose controls and record-page lifecycle behavior without field schemas. Configure production signing before release.
