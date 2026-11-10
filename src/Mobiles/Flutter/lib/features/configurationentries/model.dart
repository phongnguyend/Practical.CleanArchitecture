import '../../shared/models/record.dart';

class ConfigurationEntry {
  String id;
  String key;
  String value;
  String description;
  bool isSensitive;
  String updatedDateTime;
  ConfigurationEntry({
    this.id = '',
    this.key = '',
    this.value = '',
    this.description = '',
    this.isSensitive = false,
    this.updatedDateTime = '',
  });
  factory ConfigurationEntry.fromJson(Json json) => ConfigurationEntry(
    id: json['id']?.toString() ?? '',
    key: json['key']?.toString() ?? '',
    value: json['value']?.toString() ?? '',
    description: json['description']?.toString() ?? '',
    isSensitive: json['isSensitive'] == true,
    updatedDateTime: json['updatedDateTime']?.toString() ?? '',
  );
  Json toJson() => {
    'id': id,
    'key': key,
    'value': value,
    'description': description,
    'isSensitive': isSensitive,
    'updatedDateTime': updatedDateTime,
  };
  Json toPayload() => {
    if (id.isNotEmpty) 'id': id,
    'key': key,
    'value': value,
    'description': description,
    'isSensitive': isSensitive,
  };
  String? validate() {
    if (key.trim().isEmpty) return 'Key is required.';
    if (value.trim().isEmpty) return 'Value is required.';
    return null;
  }
}
