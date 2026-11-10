import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/repository.dart';
import 'model.dart';

class StoredFileRepository implements RecordRepository<StoredFile> {
  StoredFileRepository(this.api);
  final AppServices api;
  @override
  Future<List<StoredFile>> list() async => (await api.api('files') as List)
      .map((json) => StoredFile.fromJson(Map<String, dynamic>.from(json)))
      .toList();
  @override
  Future<StoredFile> get(String id) async => StoredFile.fromJson(
    Map<String, dynamic>.from(
      await api.api('files/${Uri.encodeComponent(id)}'),
    ),
  );
  @override
  Future<void> remove(String id) async {
    await api.api('files/${Uri.encodeComponent(id)}', method: 'DELETE');
  }

  @override
  Future<void> save(StoredFile value, PlatformFile? file) async {
    if (value.id.isEmpty) {
      if (file == null) throw const FormatException('Choose a file.');
      await api.upload('files', file, value.toPayload());
      return;
    }
    await api.api(
      'files${value.id.isEmpty ? '' : '/${Uri.encodeComponent(value.id)}'}',
      method: value.id.isEmpty ? 'POST' : 'PUT',
      body: value.toPayload(),
    );
  }

  Future<List<dynamic>> history(String id) async =>
      await api.api('files/${Uri.encodeComponent(id)}/auditlogs') as List;
}
