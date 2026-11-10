import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import 'model.dart';

class ProductForm extends StatelessWidget {
  const ProductForm({
    super.key,
    required this.value,
    required this.changed,
    required this.services,
    required this.file,
    required this.pick,
    this.busy = false,
  });
  final Product value;
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
        initialValue: value.code.toString(),
        decoration: const InputDecoration(labelText: 'Code'),
        enabled: !(busy),
        onChanged: (text) {
          value.code = text;
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
    ],
  );
}
