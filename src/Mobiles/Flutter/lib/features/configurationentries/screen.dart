import '../../shared/widgets/action_label.dart';

import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/widgets/record_page.dart';
import '../../shared/widgets/import_dialog.dart';
import 'model.dart';
import 'repository.dart';
import 'form.dart';
import 'details.dart';

class ConfigurationEntryScreen extends StatefulWidget {
  const ConfigurationEntryScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<ConfigurationEntryScreen> createState() =>
      _ConfigurationEntryScreenState();
}

class _ConfigurationEntryScreenState extends State<ConfigurationEntryScreen> {
  late final repository = ConfigurationEntryRepository(widget.services);
  bool showImages = false;
  @override
  Widget build(BuildContext context) => RecordPage<ConfigurationEntry>(
    title: 'Configuration',
    repository: repository,
    empty: () => ConfigurationEntry(),
    copy: (value) => ConfigurationEntry.fromJson(value.toJson()),
    id: (value) => value.id,
    label: (value) => value.key,
    searchText: (value) => value.toJson().toString(),
    serialize: (value) => value.toJson(),
    validate: (value) => value.validate(),
    form: (value, changed, file, pick, busy) => ConfigurationEntryForm(
      value: value,
      changed: changed,
      file: file,
      pick: pick,
      busy: busy,
      services: widget.services,
    ),
    details: (value) => ConfigurationEntryDetails(value),
    summary: (value) => ConfigurationEntrySummary(value),
    toolbar: (run, reload) => [
      TextButton(
        onPressed: () async {
          final done = await showDialog<bool>(
            context: context,
            builder: (context) => ImportDialog(
              suffix: '.xlsx',
              submit: repository.importFile,
              services: widget.services,
            ),
          );
          if (done == true) {
            await reload();
            if (context.mounted) {
              ScaffoldMessenger.of(
                context,
              ).showSnackBar(const SnackBar(content: Text('Import processed')));
            }
          }
        },
        child: const ActionLabel('Import'),
      ),
      TextButton(
        onPressed: () => run(
          () => widget.services.download(
            'configurationentries/ExportAsExcel',
            'ConfigurationEntries.xlsx',
            Rect.fromLTWH(0, 0, MediaQuery.sizeOf(context).width, 1),
          ),
        ),
        child: const ActionLabel('Export XLSX'),
      ),
    ],
  );
}
