import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import 'notification_playgrounds.dart';

class AdminScreen extends StatefulWidget {
  const AdminScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<AdminScreen> createState() => _AdminScreenState();
}

class _AdminScreenState extends State<AdminScreen> {
  String page = 'admin';
  @override
  Widget build(BuildContext context) {
    if (page != 'admin' && page != 'playgrounds') {
      return NotificationPlaygrounds(
        key: ValueKey(page),
        services: widget.services,
        channel: page,
        back: () => setState(() => page = 'playgrounds'),
      );
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      spacing: 16,
      children: [
        Text(
          page == 'admin' ? 'Admin' : 'Playgrounds',
          style: Theme.of(context).textTheme.titleLarge,
        ),
        if (page == 'admin')
          OutlinedButton.icon(
            onPressed: () => setState(() => page = 'playgrounds'),
            icon: const Icon(Icons.settings_outlined),
            label: const Text('Playgrounds'),
          ),
        if (page == 'playgrounds') ...[
          OutlinedButton.icon(
            onPressed: () => setState(() => page = 'admin'),
            icon: const Icon(Icons.arrow_back),
            label: const Text('Back'),
          ),
          OutlinedButton.icon(
            onPressed: () => setState(() => page = 'mobilepush'),
            icon: const Icon(Icons.notifications_outlined),
            label: const Text('Mobile Push'),
          ),
          OutlinedButton.icon(
            onPressed: () => setState(() => page = 'email'),
            icon: const Icon(Icons.mail_outline),
            label: const Text('Email'),
          ),
          OutlinedButton.icon(
            onPressed: () => setState(() => page = 'sms'),
            icon: const Icon(Icons.sms_outlined),
            label: const Text('SMS'),
          ),
        ],
      ],
    );
  }
}
