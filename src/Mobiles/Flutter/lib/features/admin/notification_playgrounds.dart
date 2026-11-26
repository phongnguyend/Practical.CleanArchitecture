import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';

class NotificationPlaygrounds extends StatefulWidget {
  const NotificationPlaygrounds({
    super.key,
    required this.services,
    required this.channel,
  });
  final AppServices services;
  final String channel;
  @override
  State<NotificationPlaygrounds> createState() =>
      _NotificationPlaygroundsState();
}

class _NotificationPlaygroundsState extends State<NotificationPlaygrounds> {
  final from = TextEditingController(),
      recipient = TextEditingController(),
      subject = TextEditingController(),
      body = TextEditingController();
  bool busy = false;
  String error = '', message = '', section = 'notifications';
  Map<String, dynamic>? info;
  bool get push => widget.channel == 'mobilepush';
  bool get email => widget.channel == 'email';
  String get name => push
      ? 'Mobile Push'
      : email
      ? 'Email'
      : 'SMS';
  String get path => push
      ? 'notifications/playground'
      : 'notifications/playgrounds/${widget.channel}';
  int get bodyLimit => push
      ? 500
      : email
      ? 10000
      : 1600;
  @override
  void initState() {
    super.initState();
    run(refresh);
  }

  @override
  void dispose() {
    from.dispose();
    recipient.dispose();
    subject.dispose();
    body.dispose();
    super.dispose();
  }

  Future<void> refresh() async {
    info = Map<String, dynamic>.from(await widget.services.api(path));
  }

  Future<void> run(Future<void> Function() action) async {
    setState(() {
      busy = true;
      error = '';
    });
    try {
      await action();
    } catch (_) {
      error = 'Unable to access the queue. Check the fields and try again.';
    } finally {
      if (mounted) {
        setState(() => busy = false);
      }
    }
  }

  void queue() {
    setState(() => message = '');
    if ((!push && recipient.text.trim().isEmpty) ||
        body.text.trim().isEmpty ||
        ((email || push) && subject.text.trim().isEmpty) ||
        (email && from.text.trim().isEmpty)) {
      setState(
        () => error = push
            ? 'Title and message are required.'
            : 'Complete all required fields.',
      );
      return;
    }
    final pattern = RegExp(r'^[^\s@;]+@[^\s@;]+\.[^\s@;]+$');
    if (!push &&
        (email
            ? !pattern.hasMatch(from.text.trim()) ||
                  !pattern.hasMatch(recipient.text.trim())
            : !RegExp(r'^\+[1-9][0-9]{7,14}$')
                  .hasMatch(recipient.text.trim()))) {
      setState(
        () => error = email ? 'Enter valid From and To email addresses.' : 'Enter a phone number in international format, for example +15551234567.',
      );
      return;
    }
    run(() async {
      await widget.services.api(
        path,
        method: 'POST',
        body: push
            ? {
                'title': subject.text.trim(),
                'body': body.text.trim(),
                'section': section,
              }
            : {
                'from': from.text.trim(),
                'recipient': recipient.text.trim(),
                'subject': subject.text.trim(),
                'body': body.text.trim(),
              },
      );
      message = 'Message queued successfully.';
      await refresh();
    });
  }

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    spacing: 16,
    children: [
      Text(name, style: Theme.of(context).textTheme.titleLarge),
      Text(
        push
            ? 'Queue a notification for your own registered devices.'
            : 'Queue a message for delivery by the existing $name worker.',
      ),
      if (info != null)
        Text(
          'Provider: ${info!["provider"]}. ${push ? "Registered devices: ${info!["deviceCount"]}. " : ""}${info!["provider"] == "Fake" ? "Delivery is simulated; no message is sent." : "Queued messages may be delivered to the specified recipient."}',
        ),
      if (email)
        TextField(
          controller: from,
          enabled: !busy,
          maxLength: 254,
          keyboardType: TextInputType.emailAddress,
          decoration: const InputDecoration(labelText: 'From'),
        ),
      if (!push)
        TextField(
          controller: recipient,
          enabled: !busy,
          maxLength: email ? 254 : 16,
          keyboardType: email
              ? TextInputType.emailAddress
              : TextInputType.phone,
          decoration: InputDecoration(
            labelText: email ? 'To' : 'Phone number',
            helperText: email
                ? null
                : 'International format, for example +15551234567.',
          ),
        ),
      if (email || push)
        TextField(
          controller: subject,
          enabled: !busy,
          maxLength: push ? 100 : 200,
          decoration: InputDecoration(labelText: push ? 'Title' : 'Subject'),
        ),
      TextField(
        controller: body,
        enabled: !busy,
        maxLength: bodyLimit,
        minLines: 3,
        maxLines: 6,
        decoration: const InputDecoration(labelText: 'Message'),
      ),
      if (push)
        DropdownButtonFormField<String>(
          initialValue: section,
          decoration: const InputDecoration(labelText: 'Destination'),
          items: [
            for (final value in [
              'notifications',
              'home',
              'products',
              'files',
              'users',
              'roles',
              'configurationentries',
              'auditlogentries',
            ])
              DropdownMenuItem(value: value, child: Text(value)),
          ],
          onChanged: busy ? null : (value) => setState(() => section = value!),
        ),
      FilledButton.icon(
        onPressed: busy || info == null ? null : queue,
        icon: const Icon(Icons.send_outlined),
        label: const Text('Queue message'),
      ),
      OutlinedButton.icon(
        onPressed: busy ? null : () => run(refresh),
        icon: const Icon(Icons.refresh),
        label: const Text('Refresh queue'),
      ),
      if (error.isNotEmpty)
        Text(
          error,
          style: TextStyle(color: Theme.of(context).colorScheme.error),
        ),
      if (message.isNotEmpty) Text(message),
      Text('Recent messages', style: Theme.of(context).textTheme.titleMedium),
      if (info != null && (info!['items'] as List).isEmpty)
        const Text('No queued messages yet.'),
      for (final item in info?['items'] ?? [])
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              spacing: 8,
              children: [
                Text(
                  item['title'],
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                if (!push) Text('To: ${item["recipient"]}'),
                Text(item['body']),
                Text(
                  push
                      ? 'Pending: ${item["pending"]} / Completed: ${item["completed"]} / Failed: ${item["failed"]}'
                      : '${item["status"]} / Attempts: ${item["attempts"]}',
                ),
              ],
            ),
          ),
        ),
    ],
  );
}
