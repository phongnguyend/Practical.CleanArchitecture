import 'package:flutter_test/flutter_test.dart';
import 'package:sentry_flutter/sentry_flutter.dart';
import 'package:classified_ads_mobile/core/diagnostics/sentry_reporting.dart';

void main() {
  test(
    'unconfigured Sentry still starts the app without initializing a client',
    () async {
      var started = false;
      await SentryReporting.run(() {
        started = true;
      });
      expect(started, isTrue);
      expect(Sentry.isEnabled, isFalse);
    },
  );
  test(
    'event filter removes sensitive values while preserving stack information',
    () {
      final event = SentryEvent(
        user: SentryUser(email: 'private'),
        message: SentryMessage('private'),
        request: SentryRequest(data: 'secret'),
        exceptions: [
          SentryException(
            type: 'StateError',
            value: 'private',
            stackTrace: SentryStackTrace(
              frames: [SentryStackFrame(fileName: 'app.dart', lineNo: 42)],
            ),
          ),
        ],
      );
      final safe = SentryReporting.sanitize(event);
      expect(safe.toJson().toString(), isNot(contains('private')));
      expect(safe.toJson().toString(), isNot(contains('secret')));
      expect(safe.exceptions!.first.stackTrace!.frames.first.lineNo, 42);
    },
  );
}
