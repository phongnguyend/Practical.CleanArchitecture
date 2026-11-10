import 'package:file_picker/file_picker.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/repository.dart';
import 'model.dart';

class ProductRepository implements RecordRepository<Product> {
  ProductRepository(this.api);
  final AppServices api;
  @override
  Future<List<Product>> list() async => (await api.api('products') as List)
      .map((json) => Product.fromJson(Map<String, dynamic>.from(json)))
      .toList();
  @override
  Future<Product> get(String id) async => Product.fromJson(
    Map<String, dynamic>.from(
      await api.api('products/${Uri.encodeComponent(id)}'),
    ),
  );
  @override
  Future<void> remove(String id) async {
    await api.api('products/${Uri.encodeComponent(id)}', method: 'DELETE');
  }

  @override
  Future<void> save(Product value, PlatformFile? file) async {
    await api.api(
      'products${value.id.isEmpty ? '' : '/${Uri.encodeComponent(value.id)}'}',
      method: value.id.isEmpty ? 'POST' : 'PUT',
      body: value.toPayload(),
    );
  }

  Future<List<dynamic>> history(String id) async =>
      await api.api('products/${Uri.encodeComponent(id)}/auditlogs') as List;
  Future<void> importFile(PlatformFile file) async {
    await api.upload('products/importcsv', file);
  }
}
