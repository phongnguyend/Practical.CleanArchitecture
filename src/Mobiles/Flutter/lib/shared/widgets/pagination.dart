import 'package:flutter/material.dart';

import 'action_label.dart';

class Pagination extends StatelessWidget {
  const Pagination({
    super.key,
    required this.page,
    required this.total,
    required this.pageSize,
    required this.onPage,
    this.busy = false,
  });
  final int page, total, pageSize;
  final bool busy;
  final ValueChanged<int> onPage;
  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.symmetric(vertical: 12),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text('Page $page | $total records', textAlign: TextAlign.center),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(
              child: TextButton(
                style: TextButton.styleFrom(minimumSize: const Size(0, 48)),
                onPressed: busy || page <= 1 ? null : () => onPage(page - 1),
                child: const ActionLabel('Previous'),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: TextButton(
                style: TextButton.styleFrom(minimumSize: const Size(0, 48)),
                onPressed: busy || page * pageSize >= total
                    ? null
                    : () => onPage(page + 1),
                child: const ActionLabel('Next'),
              ),
            ),
          ],
        ),
      ],
    ),
  );
}
