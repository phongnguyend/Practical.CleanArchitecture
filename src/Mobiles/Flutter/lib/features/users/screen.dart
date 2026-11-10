import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/record.dart';
import '../../shared/widgets/record_page.dart';
import 'model.dart';
import 'repository.dart';
import 'form.dart';
import 'details.dart';
import 'user_actions.dart';

class UserScreen extends StatefulWidget {
  const UserScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<UserScreen> createState() => _UserScreenState();
}

class _UserScreenState extends State<UserScreen> {
  late final repository = UserRepository(widget.services);
  bool showImages = false;
  @override
  Widget build(BuildContext context) => RecordPage<User>(
    title: 'Users',
    repository: repository,
    empty: () => User(),
    copy: (value) => User.fromJson(value.toJson()),
    id: (value) => value.id,
    label: (value) => value.userName,
    searchText: (value) => value.toJson().toString(),
    serialize: (value) => value.toJson(),
    validate: (value) => value.validate(),
    form: (value, changed, file, pick, busy) => UserForm(
      value: value,
      changed: changed,
      file: file,
      pick: pick,
      busy: busy,
      services: widget.services,
    ),
    details: (value) => UserDetails(value),
    summary: (value) => UserSummary(value),
    history: repository.history,
    historyBody: (entry) => UserHistory(
      User.fromJson(auditSnapshot(entry)),
      changed: Map<String, dynamic>.from(entry['highLight'] ?? {}),
    ),
    actions: (value, run) => [
      UserActions(value: value, services: widget.services),
    ],
  );
}
