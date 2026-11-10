import 'dart:async';

import 'package:flutter/material.dart';

import '../core/services/app_services.dart';
import '../features/workspace/workspace_screen.dart';
import '../shared/widgets/app_theme.dart';

class ClassifiedAdsApp extends StatefulWidget {
  const ClassifiedAdsApp({super.key, this.services});
  final AppServices? services;
  @override
  State<ClassifiedAdsApp> createState() => _AppState();
}

class _AppState extends State<ClassifiedAdsApp> {
  late final AppServices services = widget.services ?? AppServices();
  ThemeMode mode = ThemeMode.system;
  @override
  void initState() {
    super.initState();
    services.readTheme().then((value) {
      if (mounted) {
        setState(
          () => mode = ThemeMode.values.firstWhere(
            (m) => m.name == value,
            orElse: () => ThemeMode.system,
          ),
        );
      }
    });
  }

  @override
  Widget build(BuildContext context) => MaterialApp(
    title: 'Classified Ads',
    debugShowCheckedModeBanner: false,
    themeMode: mode,
    theme: appTheme(Brightness.light),
    darkTheme: appTheme(Brightness.dark),
    home: Workspace(
      services: services,
      themeMode: mode,
      theme: (value) {
        setState(() => mode = value);
        unawaited(services.saveTheme(value.name));
      },
    ),
  );
}
