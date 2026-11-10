import 'package:file_picker/file_picker.dart';

abstract interface class RecordRepository<T> {
  Future<List<T>> list();
  Future<T> get(String id);
  Future<void> save(T value, PlatformFile? file);
  Future<void> remove(String id);
}
