import 'package:classified_ads_mobile/core/diagnostics/crash_reporting.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  test('unconfigured reporting does not initialize Firebase or need native channels', () async {
    await CrashReporting.initialize();
    await CrashReporting.record(
      StateError('synthetic failure'),
      StackTrace.current,
    );
    expect(Firebase.apps, isEmpty);
  });
}
