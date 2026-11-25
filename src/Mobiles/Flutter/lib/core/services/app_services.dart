import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';
import 'dart:ui' show Rect;

import 'package:flutter/foundation.dart'
    show defaultTargetPlatform, TargetPlatform;
import 'package:flutter/services.dart' show MethodChannel;

import 'package:url_launcher/url_launcher.dart';

import 'package:file_picker/file_picker.dart' hide AndroidOptions;
import 'package:flutter_appauth/flutter_appauth.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:http/http.dart' as http;
import 'package:path_provider/path_provider.dart';
import 'package:share_plus/share_plus.dart';
import 'package:signalr_netcore/signalr_client.dart';

import '../config/app_config.dart';
import '../../shared/models/record.dart';

class AppServices {
  Future<void> openProject() async {
    if (!await launchUrl(
      Uri.parse('https://github.com/phongnguyend/Practical.CleanArchitecture'),
      mode: LaunchMode.externalApplication,
    )) {
      throw Exception('Unable to open the project link.');
    }
  }

  Future<String> readTheme() async {
    try {
      return await _storage.read(key: 'theme') ?? 'system';
    } catch (_) {
      return 'system';
    }
  }

  Future<void> saveTheme(String value) async {
    try {
      await _storage.write(key: 'theme', value: value);
    } catch (_) {
      /* Retain the current in-memory appearance. */
    }
  }

  final _auth = const FlutterAppAuth();
  final _storage = const FlutterSecureStorage();
  final _biometricStorage = const FlutterSecureStorage(
    aOptions: AndroidOptions.biometric(
      enforceBiometrics: true,
      requireBiometricsPerOperation: true,
      biometricType: AndroidBiometricType.strongBiometricOnly,
      storageNamespace: 'classifiedads_biometric',
      // Never migrate through a cached unlocked cipher; initialize a fresh
      // per-operation store instead, including after biometric key invalidation.
      migrateOnAlgorithmChange: false,
      resetOnError: true,
      biometricPromptTitle: 'Unlock Classified Ads',
    ),
    iOptions: IOSOptions(
      accountName: 'classifiedads.biometric',
      accessibility: KeychainAccessibility.passcode,
      accessControlFlags: [AccessControlFlag.biometryCurrentSet],
    ),
  );
  bool biometricEnabled = false;
  bool _prompting = false;
  int _generation = 0;
  Json? _session;

  Future<T> _protected<T>(Future<T> Function() operation) async {
    if (_prompting) {
      throw Exception('Biometric authentication is already in progress.');
    }
    _prompting = true;
    try {
      return await operation();
    } finally {
      _prompting = false;
    }
  }

  bool lockSession() {
    _generation++;
    if (!biometricEnabled) {
      return false;
    }
    _session = null;
    _refreshing = null;
    unawaited(stopNotifications());
    return true;
  }

  Future<void> enableBiometrics() async {
    if (_session?['refreshToken'] == null) {
      throw Exception('Sign in before enabling biometric login.');
    }
    final epoch = _generation;
    final value = jsonEncode(_session);
    await _protected(() async {
      await _biometricStorage.write(key: 'session', value: value);
      if (await _biometricStorage.read(key: 'session') != value) {
        throw Exception('Biometric enrollment did not complete.');
      }
    });
    if (epoch != _generation) {
      throw Exception('App was backgrounded. Enable biometric login again.');
    }
    await _storage.write(key: 'biometric_enabled', value: 'true');
    biometricEnabled = true;
    await _storage.delete(key: 'session');
    if (epoch != _generation) {
      lockSession();
      throw Exception('Session locked. Unlock again.');
    }
  }

  Future<void> disableBiometrics() async {
    if (_session == null) {
      throw Exception('Unlock before changing biometric settings.');
    }
    final epoch = _generation;
    final stored = await _protected(
      () => _biometricStorage.read(key: 'session'),
    );
    if (stored == null || epoch != _generation || _session == null) {
      throw Exception(
        'Session changed. Unlock before changing biometric settings.',
      );
    }
    await _storage.write(key: 'session', value: jsonEncode(_session));
    await _clearBiometricStorage();
    await _storage.delete(key: 'biometric_enabled');
    biometricEnabled = false;
  }

