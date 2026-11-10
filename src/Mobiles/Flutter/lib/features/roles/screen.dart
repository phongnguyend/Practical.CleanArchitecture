import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/widgets/record_page.dart';
import 'model.dart';
import 'repository.dart';
import 'form.dart';
import 'details.dart';

class RoleScreen extends StatefulWidget {
  const RoleScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<RoleScreen> createState() => _RoleScreenState();
}

class _RoleScreenState extends State<RoleScreen> {
  late final repository = RoleRepository(widget.services);
  bool showImages = false;
  @override
  Widget build(BuildContext context) => RecordPage<Role>(
    title: 'Roles',
    repository: repository,
    empty: () => Role(),
    copy: (value) => Role.fromJson(value.toJson()),
    id: (value) => value.id,
    label: (value) => value.name,
    searchText: (value) => value.toJson().toString(),
    serialize: (value) => value.toJson(),
    validate: (value) => value.validate(),
    form: (value, changed, file, pick, busy) => RoleForm(
      value: value,
      changed: changed,
      file: file,
      pick: pick,
      busy: busy,
      services: widget.services,
    ),
    details: (value) => RoleDetails(value),
    summary: (value) => RoleSummary(value),
  );
}
