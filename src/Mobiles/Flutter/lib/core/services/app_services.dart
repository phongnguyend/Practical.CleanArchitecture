import 'dart:async';
import 'dart:convert';
import 'dart:io';
import 'dart:typed_data';
import 'dart:ui' show Rect;

import 'package:url_launcher/url_launcher.dart';

import 'package:file_picker/file_picker.dart';
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
  Json? _session;
  Future<String>? _refreshing;
  HubConnection? _hub;
  Timer? _retry;
  bool get signedIn => _session != null;
  Future<void> restore() async {
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
    _session = {
      'accessToken': access,
      'refreshToken': token.refreshToken ?? _session?['refreshToken'],
      'idToken': token.idToken ?? _session?['idToken'],
      'expiresAt':
          (token.accessTokenExpirationDateTime ??
                  DateTime.now().add(const Duration(minutes: 5)))
              .toIso8601String(),
    };
    await _storage.write(key: 'session', value: jsonEncode(_session));
    return access;
  }

  Future<void> login() async {
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
    if (_session == null) throw Exception('Please sign in.');
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
    _session = null;
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
