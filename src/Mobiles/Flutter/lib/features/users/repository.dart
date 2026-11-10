import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/repository.dart';
import 'model.dart';

class UserRepository implements RecordRepository<User> {
  UserRepository(this.api);
  final AppServices api;
  @override
  Future<List<User>> list() async => (await api.api('users') as List)
      .map((json) => User.fromJson(Map<String, dynamic>.from(json)))
      .toList();
  @override
  Future<User> get(String id) async => User.fromJson(
    Map<String, dynamic>.from(
      await api.api('users/${Uri.encodeComponent(id)}'),
    ),
  );
  @override
  Future<void> remove(String id) async {
    await api.api('users/${Uri.encodeComponent(id)}', method: 'DELETE');
  }

  @override
  Future<void> save(User value, PlatformFile? file) async {
    await api.api(
      'users${value.id.isEmpty ? '' : '/${Uri.encodeComponent(value.id)}'}',
      method: value.id.isEmpty ? 'POST' : 'PUT',
      body: value.toPayload(),
    );
  }

  Future<List<dynamic>> history(String id) async =>
      await api.api('users/${Uri.encodeComponent(id)}/auditlogs') as List;
}
