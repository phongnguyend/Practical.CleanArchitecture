import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/widgets/record_page.dart';

class Membership extends StatefulWidget {
  const Membership({
    super.key,
    required this.services,
    this.roleId,
    this.userId,
  });
  final AppServices services;
  final String? roleId;
  final String? userId;
  @override
  State<Membership> createState() => _MembershipState();
}

class _MembershipState extends State<Membership> {
  List<Map<String, dynamic>> items = [], users = [];
  String search = '', error = '';
  String? selected;
  bool busy = true;
  @override
  void initState() {
    super.initState();
    load();
  }

  Future<void> load() async {
    setState(() {
      busy = true;
      error = '';
    });
    try {
      final result = await widget.services.api(
        widget.roleId != null
            ? 'roles/${widget.roleId}/users'
            : 'users/${widget.userId}/roles',
      );
      final available = widget.roleId != null
          ? await widget.services.api('users')
          : [];
      if (mounted) {
        setState(() {
          items = List<Map<String, dynamic>>.from(
            (result as List).map((item) => Map<String, dynamic>.from(item)),
          );
          users = List<Map<String, dynamic>>.from(
            (available as List).map((item) => Map<String, dynamic>.from(item)),
          );
        });
      }
    } catch (_) {
      if (mounted) {
        setState(
          () => error = 'Unable to load role membership. Please try again.',
        );
      }
    } finally {
      if (mounted) {
        setState(() => busy = false);
      }
    }
  }

  Future<void> change(Map<String, dynamic> user, bool assign) async {
    if (busy) {
      return;
    }
    if (!assign &&
        !await confirmAction(
          context,
          'Remove ${user['userName']} from this role?',
        )) {
      return;
    }
    if (!mounted) {
      return;
    }
    setState(() {
      busy = true;
      error = '';
    });
    try {
      await widget.services.api(
        'roles/${widget.roleId}/users/${user['id']}',
        method: assign ? 'PUT' : 'DELETE',
      );
      if (mounted) {
        setState(() => selected = null);
        await load();
      }
    } catch (_) {
      if (mounted) {
        setState(
          () => error =
              'Unable to update role membership. Refresh and try again.',
        );
      }
    } finally {
      if (mounted) {
        setState(() => busy = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final candidates = users
        .where(
          (user) =>
              !items.any((item) => item['id'] == user['id']) &&
              '${user['userName']} ${user['email'] ?? ''}'
                  .toLowerCase()
                  .contains(search.toLowerCase()),
        )
        .toList();
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      spacing: 16,
      children: [
        const Divider(),
        Row(
          children: [
            Expanded(
              child: Text(
                widget.roleId != null ? 'Role users' : 'Assigned roles',
                style: Theme.of(context).textTheme.titleMedium,
              ),
            ),
            IconButton.outlined(
              onPressed: busy ? null : load,
              icon: const Icon(Icons.refresh),
              tooltip: widget.roleId != null
                  ? 'Refresh users'
                  : 'Refresh roles',
            ),
          ],
        ),
        if (busy) const Text('Loading...'),
        if (error.isNotEmpty)
          Text(
            error,
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        if (widget.roleId != null) ...[
          const Text(
            'Search by username or email to assign someone to this role.',
          ),
          TextField(
            decoration: const InputDecoration(labelText: 'Search users'),
            onChanged: (value) => setState(() {
              search = value;
              selected = null;
            }),
          ),
          DropdownButtonFormField<String>(
            key: ValueKey('$search:$selected:${items.length}'),
            initialValue: selected,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'User to assign'),
            items: candidates
                .map(
                  (user) => DropdownMenuItem(
                    value: user['id'] as String,
                    child: Text(
                      '${user['userName']}',
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                )
                .toList(),
            onChanged: busy
                ? null
                : (value) => setState(() => selected = value),
          ),
          FilledButton.icon(
            onPressed: busy || selected == null
                ? null
                : () => change(
                    candidates.firstWhere((user) => user['id'] == selected),
                    true,
                  ),
            icon: const Icon(Icons.person_add),
            label: const Text('Assign user'),
          ),
        ],
        if (!busy && error.isEmpty && items.isEmpty)
          Text(
            widget.roleId != null ? 'No users assigned.' : 'No roles assigned.',
          ),
        for (final item in items)
          Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            spacing: 12,
            children: [
              const Divider(),
              Text('${item['userName'] ?? item['name']}'),
              if (item['email'] != null) Text('${item['email']}'),
              if (widget.roleId != null)
                OutlinedButton.icon(
                  style: OutlinedButton.styleFrom(
                    foregroundColor: Theme.of(context).colorScheme.error,
                  ),
                  onPressed: busy ? null : () => change(item, false),
                  icon: const Icon(Icons.person_remove),
                  label: Text(
                    'Remove',
                    semanticsLabel: 'Remove ${item['userName']} from role',
                  ),
                ),
            ],
          ),
      ],
    );
  }
}
