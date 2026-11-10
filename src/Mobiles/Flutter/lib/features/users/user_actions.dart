import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/widgets/dialog_shell.dart';
import '../../shared/widgets/record_page.dart';
import 'model.dart';

class UserActions extends StatelessWidget {
  const UserActions({super.key, required this.value, required this.services});
  final User value;
  final AppServices services;
  Future<void> email(
    BuildContext context,
    String endpoint,
    String label,
  ) async {
    if (!await confirmAction(context, 'Send $label email to ${value.email}?')) {
      return;
    }
    try {
      await services.api(
        'users/${Uri.encodeComponent(value.id)}/$endpoint',
        method: 'POST',
        body: {'id': value.id},
      );
      if (context.mounted) {
        ScaffoldMessenger.of(context)
            .showSnackBar(const SnackBar(content: Text('Email requested')));
      }
    } catch (e) {
      if (context.mounted) {
        ScaffoldMessenger.of(context)
            .showSnackBar(SnackBar(content: Text(e.toString())));
      }
    }
  }

  @override
  Widget build(BuildContext context) => Wrap(
    children: [
      TextButton(
        onPressed: () => showDialog<void>(
          context: context,
          barrierDismissible: false,
          builder: (context) => PasswordDialog(user: value, services: services),
        ),
        child: const Text('Set password'),
      ),
      TextButton(
        onPressed: () => email(context, 'passwordresetemail', 'password reset'),
        child: const Text('Reset email'),
      ),
      TextButton(
        onPressed: () =>
            email(context, 'emailaddressconfirmation', 'confirmation'),
        child: const Text('Confirm email'),
      ),
    ],
  );
}

String? validatePassword(String password, String confirmation) =>
    password.trim().isEmpty
    ? 'New password is required.'
    : password != confirmation
    ? 'Passwords do not match.'
    : null;

class PasswordDialog extends StatefulWidget {
  const PasswordDialog({super.key, required this.user, required this.services});
  final User user;
  final AppServices services;
  @override
  State<PasswordDialog> createState() => _PasswordState();
}

class _PasswordState extends State<PasswordDialog> {
  String password = '', confirmation = '', error = '';
  bool busy = false;
  Future<void> close() async {
    if (busy) return;
    if ((password.isEmpty && confirmation.isEmpty) ||
        await confirmAction(context, 'Discard unsaved changes?')) {
      if (mounted) Navigator.pop(context);
    }
  }

  @override
  Widget build(BuildContext context) => PopScope(
    canPop: false,
    onPopInvokedWithResult: (didPop, _) {
      if (!didPop) close();
    },
    child: DialogShell(
      title: 'Set password',
      body: Column(
        children: [
          TextField(
            obscureText: true,
            enabled: !busy,
            decoration: const InputDecoration(labelText: 'New password'),
            onChanged: (value) => password = value,
          ),
          TextField(
            obscureText: true,
            enabled: !busy,
            decoration: const InputDecoration(labelText: 'Confirm password'),
            onChanged: (value) => confirmation = value,
          ),
          if (error.isNotEmpty) Text(error),
        ],
      ),
      footer: [
        TextButton(onPressed: busy ? null : close, child: const Text('Cancel')),
        FilledButton(
          onPressed: busy
              ? null
              : () async {
                  final message = validatePassword(password, confirmation);
                  if (message != null) {
                    setState(() => error = message);
                    return;
                  }
                  setState(() => busy = true);
                  try {
                    await widget.services.api(
                      'users/${Uri.encodeComponent(widget.user.id)}/password',
                      method: 'PUT',
                      body: {'id': widget.user.id, 'password': password},
                    );
                    if (context.mounted) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Password updated')),
                      );
                      Navigator.pop(context);
                    }
                  } catch (e) {
                    if (mounted) setState(() => error = e.toString());
                  } finally {
                    if (mounted) setState(() => busy = false);
                  }
                },
          child: const Text('Save'),
        ),
      ],
    ),
  );
}
