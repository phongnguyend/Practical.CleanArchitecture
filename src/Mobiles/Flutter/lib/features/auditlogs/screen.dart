import '../../shared/widgets/pagination.dart';
import '../../shared/widgets/action_label.dart';

import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/record.dart';
import 'audit_entry.dart';

class AuditLogsScreen extends StatefulWidget {
  const AuditLogsScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<AuditLogsScreen> createState() => _AuditLogsState();
}

class _AuditLogsState extends State<AuditLogsScreen> {
  List<Json> rows = [];
  int page = 1, total = 0;
  String search = '', error = '';
  bool busy = true;
  @override
  void initState() {
    super.initState();
    load();
  }

  Future<void> load() async {
    try {
      final data = await widget.services.api(
        'auditlogentries/paged?page=$page&pageSize=5',
      );
      if (mounted) {
        setState(() {
          rows = (data['items'] as List)
              .map((value) => Json.from(value))
              .toList();
          total = data['totalItems'] ?? 0;
          error = '';
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() => error = e.toString());
      }
    } finally {
      if (mounted) {
        setState(() => busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.stretch,
    spacing: 12,
    children: [
      Text('Audit logs', style: Theme.of(context).textTheme.headlineSmall),
      const SizedBox(height: 16),
      TextField(
        decoration: const InputDecoration(labelText: 'Search current records'),
        onChanged: (value) => setState(() => search = value),
      ),
      TextButton(
        onPressed: busy
            ? null
            : () {
                setState(() => busy = true);
                load();
              },
        child: const ActionLabel('Refresh'),
      ),
      if (busy) const LinearProgressIndicator(),
      if (error.isNotEmpty) Text(error),
      for (final row in rows.where(
        (row) => row.toString().toLowerCase().contains(search.toLowerCase()),
      ))
        AuditEntry(key: ValueKey(row['id']), entry: row),
      if (!busy &&
          error.isEmpty &&
          rows
              .where(
                (row) =>
                    row.toString().toLowerCase().contains(search.toLowerCase()),
              )
              .isEmpty)
        const Text('No audit logs found.'),
      Pagination(
        page: page,
        total: total,
        pageSize: 5,
        busy: busy,
        onPage: (value) {
          setState(() {
            page = value;
            busy = true;
          });
          load();
        },
      ),
    ],
  );
}
