import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/repository.dart';
import 'model.dart';

class ConfigurationEntryRepository
    implements RecordRepository<ConfigurationEntry> {
  ConfigurationEntryRepository(this.api);
  final AppServices api;
  @override
  Future<List<ConfigurationEntry>> list() async =>
      (await api.api('configurationentries') as List)
          .map(
            (json) =>
                ConfigurationEntry.fromJson(Map<String, dynamic>.from(json)),
          )
          .toList();
  @override
  Future<ConfigurationEntry> get(String id) async =>
      ConfigurationEntry.fromJson(
        Map<String, dynamic>.from(
          await api.api('configurationentries/${Uri.encodeComponent(id)}'),
        ),
      );
  @override
  Future<void> remove(String id) async {
    await api.api(
      'configurationentries/${Uri.encodeComponent(id)}',
      method: 'DELETE',
    );
  }

  @override
  Future<void> save(ConfigurationEntry value, PlatformFile? file) async {
    await api.api(
      'configurationentries${value.id.isEmpty ? '' : '/${Uri.encodeComponent(value.id)}'}',
      method: value.id.isEmpty ? 'POST' : 'PUT',
      body: value.toPayload(),
    );
  }

  Future<void> importFile(PlatformFile file) async {
    await api.upload('configurationentries/ImportExcel', file);
  }
}
