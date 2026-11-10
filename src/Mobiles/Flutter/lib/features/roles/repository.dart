import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/repository.dart';
import 'model.dart';

class RoleRepository implements RecordRepository<Role> {
  RoleRepository(this.api);
  final AppServices api;
  @override
  Future<List<Role>> list() async => (await api.api('roles') as List)
      .map((json) => Role.fromJson(Map<String, dynamic>.from(json)))
      .toList();
  @override
  Future<Role> get(String id) async => Role.fromJson(
    Map<String, dynamic>.from(
      await api.api('roles/${Uri.encodeComponent(id)}'),
    ),
  );
  @override
  Future<void> remove(String id) async {
    await api.api('roles/${Uri.encodeComponent(id)}', method: 'DELETE');
  }

  @override
  Future<void> save(Role value, PlatformFile? file) async {
    await api.api(
      'roles${value.id.isEmpty ? '' : '/${Uri.encodeComponent(value.id)}'}',
      method: value.id.isEmpty ? 'POST' : 'PUT',
      body: value.toPayload(),
    );
  }
}
