import 'package:flutter/material.dart';

import '../../shared/widgets/form_controls.dart';
import 'model.dart';

class ConfigurationEntrySummary extends StatelessWidget {
  const ConfigurationEntrySummary(
    this.value, {
    super.key,
    this.changed = const {},
  });
  final ConfigurationEntry value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue(
        'Value',
        value.isSensitive ? '******' : value.value,
        changed: changed['value'] == true,
      ),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'Updated time',
        localDate(value.updatedDateTime),
        changed: changed['updatedDateTime'] == true,
      ),
    ],
  );
}

class ConfigurationEntryDetails extends StatelessWidget {
  const ConfigurationEntryDetails(
    this.value, {
    super.key,
    this.changed = const {},
  });
  final ConfigurationEntry value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue('Key', value.key, changed: changed['key'] == true),
      RecordValue(
        'Value',
        value.isSensitive ? '******' : value.value,
        changed: changed['value'] == true,
      ),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'Updated time',
        localDate(value.updatedDateTime),
        changed: changed['updatedDateTime'] == true,
      ),
      RecordValue(
        'Sensitive',
        value.isSensitive ? 'Yes' : 'No',
        changed: changed['isSensitive'] == true,
      ),
    ],
  );
}

class ConfigurationEntryHistory extends StatelessWidget {
  const ConfigurationEntryHistory(
    this.value, {
    super.key,
    this.changed = const {},
  });
  final ConfigurationEntry value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) =>
      Column(crossAxisAlignment: CrossAxisAlignment.start, children: []);
}
