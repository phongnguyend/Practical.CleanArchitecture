import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:classified_ads_mobile/core/services/app_services.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();
  const channel = MethodChannel('plugins.it_nomads.com/flutter_secure_storage');
  final values = <String, String>{};
  final calls = <MethodCall>[];
  late AppServices services;
  bool cancel = false;
  bool background = false;
  String session() => jsonEncode({
    'accessToken': 'test-access',
    'refreshToken': 'test-refresh',
    'expiresAt': DateTime.now().add(const Duration(hours: 1)).toIso8601String(),
  });
  setUp(() {
    debugDefaultTargetPlatformOverride = TargetPlatform.android;
    values.clear();
    calls.clear();
    cancel = false;
    background = false;
    services = AppServices();
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(
          const MethodChannel('classifiedads/biometrics'),
          (call) async {
            expect(call.method, 'clear');
            values.removeWhere((key, _) => key.startsWith('protected:'));
            return null;
          },
        );
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(channel, (call) async {
          calls.add(call);
          final args = call.arguments as Map;
          final options = args['options'] as Map;
          final protected =
              options['storageNamespace'] == 'classifiedads_biometric';
          final key = '${protected ? "protected:" : ""}${args['key']}';
          if (call.method == 'read' && protected) {
            if (cancel) {
              throw PlatformException(code: 'cancelled');
            }
            if (background) {
              services.lockSession();
            }
          }
          switch (call.method) {
            case 'read':
              return values[key];
            case 'write':
              values[key] = args['value'] as String;
              return null;
            case 'delete':
              values.remove(key);
              return null;
            default:
              throw StateError('Unexpected method ${call.method}');
          }
        });
  });
  tearDown(() {
    debugDefaultTargetPlatformOverride = null;
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(
          const MethodChannel('classifiedads/biometrics'),
          null,
        );
    TestDefaultBinaryMessengerBinding.instance.defaultBinaryMessenger
        .setMockMethodCallHandler(channel, null);
  });
  Future<void> enroll() async {
    values['session'] = session();
    await services.restore();
    await services.enableBiometrics();
  }

  test('enrollment moves session into biometric protected storage', () async {
    await enroll();
    expect(values.containsKey('session'), isFalse);
    expect(values['protected:session'], isNotNull);
    expect(services.biometricEnabled, isTrue);
    final options =
        calls
                .firstWhere(
                  (call) =>
                      call.method == 'write' &&
                      (call.arguments['options'] as Map)['storageNamespace'] ==
                          'classifiedads_biometric',
                )
                .arguments['options']
            as Map;
    expect(options['enforceBiometrics'], 'true');
    expect(options['requireBiometricsPerOperation'], 'true');
    expect(options['migrateOnAlgorithmChange'], 'false');
  });
  test('restore cannot read a session before biometrics', () async {
    values['biometric_enabled'] = 'true';
    values['protected:session'] = session();
    await services.restore();
    expect(services.signedIn, isFalse);
    expect(calls.length, 1);
    await expectLater(services.accessToken(), throwsException);
  });
  test('cancelled biometric read remains locked', () async {
    await enroll();
    services.lockSession();
    cancel = true;
    await expectLater(services.unlockSession(), throwsException);
    expect(services.signedIn, isFalse);
    await expectLater(services.accessToken(), throwsException);
  });
  test('successful read unlocks a valid session', () async {
    await enroll();
    services.lockSession();
    await services.unlockSession();
    expect(await services.accessToken(), 'test-access');
  });
  test('background transition invalidates a pending unlock', () async {
    await enroll();
    services.lockSession();
    background = true;
    await expectLater(services.unlockSession(), throwsException);
    expect(services.signedIn, isFalse);
  });
  test('interrupted disable cannot write an unprotected session', () async {
    await enroll();
    background = true;
    await expectLater(services.disableBiometrics(), throwsException);
    expect(values.containsKey('session'), isFalse);
    expect(services.biometricEnabled, isTrue);
  });
  test('logout clears both stores and enrollment', () async {
    await enroll();
    cancel = true;
    await services.logout();
    expect(values, isEmpty);
    expect(services.signedIn, isFalse);
  });
}
