# Android UI automation

These are real-device tests of the installed Flutter, Kotlin and React Native apps, using AndroidX UI Automator. They interact with native accessibility elements without relying on screen coordinates or application test-mode bypasses. A small separate test-host APK lets the suite test all three apps without adding dependencies to production apps. Existing Flutter widget, Kotlin unit, and React Native rendered-screen tests remain the fastest first layer.

## Run

Start one Android emulator, install the apps, and run the API/Identity Server. Sign in once in each app; tests preserve stored sessions and never enter passwords or bypass certificate warnings. Configure localhost certificates as described in each app's README. For a React Native debug APK, keep Metro running on port 8081.

From the repository root (PowerShell):

```powershell
./src/Mobiles/tests/Run-UiTests.ps1 -App Flutter
./src/Mobiles/tests/Run-UiTests.ps1 -App Kotlin
./src/Mobiles/tests/Run-UiTests.ps1 -App ReactNative
./src/Mobiles/tests/Run-UiTests.ps1 -App All

# Optional live API writes, using uniquely named products with cleanup:
./src/Mobiles/tests/Run-UiTests.ps1 -App All -IncludeCrud

# Run a single scenario when fixing a regression:
./src/Mobiles/tests/Run-UiTests.ps1 -App Kotlin -Feature Products -TestMethod productValidationAndPopupActionSpacing

# Run one feature across all apps:
./src/Mobiles/tests/Run-UiTests.ps1 -App All -Feature Users
```

The script locates the standard Android Studio SDK/JDK, forwards the local API/Identity/Metro ports, builds only the small test host, and returns a nonzero exit code for failures. It does not rebuild the three apps: rebuild/install the affected app after native or Flutter/Kotlin changes. React Native JavaScript changes can use Metro.

## Coverage

Each app has a JUnit suite (`FlutterUiTest`, `KotlinUiTest`, `ReactNativeUiTest`) composed of the same feature groups. Under `android/app/src/androidTest/java/com/classifiedads/mobile/uitests/`:

| Folder | Coverage |
| --- | --- |
| `features/home/` | Home layout, sign-out cancellation, theme/session persistence |
| `features/products/` | Search, unsaved changes, validation, optional CRUD/history |
| `features/users/` | User form validation |
| `features/roles/` | Role form validation |
| `features/configuration/` | Configuration form validation |
| `features/navigation/` | Resource lists, audit-log refresh and notification-hub connection |
| `support/` | App launch, selectors, waits, scrolling and diagnostic capture; no test cases |

Use `-Feature Home`, `Products`, `Users`, `Roles`, `Configuration`, or `Navigation` to select a group. The default `All` runs the entire app suite. `-TestMethod` requires a feature group.

The scenarios cover:

- Home buttons and the project link: consistent content width, minimum touch-target height, and vertical gaps.
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

Tests use visible text/content descriptions, accessibility text entry, bounded waits and fresh queries after recomposition. They intentionally fail when an app is signed out, missing, or unable to reach the API; these are not silently skipped. Only the opt-in CRUD case is skipped in the default run. Do not interact with the emulator while tests run.

Ordinary scenarios return home to dispose the previous feature's state. Cold launches remain explicit in session/theme persistence checks. This avoids downloading the React Native development bundle for every case. Empty accessibility text is normalized across frameworks; selectors are scoped to the target app so keyboard controls cannot be mistaken for app actions.

There are 13 scenarios per app (39 across all three). The default run executes 36 scenarios and skips the three optional CRUD cases. Form validation and discard tests do not save records or send account emails.

## Results and limitations

Verified on Medium_Phone (Android 17): the original 33 scenarios passed with `-App All -IncludeCrud`, including temporary-product cleanup in Flutter, Kotlin and React Native. After adding Audit logs and Notifications, all nine navigation checks passed with `-App All -Feature Navigation` (three checks per app, no skips).

`tests/artifacts/<timestamp>-<app>-<feature>/` contains the Gradle log, JUnit XML, HTML report, screenshots and accessibility trees. Artifacts are ignored by Git and may contain development record data. Screenshots aid diagnosis; the suite asserts control geometry and theme brightness, not pixel-identical rendering across frameworks.

This first suite covers Android, including Flutter and React Native on Android. It does not cover iOS, browser login itself, file pickers/transfers, import/export, password/email actions, notification delivery, or offline/expired-token handling. Never run optional CRUD against production data. Use disposable development backends/accounts.

Advanced: the independent Gradle project is `tests/android`; it reuses the repository's Kotlin Gradle wrapper. Standard Android Studio/Gradle `connectedDebugAndroidTest` execution and JUnit XML consumption work without the PowerShell wrapper.
