import 'action_label.dart';

import 'package:file_picker/file_picker.dart';
import 'package:flutter/material.dart';
import 'package:classified_ads_mobile/core/services/app_services.dart';

import 'dialog_shell.dart';

class ImportDialog extends StatefulWidget {
  const ImportDialog({
    super.key,
    required this.suffix,
    required this.submit,
    required this.services,
  });
  final String suffix;
  final Future<void> Function(PlatformFile) submit;
  final AppServices services;
  @override
  State<ImportDialog> createState() => _ImportState();
}

class _ImportState extends State<ImportDialog> {
  PlatformFile? file;
  bool busy = false;
  String error = '';
  @override
  Widget build(BuildContext context) {
    final suffix = widget.suffix;
    return DialogShell(
      title: 'Import ${suffix == '.csv' ? 'CSV' : 'Excel'}',
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('Choose a $suffix file, then confirm the import.'),
          TextButton(
            onPressed: busy
                ? null
                : () async {
                    try {
                      final selected = await widget.services.pick();
                      if (mounted) setState(() => file = selected);
                    } catch (e) {
                      if (mounted) setState(() => error = e.toString());
                    }
                  },
            child: const ActionLabel('Choose file'),
          ),
          Text(file?.name ?? 'No file selected'),
          if (error.isNotEmpty)
            Text(
              error,
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          if (busy) const LinearProgressIndicator(),
        ],
      ),
      footer: [
        TextButton(
          onPressed: busy ? null : () => Navigator.pop(context),
          child: const ActionLabel('Cancel'),
        ),
        FilledButton(
          onPressed: busy
              ? null
              : () async {
                  setState(() {
                    busy = true;
                    error = '';
                  });
                  try {
                    if (file == null ||
                        !file!.name.toLowerCase().endsWith(suffix)) {
                      throw FormatException('Choose a $suffix file.');
                    }
                    await widget.submit(file!);
                    if (context.mounted) {
                      Navigator.pop(context);
                      ScaffoldMessenger.of(context).showSnackBar(
                        const SnackBar(content: Text('Import processed')),
                      );
                    }
                  } catch (e) {
                    if (mounted) setState(() => error = e.toString());
                  } finally {
                    if (mounted) setState(() => busy = false);
                  }
                },
          child: const ActionLabel('Import'),
        ),
      ],
    );
  }
}
