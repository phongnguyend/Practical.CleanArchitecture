import 'package:flutter/material.dart';

import '../../shared/widgets/form_controls.dart';
import 'model.dart';

class RoleSummary extends StatelessWidget {
  const RoleSummary(this.value, {super.key, this.changed = const {}});
  final Role value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) =>
      Column(crossAxisAlignment: CrossAxisAlignment.start, children: []);
}

class RoleDetails extends StatelessWidget {
  const RoleDetails(this.value, {super.key, this.changed = const {}});
  final Role value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue('Name', value.name, changed: changed['name'] == true),
      RecordValue(
        'Normalized name',
        value.normalizedName,
        changed: changed['normalizedName'] == true,
      ),
    ],
  );
}

class RoleHistory extends StatelessWidget {
  const RoleHistory(this.value, {super.key, this.changed = const {}});
  final Role value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) =>
      Column(crossAxisAlignment: CrossAxisAlignment.start, children: []);
}
