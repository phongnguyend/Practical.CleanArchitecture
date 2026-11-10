import '../models/repository.dart';

import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:file_picker/file_picker.dart';

import '../models/record.dart';
import 'dialog_shell.dart';
import 'json_tree.dart';
import 'app_theme.dart';

typedef Operation = Future<void> Function(Future<void> Function());

class RecordPage<T> extends StatefulWidget {
  const RecordPage({
    super.key,
    required this.title,
    required this.repository,
    required this.empty,
    required this.copy,
    required this.id,
    required this.label,
    required this.searchText,
    required this.serialize,
    required this.validate,
    required this.form,
    required this.summary,
    required this.details,
    this.history,
    this.historyBody,
    this.toolbar,
    this.actions,
    this.createTitle = 'Add new',
  });
  final String title, createTitle;
  final RecordRepository<T> repository;
  final T Function() empty;
  final T Function(T) copy;
  final String Function(T) id, label, searchText;
  final Json Function(T) serialize;
  final String? Function(T) validate;
  final Widget Function(
    T,
    VoidCallback,
    PlatformFile?,
    ValueChanged<PlatformFile?>,
    bool,
  )
  form;
  final Widget Function(T) summary, details;
  final Future<List<dynamic>> Function(String)? history;
  final Widget Function(Json)? historyBody;
  final List<Widget> Function(Operation, Future<void> Function())? toolbar;
  final List<Widget> Function(T, Operation)? actions;
  @override
  State<RecordPage<T>> createState() => _RecordPageState<T>();
}

class _RecordPageState<T> extends State<RecordPage<T>> {
  List<T> rows = [];
  String search = '', error = '';
  int page = 1;
  bool busy = true;
  int generation = 0;
  @override
  void initState() {
    super.initState();
    load();
  }

  @override
  void dispose() {
    generation++;
    super.dispose();
  }

  Future<void> load() async {
    final ticket = ++generation;
    try {
      final result = await widget.repository.list();
      if (mounted && ticket == generation) setState(() => rows = result);
    } catch (e) {
      if (mounted && ticket == generation) setState(() => error = e.toString());
    } finally {
      if (mounted && ticket == generation) setState(() => busy = false);
    }
  }

  Future<void> run(Future<void> Function() action) async {
    if (busy) return;
    setState(() {
      busy = true;
      error = '';
    });
    try {
      await action();
    } catch (e) {
      if (mounted) setState(() => error = e.toString());
    } finally {
      if (mounted) setState(() => busy = false);
    }
  }

  void message(String text) {
    if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(text)));
    }
  }

  Future<void> edit(T value) async {
    final saved = await showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (context) => EntityEditor<T>(
        title: '${widget.id(value).isEmpty ? 'Add' : 'Edit'} ${widget.title}',
        value: widget.copy(value),
        serialize: widget.serialize,
        validate: widget.validate,
        form: widget.form,
        save: widget.repository.save,
      ),
    );
    if (saved == true) {
      await load();
      message('Saved successfully');
    }
  }

  Future<void> details(T value) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => DialogShell(
        title: 'Details',
        body: widget.details(value),
        footer: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Back'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Edit'),
          ),
        ],
      ),
    );
    if (result == true && mounted) await edit(value);
  }

  Future<void> history(T value) async {
    final entries = await widget.history!(widget.id(value));
    if (!mounted) return;
    await showDialog<void>(
      context: context,
      builder: (context) => DialogShell(
        title: 'Audit history',
        body: Column(
          children: [
            if (entries.isEmpty) const Text('No audit entries.'),
            for (final item in entries)
              Card(
                child: Padding(
                  padding: const EdgeInsets.all(12),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        '${item['createdDateTime']} · ${item['userName']} · ${item['action']}',
                      ),
                      if (widget.historyBody != null)
                        widget.historyBody!(Json.from(item)),
                      JsonViewer(value: auditSnapshot(Json.from(item))),
                    ],
                  ),
                ),
              ),
          ],
        ),
        footer: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Back'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final filtered = rows
        .where(
          (row) => widget
              .searchText(row)
              .toLowerCase()
              .contains(search.toLowerCase()),
        )
        .toList();
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(widget.title, style: Theme.of(context).textTheme.headlineSmall),
        const SizedBox(height: 16),
        TextField(
          decoration: const InputDecoration(
            labelText: 'Search current records',
          ),
          onChanged: (value) => setState(() {
            search = value;
            page = 1;
          }),
        ),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            TextButton(
              onPressed: busy ? null : () => run(load),
              child: const Text('Refresh'),
            ),
            FilledButton(
              onPressed: busy ? null : () => edit(widget.empty()),
              child: Text(widget.createTitle),
            ),
            ...?widget.toolbar?.call(run, load),
          ],
        ),
        if (busy) const LinearProgressIndicator(),
        if (error.isNotEmpty)
          Text(
            error,
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        if (filtered.isEmpty && !busy) const Text('No records found.'),
        for (final value in filtered.skip((page - 1) * 20).take(20))
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    widget.label(value),
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  widget.summary(value),
                  Wrap(
                    spacing: 8,
                    children: [
                      TextButton(
                        onPressed: busy
                            ? null
                            : () => run(
                                () async => details(
                                  await widget.repository.get(widget.id(value)),
                                ),
                              ),
                        child: const Text('View'),
                      ),
                      TextButton(
                        onPressed: busy
                            ? null
                            : () => run(
                                () async => edit(
                                  await widget.repository.get(widget.id(value)),
                                ),
                              ),
                        child: const Text('Edit'),
                      ),
                      TextButton(
                        style: dangerButtonStyle(context),
                        onPressed: busy
                            ? null
                            : () async {
                                final yes = await confirmAction(
                                  context,
                                  'Delete ${widget.label(value)}? This cannot be undone.',
                                  destructive: true,
                                );
                                if (yes) {
                                  await run(() async {
                                    await widget.repository.remove(
                                      widget.id(value),
                                    );
                                    await load();
                                    message('Deleted');
                                  });
                                }
                              },
                        child: const Text('Delete'),
                      ),
                      if (widget.history != null)
                        TextButton(
                          onPressed: busy
                              ? null
                              : () => run(() => history(value)),
                          child: const Text('History'),
                        ),
                      ...?widget.actions?.call(value, run),
                    ],
                  ),
                ],
              ),
            ),
          ),
        Wrap(
          spacing: 8,
          children: [
            if (page > 1)
              TextButton(
                onPressed: () => setState(() => page--),
                child: const Text('Previous'),
              ),
            Text('Page $page · ${filtered.length} records'),
            if (page * 20 < filtered.length)
              TextButton(
                onPressed: () => setState(() => page++),
                child: const Text('Next'),
              ),
          ],
        ),
      ],
    );
  }
}

