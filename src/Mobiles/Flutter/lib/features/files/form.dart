import '../../shared/widgets/action_label.dart';

import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import 'model.dart';

class StoredFileForm extends StatelessWidget {
  const StoredFileForm({
    super.key,
    required this.value,
    required this.changed,
    required this.services,
    required this.file,
    required this.pick,
    this.busy = false,
  });
  final StoredFile value;
  final VoidCallback changed;
  final AppServices services;
  final PlatformFile? file;
  final ValueChanged<PlatformFile?> pick;
  final bool busy;
  @override
  Widget build(BuildContext context) => Column(
    children: [
      TextFormField(
        initialValue: value.name.toString(),
        decoration: const InputDecoration(labelText: 'Name'),
        enabled: !(busy),
        onChanged: (text) {
          value.name = text;
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
        title: const Text('Encrypted'),
        value: value.encrypted,
        onChanged: busy || value.id.isNotEmpty
            ? null
            : (text) {
                value.encrypted = text;
                changed();
              },
      ),
      if (value.id.isNotEmpty)
        TextFormField(
          initialValue: value.fileName.toString(),
          decoration: const InputDecoration(labelText: 'File name'),
          enabled: !(busy || true),
          onChanged: (text) {
            value.fileName = text;
            changed();
          },
        ),
      if (value.id.isEmpty) ...[
        TextButton(
          onPressed: busy
              ? null
              : () async {
                  try {
                    pick(await services.pick());
                  } catch (e) {
                    if (context.mounted) {
                      ScaffoldMessenger.of(context)
                          .showSnackBar(SnackBar(content: Text(e.toString())));
                    }
                  }
                },
          child: const ActionLabel('Choose file'),
        ),
        Text(file?.name ?? 'No file selected'),
      ],
    ],
  );
}