  Future<void> unlockSession() async {
    if (!biometricEnabled) {
      throw Exception('Biometric login is not enabled.');
    }
    final epoch = _generation;
    final value = await _protected(
      () => _biometricStorage.read(key: 'session'),
    );
    if (value == null || epoch != _generation) {
      throw Exception(
        'Biometric credentials are unavailable. Please sign in again.',
      );
    }
    _session = jsonDecode(value) as Json;
    try {
      await accessToken();
    } catch (_) {
      _session = null;
      rethrow;
    }
  }

  Future<void> _clearBiometricStorage() async {
    if (defaultTargetPlatform == TargetPlatform.android) {
      // Deletion must work even after enrollment changes or on devices without biometrics.
      await const MethodChannel('classifiedads/biometrics')
          .invokeMethod<void>('clear');
    } else {
      await _biometricStorage.delete(key: 'session');
    }
  }

  Future<void> _forgetBiometrics() async {
    _generation++;
    _session = null;
    await _clearBiometricStorage();
    await _storage.delete(key: 'biometric_enabled');
    biometricEnabled = false;
  }

  Future<String>? _refreshing;
  HubConnection? _hub;
  Timer? _retry;
  bool get signedIn => _session != null;
  Future<void> restore() async {
    _session = null;
    biometricEnabled = await _storage.read(key: 'biometric_enabled') == 'true';
    if (biometricEnabled) {
      return;
    }
    try {
      final saved = await _storage.read(key: 'session');
      _session = saved == null ? null : jsonDecode(saved) as Json;
    } catch (_) {
      _session = null;
      await _storage.delete(key: 'session');
    }
  }

  Future<String> _save(TokenResponse token) async {
    final access = token.accessToken;
    if (access == null || access.isEmpty) {
      throw Exception('The identity provider did not return an access token.');
    }
    final epoch = _generation;
    final next = {
      'accessToken': access,
      'refreshToken': token.refreshToken ?? _session?['refreshToken'],
      'idToken': token.idToken ?? _session?['idToken'],
      'expiresAt':
          (token.accessTokenExpirationDateTime ??
                  DateTime.now().add(const Duration(minutes: 5)))
              .toIso8601String(),
    };
    if (biometricEnabled) {
      await _protected(
        () => _biometricStorage.write(key: 'session', value: jsonEncode(next)),
      );
    } else {
      await _storage.write(key: 'session', value: jsonEncode(next));
    }
    if (epoch != _generation) {
      throw Exception('Session locked. Please unlock again.');
    }
    _session = next;
    return access;
  }

  Future<void> login() async {
    await _forgetBiometrics();
    await _storage.delete(key: 'session');
    final token = await _auth.authorizeAndExchangeCode(
      AuthorizationTokenRequest(
        AppConfig.clientId,
        AppConfig.redirect,
        issuer: AppConfig.authority,
        scopes: ['openid', 'profile', 'offline_access', 'ClassifiedAds.WebAPI'],
      ),
    );
    await _save(token);
  }

  Future<String> accessToken() async {
    if (_session == null) {
      throw Exception('Please sign in.');
    }
    if (DateTime.parse(_session!['expiresAt'] as String)
        .isAfter(DateTime.now().add(const Duration(minutes: 1)))) {
      return _session!['accessToken'] as String;
    }
    if (_session!['refreshToken'] == null) {
      throw Exception('Session expired. Please sign out and sign in again.');
    }
    return _refreshing ??= _refresh().whenComplete(() => _refreshing = null);
  }

  Future<String> _refresh() async {
    final previous = _session;
    final token = await _auth.token(
      TokenRequest(
        AppConfig.clientId,
        AppConfig.redirect,
        issuer: AppConfig.authority,
        refreshToken: previous!['refreshToken'] as String,
      ),
    );
    if (!identical(previous, _session)) {
      throw Exception('Session changed. Please sign in again.');
    }
    return _save(token);
  }

