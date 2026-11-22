import 'package:flutter/material.dart';

/// Equal-width actions with room for translated or enlarged labels.
class ActionGroup extends StatelessWidget {
  const ActionGroup({super.key, required this.children});

  final List<Widget> children;

  @override
  Widget build(BuildContext context) => LayoutBuilder(
    builder: (context, constraints) {
      final minimumWidth = 144 * MediaQuery.textScalerOf(context).scale(1);
      final columns = constraints.maxWidth >= minimumWidth * 2 + 12 ? 2 : 1;
      return Column(
        spacing: 12,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          for (var start = 0; start < children.length; start += columns)
            IntrinsicHeight(
              child: Row(
                spacing: 12,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  for (var column = 0; column < columns; column++)
                    Expanded(
                      child: start + column < children.length
                          ? children[start + column]
                          : const SizedBox.shrink(),
                    ),
                ],
              ),
            ),
        ],
      );
    },
  );
}
