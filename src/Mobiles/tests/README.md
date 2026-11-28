# Android UI automation

These are real-device tests of the installed Flutter, Kotlin, React Native, MAUI and Capacitor apps, using AndroidX UI Automator. They interact with native accessibility elements without relying on screen coordinates or application test-mode bypasses. A small separate test-host APK lets the suite test all five apps without adding dependencies to production apps. Existing Flutter widget, Kotlin unit, and React Native rendered-screen tests remain the fastest first layer.

## Run

Start one Android emulator, install the apps, and run the API/Identity Server. Sign in once in each app; tests preserve stored sessions and never enter passwords or bypass certificate warnings. Configure localhost certificates as described in each app's README. For a React Native debug APK, keep Metro running on port 8081.

From the repository root (PowerShell):

```powershell
./src/Mobiles/tests/Run-UiTests.ps1 -App Flutter
./src/Mobiles/tests/Run-UiTests.ps1 -App Kotlin
./src/Mobiles/tests/Run-UiTests.ps1 -App ReactNative
./src/Mobiles/tests/Run-UiTests.ps1 -App MAUI
./src/Mobiles/tests/Run-UiTests.ps1 -App Capacitor
./src/Mobiles/tests/Run-UiTests.ps1 -App All

# Optional live API writes, using uniquely named products with cleanup:
./src/Mobiles/tests/Run-UiTests.ps1 -App All -IncludeCrud

# Run a single scenario when fixing a regression:
./src/Mobiles/tests/Run-UiTests.ps1 -App Kotlin -Feature Products -TestMethod productValidationAndPopupActionSpacing

# Run one feature across all apps:
./src/Mobiles/tests/Run-UiTests.ps1 -App All -Feature Users

# PDF export across all five apps (requires the HTML-to-PDF service):
./src/Mobiles/tests/Run-UiTests.ps1 -App All -Feature Products -TestMethod productPdfExportCreatesValidFileAndOpensShareSheet -IncludePdf
```

The opt-in PDF export check opens Products, taps Export PDF, verifies Android's share sheet shows `Products.pdf`, and checks that a fresh export has a `%PDF-` signature and `%%EOF` marker. It dismisses sharing without sending the file. Install debuggable APKs so the test can inspect each app's export cache using `run-as`; no authentication data is read. PDF tests are skipped unless `-IncludePdf` is supplied.

The script locates the standard Android Studio SDK/JDK, forwards the local API/Identity/Metro ports, builds only the small test host, and returns a nonzero exit code for failures. It does not rebuild the five apps: rebuild/install the affected app after native or Flutter/Kotlin changes. React Native JavaScript changes can use Metro.

## Coverage

Each app has a JUnit suite (`FlutterUiTest`, `KotlinUiTest`, `ReactNativeUiTest`, `MAUIUiTest`, `CapacitorUiTest`) composed of the same feature groups. Under `android/app/src/androidTest/java/com/classifiedads/mobile/uitests/`:

| Folder | Coverage |
| --- | --- |
| `features/home/` | App-specific title, home layout, sign-out cancellation, theme/session persistence |
| `features/products/` | Search, unsaved changes, validation, optional CRUD/history |
| `features/users/` | User form validation |
| `features/roles/` | Role form validation |
| `features/configuration/` | Configuration form validation |
| `features/navigation/` | Resource lists, audit-log refresh, event detail expansion/collapse and notification-hub connection |
| `support/` | App launch, selectors, waits, scrolling and diagnostic capture; no test cases |

Use `-Feature Home`, `Products`, `Users`, `Roles`, `Configuration`, or `Navigation` to select a group. The default `All` runs the entire app suite. `-TestMethod` requires a feature group.

The scenarios cover:

- App title: identifies the framework, stays within the screen bounds, and does not overlap the compact theme control.
- Home buttons and the project link: consistent content width, minimum touch-target height, and vertical gaps.
- Home shortcuts: open Audit logs and Notifications from their full-width home buttons, verify audit loading and notification-hub connection, then return Home.
- Product toolbar buttons: 48dp touch targets, screen bounds, and 12dp separation when arranged side by side or wrapped. Navigation labels must stay within a compact tab height.
- Products, files, users, roles and configuration: navigation, completed API loading, paging/status, and visible API errors.
- Audit logs: reach the off-screen navigation tab, load and refresh the page, check equal-width pagination buttons, 12dp spacing and 48dp touch targets, verify disabled Previous on page one, move forward/back when another page exists, and return Home.
- Notifications: reach the off-screen navigation tab, wait for the live hub connection and return Home. Message delivery is a separate scenario and is not covered by this navigation check.
- Product editor: required-name validation, Cancel/Save spacing, cancellation without saving.
- Product search: unmatched query, empty-state actions, refresh preserving the query, clearing the filter and returning to page one.
- Unsaved product draft: cancel discarding to keep the draft, confirm discarding, then verify a new editor is empty.
- Role, user and configuration editors: required name, username and key validation; invalid editors remain open and can be cancelled.
- Sign-out confirmation: cancel and verify the authenticated session survives relaunch and can still open products.
- Dark/light appearance and authenticated session persistence across force-stop/relaunch. The theme test restores System afterward.
- Optional CRUD: create a unique product, edit its description, verify update history, delete it and assert its disappearance. Cleanup only confirms deletion when the dialog names that exact fixture. Normal backend audit entries remain.

