import '../../shared/models/record.dart';

class Role {
  String id;
  String name;
  String normalizedName;
  Role({this.id = '', this.name = '', this.normalizedName = ''});
  factory Role.fromJson(Json json) => Role(
    id: json['id']?.toString() ?? '',
    name: json['name']?.toString() ?? '',
    normalizedName: json['normalizedName']?.toString() ?? '',
  );
  Json toJson() => {'id': id, 'name': name, 'normalizedName': normalizedName};
  Json toPayload() => {if (id.isNotEmpty) 'id': id, 'name': name};
  String? validate() {
    if (name.trim().isEmpty) return 'Name is required.';
    return null;
  }
}
