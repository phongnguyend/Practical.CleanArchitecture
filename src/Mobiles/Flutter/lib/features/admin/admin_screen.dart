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

  Widget tab(String value, String label, IconData icon, {bool? selected}) {
    final active = selected ?? page == value;
    final colors = Theme.of(context).colorScheme;
    return Expanded(
      child: Semantics(
        selected: active,
        button: true,
        child: InkWell(
          onTap: () => setState(() => page = value),
          child: Container(
            constraints: const BoxConstraints(minHeight: 48),
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
            decoration: BoxDecoration(
              border: Border(
                bottom: BorderSide(
                  color: active ? colors.primary : colors.outlineVariant,
                  width: active ? 3 : 1,
                ),
              ),
            ),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(
                  icon,
                  size: 18,
                  color: active ? colors.primary : colors.onSurfaceVariant,
                ),
                const SizedBox(height: 4),
                Text(
                  label,
                  textAlign: TextAlign.center,
                  style: TextStyle(
                    color: active ? colors.primary : colors.onSurface,
                    fontWeight: active ? FontWeight.bold : FontWeight.normal,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      spacing: 16,
      children: [
        Row(
          children: [
            tab(
              'playgrounds',
              'Playgrounds',
              Icons.science_outlined,
              selected: page != 'admin',
            ),
          ],
        ),
        if (page != 'admin')
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              tab('mobilepush', 'Mobile Push', Icons.notifications_outlined),
              tab('email', 'Email', Icons.mail_outline),
              tab('sms', 'SMS', Icons.sms_outlined),
            ],
          ),
        if (page == 'admin' || page == 'playgrounds') ...[
          Text(
            page == 'admin' ? 'Admin' : 'Playgrounds',
            style: Theme.of(context).textTheme.titleLarge,
          ),
          const Text(
            'Choose a playground to inspect its queue and compose a message.',
          ),
        ] else
          NotificationPlaygrounds(
            key: ValueKey(page),
            services: widget.services,
            channel: page,
          ),
      ],
    );
  }
}