Tests use visible text/content descriptions, accessibility text entry, bounded waits and fresh queries after recomposition. They intentionally fail when an app is signed out, missing, or unable to reach the API; these are not silently skipped. The opt-in CRUD and PDF export cases are skipped in the default run. Do not interact with the emulator while tests run.

Ordinary scenarios return home to dispose the previous feature's state. Cold launches remain explicit in session/theme persistence checks. This avoids downloading the React Native development bundle for every case. Empty accessibility text is normalized across frameworks; selectors are scoped to the target app so keyboard controls cannot be mistaken for app actions.

The PDF export scenario adds five opt-in checks, one per app, to the existing feature suites. Both CRUD and PDF export require their corresponding opt-in switches. The `Home` group runs 21 checks across all five apps. Biometric scenarios are opt-in and excluded from these totals. Form validation and discard tests do not save records or send account emails. The audit event detail check requires at least one audit event in the backend.

## Results and limitations

On 2026-11-27, the product PDF export check passed for all five apps on Medium_Phone (Android 17), with no skips or failures. Each app opened the share sheet and produced a fresh PDF with valid header and end-of-file markers. Local report: `artifacts/20261127-162642-All-Products/reports/debug/index.html`.

On 2026-11-25, all 21 workspace/home checks passed across Flutter, Kotlin, React Native, MAUI and Capacitor on Medium_Phone (Android 17), with no skips or failures. This includes compact titles/theme controls, home action spacing, theme persistence and sign-out cancellation. Local report: `artifacts/20261125-112459-All-Home/reports/debug/index.html`. The separate biometric lifecycle scenario also passed for each of the five apps; iOS biometrics were not device-tested in this run.

Verified on Medium_Phone (Android 17): the original 33 scenarios passed with `-App All -IncludeCrud`, including temporary-product cleanup in Flutter, Kotlin and React Native. After adding Audit logs and Notifications, all nine navigation checks passed with `-App All -Feature Navigation` (three checks per app, no skips).

MAUI was verified on Medium_Phone (Android 17) on 2026-11-21: the default suite passed all 12 non-destructive scenarios, with the optional CRUD case skipped. A separate `-Feature Products -TestMethod productCreateUpdateHistoryDelete -IncludeCrud` run passed the remaining scenario, including temporary-product cleanup. Testing uncovered and fixed editable-field accessibility labels that were being included in field values. Local HTTPS used the optional localhost-only Debug certificate configuration documented in the MAUI README.

`tests/artifacts/<timestamp>-<app>-<feature>/` contains the Gradle log, JUnit XML, HTML report, screenshots and accessibility trees. Artifacts are ignored by Git and may contain development record data. Screenshots aid diagnosis; the suite asserts control geometry and theme brightness, not pixel-identical rendering across frameworks.

This suite covers Android, including Flutter and React Native on Android. It does not cover iOS, browser login itself, file pickers/uploads, imports or exports other than the opt-in product PDF export, password/email actions, notification delivery, or offline/expired-token handling. Never run optional CRUD against production data. Use disposable development backends/accounts.

Advanced: the independent Gradle project is `tests/android`; it reuses the repository's Kotlin Gradle wrapper. Standard Android Studio/Gradle `connectedDebugAndroidTest` execution and JUnit XML consumption work without the PowerShell wrapper.

Capacitor was verified on Medium_Phone (Android 17) on 2026-11-22 using `-App Capacitor -IncludeCrud`: all 13 shared scenarios passed, with no skips. The run includes theme/session persistence, form validation and spacing, discard protection, product CRUD/history/cleanup, resource lists, audit pagination, and notifications. Local report: `artifacts/20261122-123907-Capacitor-All/reports/debug/index.html`. Its separate browser suite also checks aligned action groups and preserved product searches.

## Biometric login

Enroll an Android emulator fingerprint and sign in normally in each app before running this opt-in group. The runner sends the enrolled fingerprint only when the test requests it; it does not change screen-lock or enrollment settings. Other feature groups do not enable biometrics.

```powershell
./Run-UiTests.ps1 -App All -Feature Biometrics -Fingerprint 1
```

This covers enabling protection, manual locking, cancelling and retrying unlock, background locking, cold-start locking, and disabling protection. A successful run leaves biometrics disabled so the normal authenticated suite can run. If a test fails after enrollment, unlock and disable biometric login before rerunning. The `-Fingerprint` number matches Extended controls (Finger 1 through Finger 10). The runner maps it to the emulator console ID; for fingerprints enrolled directly through the console, supply the raw ID with `-FingerprintId`.
# Mobile push playground coverage

The shared `Admin` feature checks the full-width **Home → Admin** shortcut, **Admin → Playgrounds → Mobile Push / Email / SMS**, required fields, queue refresh, and recent-message history for all five apps. It does not send a message. The `Notifications` feature checks the disabled-configuration explanation and inbox refresh. React Native and Flutter component tests also verify the three channels' payloads without contacting a delivery provider.

```powershell
./Run-UiTests.ps1 -App All -Feature Admin
./Run-UiTests.ps1 -App All -Feature Notifications
```

Install updated apps and start the updated API/Identity Server first. Apply the existing notification-context migration so the push queue tables exist, and sign in to each app. Actual device delivery requires separately configured Firebase/APNs credentials.
