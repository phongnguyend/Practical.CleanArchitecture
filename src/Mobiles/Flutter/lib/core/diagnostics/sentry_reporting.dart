import 'package:flutter/foundation.dart';
import 'package:sentry_flutter/sentry_flutter.dart';

class SentryReporting {
  static Future<void> run(VoidCallback appRunner) async {
    if (const bool.fromEnvironment('SENTRY_ENABLED') &&
        const String.fromEnvironment('SENTRY_DSN').isNotEmpty &&
        kReleaseMode) {
      await SentryFlutter.init((options) {
        options.dsn = const String.fromEnvironment('SENTRY_DSN');
        options.environment = const String.fromEnvironment(
          'SENTRY_ENVIRONMENT',
          defaultValue: 'development',
        );
        const release = String.fromEnvironment('SENTRY_RELEASE');
        if (release.isNotEmpty) {
          options.release = release;
        }
        options.sendDefaultPii = false;
        options.maxBreadcrumbs = 0;
        options.tracesSampleRate = 0;
        options.beforeSend = (event, hint) => sanitize(event);
      }, appRunner: appRunner);
    } else {
      appRunner();
    }
  }

  static SentryEvent sanitize(SentryEvent event) {
    event.message = null;
    event.user = null;
    event.request = null;
    event.breadcrumbs?.clear();
    // ignore: deprecated_member_use
    event.extra?.clear();
    for (final error in event.exceptions ?? <SentryException>[]) {
      error.value = error.type ?? 'Unexpected error';
    }
    return event;
  }
}
