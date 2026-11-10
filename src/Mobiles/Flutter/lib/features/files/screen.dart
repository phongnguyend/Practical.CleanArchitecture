import '../../shared/widgets/action_label.dart';

import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/record.dart';
import '../../shared/widgets/record_page.dart';
import 'model.dart';
import 'repository.dart';
import 'form.dart';
import 'details.dart';

class StoredFileScreen extends StatefulWidget {
  const StoredFileScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<StoredFileScreen> createState() => _StoredFileScreenState();
}

class _StoredFileScreenState extends State<StoredFileScreen> {
  late final repository = StoredFileRepository(widget.services);
  bool showImages = false;
  @override
  Widget build(BuildContext context) => RecordPage<StoredFile>(
    title: 'Files',
    repository: repository,
    empty: () => StoredFile(),
    copy: (value) => StoredFile.fromJson(value.toJson()),
    id: (value) => value.id,
    label: (value) => value.name,
    searchText: (value) => value.toJson().toString(),
    serialize: (value) => value.toJson(),
    validate: (value) => value.validate(),
    form: (value, changed, file, pick, busy) => StoredFileForm(
      value: value,
      changed: changed,
      file: file,
      pick: pick,
      busy: busy,
      services: widget.services,
    ),
    details: (value) => StoredFileDetails(value),
    summary: (value) => StoredFileSummary(value),
    history: repository.history,
    historyBody: (entry) => StoredFileHistory(
      StoredFile.fromJson(auditSnapshot(entry)),
      changed: Map<String, dynamic>.from(entry['highLight'] ?? {}),
    ),
    createTitle: 'Upload file',
    actions: (value, run) => [
      TextButton(
        onPressed: () => run(
          () => widget.services.download(
            'files/${Uri.encodeComponent(value.id)}/download',
            value.fileName,
            Rect.fromLTWH(0, 0, MediaQuery.sizeOf(context).width, 1),
          ),
        ),
        child: const ActionLabel('Download'),
      ),
    ],
  );
}
