import 'dart:async';

import 'package:flutter/material.dart';

import '../../shared/widgets/action_label.dart';

import '../../core/services/app_services.dart';
import '../../shared/widgets/record_widgets.dart';
import '../../shared/widgets/record_page.dart';
import '../../shared/widgets/home_button.dart';
import '../products/screen.dart';
import '../files/screen.dart';
import '../users/screen.dart';
import '../roles/screen.dart';
import '../configurationentries/screen.dart';
import '../auditlogs/screen.dart';

const destinations = {
  'home': 'Home',
  'products': 'Products',
  'files': 'Files',
  'users': 'Users',
  'roles': 'Roles',
  'configurationentries': 'Configuration Entries',
  'auditlogentries': 'Audit logs',
  'notifications': 'Notifications',
};

class Workspace extends StatefulWidget {
  const Workspace({
    super.key,
    required this.services,
    required this.theme,
    this.themeMode = ThemeMode.system,
  });
  final AppServices services;
  final ValueChanged<ThemeMode> theme;
  final ThemeMode themeMode;
  @override
  State<Workspace> createState() => _WorkspaceState();
}

class _WorkspaceState extends State<Workspace> with WidgetsBindingObserver {
  String section = 'home', error = '', status = 'Connecting...';
  bool ready = false, busy = false;
  final List<String> feed = [];
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    initialize();
  }

  Future<void> initialize() async {
    try {
      await widget.services.restore();
      if (widget.services.signedIn) await connect();
    } catch (e) {
      error = e.toString();
    }
    if (mounted) setState(() => ready = true);
  }

  Future<void> connect() => widget.services.startNotifications(
    (message) {
      if (!mounted || !widget.services.signedIn) {
        return;
      }
      setState(() {
        feed.insert(0, message);
        if (feed.length > 100) feed.removeLast();
      });
      ScaffoldMessenger.of(context)
          .showSnackBar(SnackBar(content: Text(message)));
    },
    status: (value) {
      if (mounted) setState(() => status = value);
    },
  );
  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.paused && widget.services.lockSession()) {
      Navigator.of(context).popUntil((route) => route.isFirst);
      ScaffoldMessenger.of(context).hideCurrentSnackBar();
      setState(() {
        feed.clear();
        section = 'home';
      });
    }
    if (state == AppLifecycleState.resumed && widget.services.signedIn) {
      unawaited(connect());
    } else if (state != AppLifecycleState.resumed) {
      unawaited(widget.services.stopNotifications());
    }
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    unawaited(widget.services.stopNotifications());
    super.dispose();
  }

  Future<void> biometricAction(Future<void> Function() action) async {
    setState(() {
      busy = true;
      error = '';
    });
    try {
      await action();
      if (widget.services.signedIn) {
        await connect();
      }
    } catch (_) {
      error = 'Biometric authentication did not complete. Try again or sign in with Identity Server.';
    } finally {
      if (mounted) {
        setState(() => busy = false);
      }
    }
  }

  Future<void> authenticate({bool out = false}) async {
    setState(() {
      busy = true;
      error = '';
    });
    try {
      if (out) {
        await widget.services.stopNotifications();
        await widget.services.logout();
        feed.clear();
        section = 'home';
      } else {
        await widget.services.login();
        await connect();
      }
    } catch (e) {
      error = e.toString();
    }
    if (mounted) setState(() => busy = false);
  }

  Widget screen() => switch (section) {
    'products' => ProductScreen(services: widget.services),
    'files' => StoredFileScreen(services: widget.services),
    'users' => UserScreen(services: widget.services),
    'roles' => RoleScreen(services: widget.services),
    'configurationentries' => ConfigurationEntryScreen(
      services: widget.services,
    ),
    'auditlogentries' => AuditLogsScreen(services: widget.services),
    'notifications' => Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Notifications', style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 16),
        Text(status),
        if (feed.isEmpty)
          const Text('Live messages appear here while the app is active.'),
        for (final message in feed)
          Card(
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: Text(message),
            ),
          ),
      ],
    ),
    _ => Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      spacing: 12,
      children: [
        HomeInfo(openProject: widget.services.openProject),
        for (final entry in destinations.entries.where(
          (e) => !['home', 'notifications', 'auditlogentries'].contains(e.key),
        ))
          HomeButton(
            onPressed: () => setState(() => section = entry.key),
            label: entry.value,
          ),
        HomeButton(
          label: widget.services.biometricEnabled
              ? 'Disable biometric login'
              : 'Enable biometric login',
          onPressed: () => biometricAction(
            widget.services.biometricEnabled
                ? widget.services.disableBiometrics
                : widget.services.enableBiometrics,
          ),
        ),
        if (widget.services.biometricEnabled)
          HomeButton(
            label: 'Lock app',
            onPressed: () {
              widget.services.lockSession();
              setState(() {
                feed.clear();
                section = 'home';
              });
            },
          ),
        HomeButton(
          onPressed: () async {
            if (await confirmAction(context, 'Sign out of this device?')) {
              await authenticate(out: true);
            }
          },
          label: 'Sign out',
        ),
      ],
    ),
  };
  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(
      title: const Text(
        'CLASSIFIED ADS (Flutter)',
        style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
      ),
      actions: [
        PopupMenuButton<ThemeMode>(
          tooltip: 'Theme',
          initialValue: widget.themeMode,
          onSelected: widget.theme,
          itemBuilder: (context) => [
            for (final mode in ThemeMode.values)
              PopupMenuItem(value: mode, child: ActionLabel(mode.name)),
          ],
          icon: Image.asset(
            'assets/icons/action_theme.png',
            width: 20,
            height: 20,
            color: Theme.of(context).colorScheme.primary,
            excludeFromSemantics: true,
          ),
        ),
      ],
    ),
    body: SafeArea(
      child: Column(
        children: [
          if (busy) const LinearProgressIndicator(),
          if (error.isNotEmpty)
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              child: Material(
                color: Theme.of(context).colorScheme.errorContainer,
                borderRadius: BorderRadius.circular(10),
                child: Padding(
                  padding: const EdgeInsets.only(left: 12),
                  child: Row(
                    children: [
                      Expanded(
                        child: Semantics(
                          liveRegion: true,
                          child: Text(
                            error,
                            style: TextStyle(
                              fontSize: 14,
                              color: Theme.of(context)
                                  .colorScheme
                                  .onErrorContainer,
                            ),
                          ),
                        ),
                      ),
                      IconButton(
                        tooltip: 'Dismiss error',
                        onPressed: () => setState(() => error = ''),
                        icon: const Icon(Icons.close, size: 20),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          if (!ready)
            const CircularProgressIndicator()
          else if (!widget.services.signedIn)
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.fromLTRB(16, 16, 16, 32),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Text(
                      widget.services.biometricEnabled
                          ? 'Workspace locked'
                          : 'Your workspace, anywhere.',
                      style: Theme.of(context).textTheme.headlineSmall
                          ?.copyWith(fontSize: 24),
                    ),
                    const SizedBox(height: 16),
                    if (widget.services.biometricEnabled)
                      OutlinedButton(
                        onPressed: busy
                            ? null
                            : () => biometricAction(
                                widget.services.unlockSession,
                              ),
                        child: const ActionLabel('Unlock with biometrics'),
                      ),
                    const SizedBox(height: 12),
                    FilledButton(
                      onPressed: busy ? null : () => authenticate(),
                      child: const ActionLabel('Sign in'),
                    ),
                  ],
                ),
              ),
            )
          else ...[
            SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
              child: Row(
                children: [
                  for (final entry in destinations.entries)
                    Padding(
                      padding: const EdgeInsets.all(4),
                      child: ChoiceChip(
                        label: ActionLabel(
                          entry.value,
                          color: entry.key == section
                              ? Colors.white
                              : Theme.of(context).colorScheme.onSurface,
                        ),
                        selected: entry.key == section,
                        onSelected: (_) => setState(() => section = entry.key),
                      ),
                    ),
                ],
              ),
            ),
            Expanded(
              child: SingleChildScrollView(
                key: ValueKey(section),
                padding: const EdgeInsets.all(16),
                child: screen(),
              ),
            ),
          ],
        ],
      ),
    ),
  );
}
