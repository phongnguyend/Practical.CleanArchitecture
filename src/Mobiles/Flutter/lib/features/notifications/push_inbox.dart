import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../core/services/push_notifications.dart';

class PushInbox extends StatefulWidget {
  const PushInbox({
    super.key,
    required this.services,
    required this.push,
    required this.navigate,
  });
  final AppServices services;
  final PushNotifications push;
  final void Function(String) navigate;
  @override
  State<PushInbox> createState() => _PushInboxState();
}

class _PushInboxState extends State<PushInbox> {
  List<dynamic> items = [];
  int page = 1, total = 0;
  bool busy = false, testing = false;
  String error = '';
  @override
  void initState() {
    super.initState();
    run(() async {
      testing =
          (await widget.services.api(
            'notifications/status',
          ))['allowTestMessages'] ==
          true;
      await refresh();
    });
  }

  Future<void> run(Future<void> Function() action) async {
    setState(() {
      busy = true;
      error = '';
    });
    try {
      await action();
    } catch (e) {
      error = e.toString();
    } finally {
      if (mounted) {
        setState(() => busy = false);
      }
    }
  }

  Future<void> refresh() async {
    final result = await widget.services.api('notifications/inbox?page=$page');
    items = result['items'] as List<dynamic>;
    total = result['total'] as int;
  }

  Widget button(String label, IconData icon, Future<void> Function()? action) =>
      Padding(
        padding: const EdgeInsets.only(bottom: 10),
        child: OutlinedButton.icon(
          onPressed: busy || action == null ? null : () => run(action),
          icon: Icon(icon),
          label: Text(label),
        ),
      );

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    children: [
      Text('Notification inbox', style: Theme.of(context).textTheme.titleLarge),
      const SizedBox(height: 12),
      if (!pushConfigured)
        const Text('Push notifications are not configured for this build.'),
      button(
        'Enable notifications',
        Icons.notifications_active_outlined,
        pushConfigured ? widget.push.enable : null,
      ),
      button(
        'Disable notifications',
        Icons.notifications_off_outlined,
        pushConfigured ? widget.push.disable : null,
      ),
      if (testing)
        button('Send test notification', Icons.send_outlined, () async {
          await widget.services.api(
            'notifications/test',
            method: 'POST',
            body: {},
          );
          await refresh();
        }),
      button('Refresh inbox', Icons.refresh, refresh),
      if (error.isNotEmpty)
        Text(
          error,
          style: TextStyle(color: Theme.of(context).colorScheme.error),
        ),
      if (items.isEmpty) const Text('No saved notifications yet.'),
      for (final item in items)
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  item['title'] as String,
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const SizedBox(height: 8),
                Text(item['body'] as String),
                const SizedBox(height: 8),
                Text(
                  '${item['createdAt']} / ${item['readAt'] == null ? 'Unread' : 'Read'}',
                ),
                button('Open notification', Icons.open_in_new, () async {
                  await widget.services.api(
                    'notifications/inbox/${item['id']}/read',
                    method: 'PUT',
                    body: {},
                  );
                  widget.navigate(
                    pushSections.contains(item['section'])
                        ? item['section'] as String
                        : 'notifications',
                  );
                }),
              ],
            ),
          ),
        ),
      Text('Page $page / $total notifications'),
      const SizedBox(height: 10),
      Row(
        children: [
          Expanded(
            child: button(
              'Previous',
              Icons.arrow_back,
              page <= 1
                  ? null
                  : () async {
                      page--;
                      await refresh();
                    },
            ),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: button(
              'Next',
              Icons.arrow_forward,
              page * 20 >= total
                  ? null
                  : () async {
                      page++;
                      await refresh();
                    },
            ),
          ),
        ],
      ),
    ],
  );
}
