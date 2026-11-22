import 'dart:convert';
import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import 'app/app.dart';
import 'core/diagnostics/crash_reporting.dart';
import 'core/diagnostics/sentry_reporting.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  const certificate = String.fromEnvironment('DEBUG_CERTIFICATE_BASE64');
  if (kDebugMode && certificate.isNotEmpty) {
    SecurityContext.defaultContext.setTrustedCertificatesBytes(
      base64Decode(certificate),
    );
  }
  try {
    await CrashReporting.initialize();
  } catch (_) {
    debugPrint('Crash reporting could not be initialized.');
  }
  await SentryReporting.run(() => runApp(const ClassifiedAdsApp()));
}
