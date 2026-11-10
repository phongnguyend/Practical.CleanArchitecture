import 'dart:convert';
import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';

import 'app/app.dart';

void main() {
  const certificate = String.fromEnvironment('DEBUG_CERTIFICATE_BASE64');
  if (kDebugMode && certificate.isNotEmpty) {
    SecurityContext.defaultContext.setTrustedCertificatesBytes(
      base64Decode(certificate),
    );
  }
  runApp(const ClassifiedAdsApp());
}
