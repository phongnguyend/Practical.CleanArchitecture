import 'dart:convert';

import 'package:flutter/material.dart';

import '../../shared/models/record.dart';
import '../../shared/widgets/json_tree.dart';

class AuditEntry extends StatelessWidget {
  const AuditEntry({super.key, required this.entry});
  final Json entry;

  String display(dynamic value) {
    if (value == null || value == '') {
      return 'Not provided';
    }
    if (value is Map || value is List) {
      return const JsonEncoder.withIndent('  ').convert(value);
    }
    return value.toString();
  }

  @override
  Widget build(BuildContext context) {
    dynamic data = entry['log'];
    if (data is String) {
      try {
        data = jsonDecode(data);
      } catch (_) {
        // Keep non-JSON events readable.
      }
    }
    final theme = Theme.of(context);
    final rawDate = entry['createdDateTime']?.toString() ?? '';
    final date = DateTime.tryParse(rawDate)?.toLocal();
    final timestamp = date == null
        ? rawDate
        : '${MaterialLocalizations.of(context).formatMediumDate(date)} ${MaterialLocalizations.of(context).formatTimeOfDay(TimeOfDay.fromDateTime(date))}';
    final user = entry['userName']?.toString() ?? '';
    final action = entry['action']?.toString() ?? '';
    final actionType = action.trim().toUpperCase().split(RegExp(r'[_\s-]')).first;
    final (foreground, background) = switch (actionType) {
      'DELETE' ||
      'DELETED' => (const Color(0xffb91c1c), const Color(0xfffee2e2)),
      'UPDATE' ||
      'UPDATED' => (const Color(0xff9a3412), const Color(0xffffedd5)),
      'ADD' ||
      'ADDED' ||
      'CREATE' ||
      'CREATED' => (const Color(0xff166534), const Color(0xffdcfce7)),
      _ => (const Color(0xff334155), const Color(0xffe2e8f0)),
    };
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          spacing: 12,
          children: [
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: DecoratedBox(
                decoration: BoxDecoration(
                  color: background,
                  borderRadius: BorderRadius.circular(6),
                ),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 10,
                    vertical: 4,
                  ),
                  child: Text(
                    action.isEmpty ? 'Event' : action,
                    style: theme.textTheme.titleSmall?.copyWith(
                      color: foreground,
                    ),
                  ),
                ),
              ),
            ),
            Text(timestamp, style: theme.textTheme.bodySmall),
            Text('By ${user.isEmpty ? 'Unknown user' : user}'),
            ExpansionTile(
              tilePadding: EdgeInsets.zero,
              title: const Text('View event details'),
              children: [
                Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  spacing: 12,
                  children: [
                    if (data is Map)
                      for (final field in data.entries)
                        Column(
                          crossAxisAlignment: CrossAxisAlignment.stretch,
                          spacing: 6,
                          children: [
                            Text(
                              field.key.toString().replaceAllMapped(
                                RegExp(r'([a-z])([A-Z])'),
                                (m) => '${m[1]} ${m[2]}',
                              ),
                              style: theme.textTheme.labelMedium,
                            ),
                            SelectableText(display(field.value)),
                          ],
                        )
                    else
                      SelectableText(display(data)),
                    Text('Full data', style: theme.textTheme.labelMedium),
                    JsonViewer(value: data),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
