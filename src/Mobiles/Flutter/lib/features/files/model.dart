import '../../shared/models/record.dart';

class StoredFile {
  String id;
  String name;
  String description;
  bool encrypted;
  String fileName;
  int size;
  String uploadedTime;
  String fileLocation;
  StoredFile({
    this.id = '',
    this.name = '',
    this.description = '',
    this.encrypted = false,
    this.fileName = '',
    this.size = 0,
    this.uploadedTime = '',
    this.fileLocation = '',
  });
  factory StoredFile.fromJson(Json json) => StoredFile(
    id: json['id']?.toString() ?? '',
    name: json['name']?.toString() ?? '',
    description: json['description']?.toString() ?? '',
    encrypted: json['encrypted'] == true,
    fileName: json['fileName']?.toString() ?? '',
    size: (json['size'] as num?)?.toInt() ?? 0,
    uploadedTime: json['uploadedTime']?.toString() ?? '',
    fileLocation: json['fileLocation']?.toString() ?? '',
  );
  Json toJson() => {
    'id': id,
    'name': name,
    'description': description,
    'encrypted': encrypted,
    'fileName': fileName,
    'size': size,
    'uploadedTime': uploadedTime,
    'fileLocation': fileLocation,
  };
  Json toPayload() => {
    if (id.isNotEmpty) 'id': id,
    'name': name,
    'description': description,
    'encrypted': encrypted,
    if (id.isNotEmpty) 'fileName': fileName,
  };
  String? validate() {
    if (name.trim().isEmpty) return 'Name is required.';
    if (name.trim().length < 3) return 'Name needs at least 3 characters.';
    if (description.trim().isEmpty) return 'Description is required.';
    if (description.length > 100) {
      return 'Description must be at most 100 characters.';
    }
    return null;
  }
}
