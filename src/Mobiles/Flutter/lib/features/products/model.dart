import '../../shared/models/record.dart';

class Product {
  String id;
  String name;
  String code;
  String description;
  double price;
  double starRating;
  String imageUrl;
  Product({
    this.id = '',
    this.name = '',
    this.code = '',
    this.description = '',
    this.price = 0,
    this.starRating = 0,
    this.imageUrl = '',
  });
  factory Product.fromJson(Json json) => Product(
    id: json['id']?.toString() ?? '',
    name: json['name']?.toString() ?? '',
    code: json['code']?.toString() ?? '',
    description: json['description']?.toString() ?? '',
    price: (json['price'] as num?)?.toDouble() ?? 0,
    starRating: (json['starRating'] as num?)?.toDouble() ?? 0,
    imageUrl: json['imageUrl']?.toString() ?? '',
  );
  Json toJson() => {
    'id': id,
    'name': name,
    'code': code,
    'description': description,
    'price': price,
    'starRating': starRating,
    'imageUrl': imageUrl,
  };
  Json toPayload() => {
    if (id.isNotEmpty) 'id': id,
    'name': name,
    'code': code,
    'description': description,
  };
  String? validate() {
    if (name.trim().isEmpty) return 'Name is required.';
    if (name.trim().length < 3) return 'Name needs at least 3 characters.';
    if (code.trim().isEmpty) return 'Code is required.';
    if (code.length > 10) return 'Code must be at most 10 characters.';
    if (description.trim().isEmpty) return 'Description is required.';
    if (description.length > 100) {
      return 'Description must be at most 100 characters.';
    }
    return null;
  }
}
