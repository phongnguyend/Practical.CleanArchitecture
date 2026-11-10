import 'package:flutter/material.dart';

import '../../shared/widgets/form_controls.dart';
import 'model.dart';

class StoredFileSummary extends StatelessWidget {
  const StoredFileSummary(this.value, {super.key, this.changed = const {}});
  final StoredFile value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue(
        'File name',
        value.fileName,
        changed: changed['fileName'] == true,
      ),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'Size',
        '${value.size} bytes',
        changed: changed['size'] == true,
      ),
      RecordValue(
        'Uploaded time',
        localDate(value.uploadedTime),
        changed: changed['uploadedTime'] == true,
      ),
    ],
  );
}

class StoredFileDetails extends StatelessWidget {
  const StoredFileDetails(this.value, {super.key, this.changed = const {}});
  final StoredFile value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue('Name', value.name, changed: changed['name'] == true),
      RecordValue(
        'File name',
        value.fileName,
        changed: changed['fileName'] == true,
      ),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'Size',
        '${value.size} bytes',
        changed: changed['size'] == true,
      ),
      RecordValue(
        'Uploaded time',
        localDate(value.uploadedTime),
        changed: changed['uploadedTime'] == true,
      ),
      RecordValue(
        'Encrypted',
        value.encrypted ? 'Yes' : 'No',
        changed: changed['encrypted'] == true,
      ),
      RecordValue(
        'File location',
        value.fileLocation,
        changed: changed['fileLocation'] == true,
      ),
    ],
  );
}

class StoredFileHistory extends StatelessWidget {
  const StoredFileHistory(this.value, {super.key, this.changed = const {}});
  final StoredFile value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue('Name', value.name, changed: changed['name'] == true),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'File name',
        value.fileName,
        changed: changed['fileName'] == true,
      ),
      RecordValue(
        'File location',
        value.fileLocation,
        changed: changed['fileLocation'] == true,
      ),
    ],
  );
}
