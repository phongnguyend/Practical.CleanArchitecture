import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import 'model.dart';

class RoleForm extends StatelessWidget {
  const RoleForm({
    super.key,
    required this.value,
    required this.changed,
    required this.services,
    required this.file,
    required this.pick,
    this.busy = false,
  });
  final Role value;
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
    ],
  );
}