Future<bool> confirmAction(
  BuildContext context,
  String title, {
  bool destructive = false,
}) async =>
    await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            style: destructive ? dangerButtonStyle(context) : null,
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Confirm'),
          ),
        ],
      ),
    ) ??
    false;

class EntityEditor<T> extends StatefulWidget {
  const EntityEditor({
    super.key,
    required this.title,
    required this.value,
    required this.serialize,
    required this.validate,
    required this.form,
    required this.save,
  });
  final String title;
  final T value;
  final Json Function(T) serialize;
  final String? Function(T) validate;
  final Widget Function(
    T,
    VoidCallback,
    PlatformFile?,
    ValueChanged<PlatformFile?>,
    bool,
  )
  form;
  final Future<void> Function(T, PlatformFile?) save;
  @override
  State<EntityEditor<T>> createState() => _EntityEditorState<T>();
}

class _EntityEditorState<T> extends State<EntityEditor<T>> {
  late final String original = jsonEncode(widget.serialize(widget.value));
  PlatformFile? file;
  bool busy = false;
  String error = '';
  Future<void> close() async {
    if (busy) return;
    if (file == null &&
        original == jsonEncode(widget.serialize(widget.value))) {
      Navigator.pop(context);
      return;
    }
    if (await confirmAction(context, 'Discard unsaved changes?') && mounted) {
      Navigator.pop(context);
    }
  }

  @override
  Widget build(BuildContext context) => PopScope(
    canPop: false,
    onPopInvokedWithResult: (didPop, _) {
      if (!didPop) close();
    },
    child: DialogShell(
      title: widget.title,
      body: Column(
        children: [
          widget.form(
            widget.value,
            () => setState(() {}),
            file,
            (value) => setState(() => file = value),
            busy,
          ),
          if (error.isNotEmpty)
            Text(
              error,
              style: TextStyle(color: Theme.of(context).colorScheme.error),
            ),
          if (busy) const LinearProgressIndicator(),
        ],
      ),
      footer: [
        TextButton(onPressed: busy ? null : close, child: const Text('Cancel')),
        FilledButton(
          onPressed: busy
              ? null
              : () async {
                  final message = widget.validate(widget.value);
                  if (message != null) {
                    setState(() => error = message);
                    return;
                  }
                  setState(() {
                    busy = true;
                    error = '';
                  });
                  try {
                    await widget.save(widget.value, file);
                    if (context.mounted) Navigator.pop(context, true);
                  } catch (e) {
                    if (mounted) setState(() => error = e.toString());
                  } finally {
                    if (mounted) setState(() => busy = false);
                  }
                },
          child: const Text('Save'),
        ),
      ],
    ),
  );
}
