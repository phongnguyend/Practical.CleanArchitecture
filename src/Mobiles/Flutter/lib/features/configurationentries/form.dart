import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import 'model.dart';

class ConfigurationEntryForm extends StatelessWidget {
  const ConfigurationEntryForm({
    super.key,
    required this.value,
    required this.changed,
    required this.services,
    required this.file,
    required this.pick,
    this.busy = false,
  });
  final ConfigurationEntry value;
  final VoidCallback changed;
  final AppServices services;
  final PlatformFile? file;
  final ValueChanged<PlatformFile?> pick;
  final bool busy;
  @override
  Widget build(BuildContext context) => Column(
    children: [
      TextFormField(
        initialValue: value.key.toString(),
        decoration: const InputDecoration(labelText: 'Key'),
        enabled: !(busy),
        onChanged: (text) {
          value.key = text;
          changed();
        },
      ),
      TextFormField(
        initialValue: value.value.toString(),
        decoration: const InputDecoration(labelText: 'Value'),
        enabled: !(busy),
        obscureText: value.isSensitive,
        onChanged: (text) {
          value.value = text;
          changed();
        },
      ),
      TextFormField(
        initialValue: value.description.toString(),
        decoration: const InputDecoration(labelText: 'Description'),
        enabled: !(busy),
        maxLines: 3,
        onChanged: (text) {
          value.description = text;
          changed();
        },
      ),
      SwitchListTile(
        title: const Text('Sensitive'),
        value: value.isSensitive,
        onChanged: busy
            ? null
            : (text) {
                value.isSensitive = text;
                changed();
              },
      ),
    ],
  );
}
