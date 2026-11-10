import 'package:flutter/material.dart';

class RecordValue extends StatelessWidget {
  const RecordValue(this.label, this.value, {super.key, this.changed = false});
  final String label;
  final Object? value;
  final bool changed;
  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.symmetric(vertical: 6),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '$label${changed ? ' · Changed' : ''}',
          style: Theme.of(context).textTheme.labelMedium,
        ),
        SelectableText(
          value == null || value == '' ? '—' : value.toString(),
          style: TextStyle(
            color: changed ? Theme.of(context).colorScheme.error : null,
          ),
        ),
      ],
    ),
  );
}

class DateInput extends StatelessWidget {
  const DateInput({
    super.key,
    required this.label,
    required this.value,
    required this.changed,
    this.disabled = false,
  });
  final String label, value;
  final ValueChanged<String> changed;
  final bool disabled;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      Text(label),
      Text(
        value.isEmpty
            ? 'Not set'
            : DateTime.tryParse(value)?.toLocal().toString() ?? value,
      ),
      Wrap(
        children: [
          TextButton(
            onPressed: disabled
                ? null
                : () async {
                    final current =
                        DateTime.tryParse(value)?.toLocal() ?? DateTime.now();
                    final date = await showDatePicker(
                      context: context,
                      initialDate: current,
                      firstDate: DateTime(1900),
                      lastDate: DateTime(2200),
                    );
                    if (date == null || !context.mounted) return;
                    final time = await showTimePicker(
                      context: context,
                      initialTime: TimeOfDay.fromDateTime(current),
                    );
                    if (time != null) {
                      changed(
                        DateTime(
                          date.year,
                          date.month,
                          date.day,
                          time.hour,
                          time.minute,
                        ).toUtc().toIso8601String(),
                      );
                    }
                  },
            child: const Text('Choose date/time'),
          ),
          TextButton(
            onPressed: disabled ? null : () => changed(''),
            child: const Text('Clear'),
          ),
        ],
      ),
    ],
  );
}

String localDate(String value) =>
    DateTime.tryParse(value)?.toLocal().toString() ?? value;