  Future<void> logout() async {
    await stopNotifications();
    final idToken = _session?['idToken'] as String?;
    await _forgetBiometrics();
    await _storage.delete(key: 'session');
    if (idToken != null) {
      await _auth.endSession(
        EndSessionRequest(
          issuer: AppConfig.authority,
          idTokenHint: idToken,
          postLogoutRedirectUrl: AppConfig.logout,
        ),
      );
    }
  }

  Future<Uint8List> _send(
    String path, {
    String method = 'GET',
    Json? body,
    PlatformFile? file,
  }) async {
    final uri = Uri.parse(
      '${AppConfig.api.replaceFirst(RegExp(r'/+$'), '')}/${path.replaceFirst(RegExp(r'^/+'), '')}',
    );
    final http.BaseRequest request;
    if (file != null) {
      final form = http.MultipartRequest(method, uri);
      form.fields.addAll(
        (body ?? {}).map((key, value) => MapEntry(key, value.toString())),
      );
      form.files.add(
        await http.MultipartFile.fromPath(
          'formFile',
          file.path!,
          filename: file.name,
        ),
      );
      request = form;
    } else {
      final message = http.Request(method, uri);
      if (body != null) {
        message.headers['Content-Type'] = 'application/json';
        message.body = jsonEncode(body);
      }
      request = message;
    }
    request.headers['Authorization'] = 'Bearer ${await accessToken()}';
    final client = http.Client();
    try {
      final response = await http.Response.fromStream(
        await client.send(request).timeout(const Duration(seconds: 60)),
      ).timeout(const Duration(seconds: 60));
      if (response.statusCode >= 400) {
        throw Exception(apiError(response.statusCode, response.body));
      }
      return response.bodyBytes;
    } finally {
      client.close();
    }
  }

  Future<dynamic> api(String path, {String method = 'GET', Json? body}) async {
    final bytes = await _send(path, method: method, body: body);
    return bytes.isEmpty ? null : jsonDecode(utf8.decode(bytes));
  }

  Future<PlatformFile?> pick() async =>
      (await FilePicker.pickFiles()).firstOrNull;
  Future<dynamic> upload(String path, PlatformFile file, [Json? body]) async {
    final bytes = await _send(path, method: 'POST', file: file, body: body);
    return bytes.isEmpty ? null : jsonDecode(utf8.decode(bytes));
  }

  Future<void> download(String path, String name, Rect origin) async {
    final bytes = await _send(path);
    final folder = Directory('${(await getTemporaryDirectory()).path}/exports');
    await folder.create(recursive: true);
    await for (final item in folder.list()) {
      if ((await item.stat()).modified.isBefore(
        DateTime.now().subtract(const Duration(days: 1)),
      )) {
        await item.delete();
      }
    }
    final file = File(
      '${folder.path}/${name.replaceAll(RegExp(r'[^a-zA-Z0-9._-]'), '_')}',
    );
    await file.writeAsBytes(bytes);
    await SharePlus.instance.share(
      ShareParams(files: [XFile(file.path)], sharePositionOrigin: origin),
    );
  }

  Future<void> startNotifications(
    void Function(String) received, {
    void Function(String)? status,
  }) async {
    await stopNotifications();
    if (!signedIn) return;
    final connection = HubConnectionBuilder()
        .withUrl(
          AppConfig.hub,
          options: HttpConnectionOptions(accessTokenFactory: accessToken),
        )
        .withAutomaticReconnect()
        .build();
    _hub = connection;
    connection.on('ReceiveMessage', (arguments) {
      if (identical(_hub, connection)) {
        final value = arguments?.firstOrNull;
        received(value is String ? value : jsonEncode(value));
      }
    });
    Future<void> start() async {
      try {
        if (identical(_hub, connection) &&
            connection.state == HubConnectionState.Disconnected) {
          {
            await connection.start();
            status?.call('Connected to NotificationHub');
          }
        }
      } catch (_) {
        status?.call('Cannot connect to NotificationHub. Retrying…');
      }
    }

    _retry = Timer.periodic(
      const Duration(seconds: 10),
      (_) => unawaited(start()),
    );
    await start();
  }

  Future<void> stopNotifications() async {
    _retry?.cancel();
    _retry = null;
    final connection = _hub;
    _hub = null;
    await connection?.stop();
  }
}
