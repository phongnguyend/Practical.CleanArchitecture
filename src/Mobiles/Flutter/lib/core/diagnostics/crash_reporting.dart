import 'dart:async';

import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_crashlytics/firebase_crashlytics.dart';
import 'package:flutter/foundation.dart';

class CrashReporting {
  static FirebaseCrashlytics? _client;

  static Future<void> initialize() async {
    if (!const bool.fromEnvironment('CRASHLYTICS_ENABLED') || kIsWeb) {
      return;
    }
    await Firebase.initializeApp(
      options: const FirebaseOptions(
        apiKey: String.fromEnvironment('FIREBASE_API_KEY'),
        appId: String.fromEnvironment('FIREBASE_APP_ID'),
        messagingSenderId: String.fromEnvironment('FIREBASE_SENDER_ID'),
        projectId: String.fromEnvironment('FIREBASE_PROJECT_ID'),
        iosBundleId: String.fromEnvironment('FIREBASE_BUNDLE_ID'),
      ),
    );
    final client = FirebaseCrashlytics.instance;
    await client.setCustomKey('framework', 'Flutter');
    await client.setCrashlyticsCollectionEnabled(true);
    _client = client;
    final previousFlutterHandler = FlutterError.onError;
    FlutterError.onError = (details) {
      previousFlutterHandler?.call(details);
      unawaited(
        record(
          details.exception,
          details.stack ?? StackTrace.current,
          fatal: true,
        ),
      );
    };
    final previousPlatformHandler = PlatformDispatcher.instance.onError;
    PlatformDispatcher.instance.onError = (error, stack) {
      unawaited(record(error, stack, fatal: true));
      return previousPlatformHandler?.call(error, stack) ?? true;
    };
  }

  // Unexpected failures only. Never send exception messages or API bodies.
  static Future<void> record(
    Object error,
    StackTrace stack, {
    bool fatal = false,
  }) async {
    await _client?.recordError(
      error.runtimeType.toString(),
      stack,
      fatal: fatal,
    );
  }
}
