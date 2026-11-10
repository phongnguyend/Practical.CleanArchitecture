import 'package:flutter/services.dart';

import 'dart:convert';

import 'package:flutter/material.dart';

class JsonTree extends StatefulWidget {
  const JsonTree({super.key, required this.value, this.name = 'root'});
  final dynamic value;
  final String name;
  @override
  State<JsonTree> createState() => _JsonTreeState();
}

class _JsonTreeState extends State<JsonTree> {
  int limit = 50;
  @override
  Widget build(BuildContext context) {
    final value = widget.value;
    final entries = value is Map
        ? value.entries.map((e) => MapEntry(e.key.toString(), e.value)).toList()
        : value is List
        ? value
              .asMap()
              .entries
              .map((e) => MapEntry(e.key.toString(), e.value))
              .toList()
        : null;
    if (entries == null) {
      return Padding(
        padding: const EdgeInsets.symmetric(vertical: 4),
        child: SelectableText(
          '${widget.name}: ${jsonEncode(value)}',
          style: TextStyle(
            fontFamily: 'monospace',
            color: Theme.of(context).colorScheme.primary,
          ),
        ),
      );
    }
    return ExpansionTile(
      initiallyExpanded: widget.name == 'root',
      title: Text('${widget.name} · ${entries.length}'),
      childrenPadding: const EdgeInsets.only(left: 16),
      children: [
        for (final e in entries.take(limit))
          JsonTree(value: e.value, name: e.key),
        if (entries.length > limit)
          TextButton(
            onPressed: () => setState(() => limit += 50),
            child: const Text('Show more'),
          ),
      ],
    );
  }
}

class JsonViewer extends StatefulWidget {
  const JsonViewer({super.key, required this.value});
  final dynamic value;
  @override
  State<JsonViewer> createState() => _JsonViewerState();
}

class _JsonViewerState extends State<JsonViewer> {
  bool open = false, copied = false;
  @override
  Widget build(BuildContext context) {
    dynamic parsed = widget.value;
    if (parsed is String) {
      try {
        parsed = jsonDecode(parsed);
      } catch (_) {}
    }
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Wrap(
          children: [
            TextButton(
              onPressed: () => setState(() => open = !open),
              child: Text(open ? 'Hide JSON' : 'Show JSON'),
            ),
            TextButton(
              onPressed: () async {
                try {
                  await Clipboard.setData(
                    ClipboardData(
                      text: widget.value is String
                          ? widget.value
                          : const JsonEncoder.withIndent('  ')
                                .convert(widget.value),
                    ),
                  );
                  if (mounted) setState(() => copied = true);
                } catch (e) {
                  if (context.mounted) {
                    ScaffoldMessenger.of(context)
                        .showSnackBar(SnackBar(content: Text(e.toString())));
                  }
                }
              },
              child: Text(copied ? 'Copied' : 'Copy JSON'),
            ),
          ],
        ),
        if (open) JsonTree(value: parsed),
      ],
    );
  }
}
