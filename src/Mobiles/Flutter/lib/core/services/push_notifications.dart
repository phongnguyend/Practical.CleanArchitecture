import 'dart:async';
import 'dart:math';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter/services.dart';

import 'dart:io';

import 'app_services.dart';

const pushConfigured = bool.fromEnvironment('PUSH_NOTIFICATIONS_ENABLED');
const pushSections = {
  'home',
  'products',
  'files',
  'users',
  'roles',
  'configurationentries',
  'auditlogentries',
  'notifications',
};
String? pushSection(Map<String, dynamic> data) {
  if (!RegExp(r'^[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}$')
      .hasMatch('${data['notificationId']}')) {
    return null;
  }
  return pushSections.contains(data['section'])
      ? data['section'] as String
      : 'notifications';
}

class PushNotifications {
  PushNotifications(this.services);
  final AppServices services;
  final _storage = const FlutterSecureStorage();
  final List<StreamSubscription<dynamic>> _subscriptions = [];
  RemoteMessage? _pending;
  int _generation = 0;
  Future<void> initialize(
    void Function() opened,
    void Function() received,
  ) async {
    if (!pushConfigured) {
      return;
    }
    _subscriptions.add(
      FirebaseMessaging.onMessageOpenedApp.listen((message) {
        _pending = message;
        opened();
      }),
    );
    _subscriptions.add(
      FirebaseMessaging.onMessage.listen((_) {
        if (services.signedIn) {
          received();
        }
      }),
    );
    _subscriptions.add(
      FirebaseMessaging.instance.onRegistered.listen((token) {
        unawaited(_register(token).catchError((Object _) {}));
      }),
    );
    _pending = await FirebaseMessaging.instance.getInitialMessage();
    opened();
  }

  Future<void> _register(String token) async {
    if (!services.signedIn ||
        await _storage.read(key: 'push.enabled') != 'true') {
      return;
    }
    var id = await _storage.read(key: 'push.installation');
    if (id == null) {
      final random = Random.secure();
      final bytes = List.generate(16, (_) => random.nextInt(256));
      bytes[6] = (bytes[6] & 15) | 64;
      bytes[8] = (bytes[8] & 63) | 128;
      final hex = bytes.map((b) => b.toRadixString(16).padLeft(2, '0')).join();
      id =
          '${hex.substring(0, 8)}-${hex.substring(8, 12)}-${hex.substring(12, 16)}-${hex.substring(16, 20)}-${hex.substring(20)}';
      await _storage.write(key: 'push.installation', value: id);
    }
    await services.api(
      'notifications/devices/$id',
      method: 'PUT',
      body: {
        'app': 'Flutter',
        'platform': Platform.isIOS ? 'ios' : 'android',
        'token': token,
      },
    );
  }

  Future<void> sync() async {
    if (!pushConfigured ||
        !services.signedIn ||
        await _storage.read(key: 'push.enabled') != 'true') {
      return;
    }
    await FirebaseMessaging.instance.register();
  }

  Future<void> enable() async {
    if (!pushConfigured ||
        (await services.api('notifications/status'))['enabled'] != true) {
      throw Exception('Push notifications are not configured.');
    }
    final epoch = _generation;
    final permission = await FirebaseMessaging.instance.requestPermission();
    if (permission.authorizationStatus != AuthorizationStatus.authorized &&
        permission.authorizationStatus != AuthorizationStatus.provisional) {
      throw Exception(
        'Allow notifications in device settings to receive updates.',
      );
    }
    if (epoch != _generation || !services.signedIn) {
      throw Exception('Unlock your workspace before enabling notifications.');
    }
    if (Platform.isAndroid) {
      await const MethodChannel('classifiedads/push')
          .invokeMethod<void>('createChannel');
    }
    await FirebaseMessaging.instance.setAutoInitEnabled(true);
    await _storage.write(key: 'push.enabled', value: 'true');
    await sync();
  }

  Future<void> disable() async {
    _generation++;
    _pending = null;
    final id = await _storage.read(key: 'push.installation');
    await _storage.delete(key: 'push.enabled');
    try {
      if (id != null && services.signedIn) {
        await services.api('notifications/devices/$id', method: 'DELETE');
      }
    } finally {
      await _storage.delete(key: 'push.installation');
      if (pushConfigured) {
        await FirebaseMessaging.instance.setAutoInitEnabled(false);
        await FirebaseMessaging.instance.unregister();
      }
    }
  }

  Future<String?> consume() async {
    if (!services.signedIn || _pending == null) {
      return null;
    }
    final data = _pending!.data;
    _pending = null;
    final section = pushSection(data);
    if (section != null) {
      try {
        await services.api(
          'notifications/inbox/${data['notificationId']}/read',
          method: 'PUT',
          body: {},
        );
      } catch (_) {
        // The inbox can be refreshed after reconnecting.
      }
    }
    return section;
  }

  void dispose() {
    for (final subscription in _subscriptions) {
      unawaited(subscription.cancel());
    }
  }
}
