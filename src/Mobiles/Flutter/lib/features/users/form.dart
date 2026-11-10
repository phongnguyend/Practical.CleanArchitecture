import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import '../../shared/widgets/form_controls.dart';
import 'model.dart';

class UserForm extends StatelessWidget {
  const UserForm({
    super.key,
    required this.value,
    required this.changed,
    required this.services,
    required this.file,
    required this.pick,
    this.busy = false,
  });
  final User value;
  final VoidCallback changed;
  final AppServices services;
  final PlatformFile? file;
  final ValueChanged<PlatformFile?> pick;
  final bool busy;
  @override
  Widget build(BuildContext context) => Column(
    children: [
      TextFormField(
        initialValue: value.userName.toString(),
        decoration: const InputDecoration(labelText: 'Username'),
        enabled: !(busy),
        onChanged: (text) {
          value.userName = text;
          changed();
        },
      ),
      TextFormField(
        initialValue: value.email.toString(),
        decoration: const InputDecoration(labelText: 'Email'),
        enabled: !(busy),
        keyboardType: TextInputType.emailAddress,
        onChanged: (text) {
          value.email = text;
          changed();
        },
      ),
      SwitchListTile(
        title: const Text('Email confirmed'),
        value: value.emailConfirmed,
        onChanged: busy
            ? null
            : (text) {
                value.emailConfirmed = text;
                changed();
              },
      ),
      TextFormField(
        initialValue: value.phoneNumber.toString(),
        decoration: const InputDecoration(labelText: 'Phone number'),
        enabled: !(busy),
        onChanged: (text) {
          value.phoneNumber = text;
          changed();
        },
      ),
      SwitchListTile(
        title: const Text('Phone confirmed'),
        value: value.phoneNumberConfirmed,
        onChanged: busy
            ? null
            : (text) {
                value.phoneNumberConfirmed = text;
                changed();
              },
      ),
      SwitchListTile(
        title: const Text('Two-factor enabled'),
        value: value.twoFactorEnabled,
        onChanged: busy
            ? null
            : (text) {
                value.twoFactorEnabled = text;
                changed();
              },
      ),
      SwitchListTile(
        title: const Text('Lockout enabled'),
        value: value.lockoutEnabled,
        onChanged: busy
            ? null
            : (text) {
                value.lockoutEnabled = text;
                changed();
              },
      ),
      DateInput(
        label: 'Lockout end (ISO 8601)',
        value: value.lockoutEnd,
        disabled: busy,
        changed: (text) {
          value.lockoutEnd = text;
          changed();
        },
      ),
      TextFormField(
        initialValue: value.accessFailedCount.toString(),
        decoration: const InputDecoration(labelText: 'Failed attempts'),
        enabled: !(busy),
        keyboardType: TextInputType.number,
        onChanged: (text) {
          value.accessFailedCount = int.tryParse(text) ?? -1;
          changed();
        },
      ),
    ],
  );
}
