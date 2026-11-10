import 'package:flutter/material.dart';

import '../../shared/widgets/form_controls.dart';
import '../../shared/widgets/record_widgets.dart';
import 'model.dart';

class ProductSummary extends StatelessWidget {
  const ProductSummary(this.value, {super.key, this.changed = const {}});
  final Product value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue(
        'Code',
        value.code.toUpperCase(),
        changed: changed['code'] == true,
      ),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'Price',
        value.price == 0 ? 5 : value.price,
        changed: changed['price'] == true,
      ),
      RecordValue(
        '5 Star Rating',
        value.starRating == 0 ? 4 : value.starRating,
        changed: changed['starRating'] == true,
      ),
    ],
  );
}

class ProductDetails extends StatelessWidget {
  const ProductDetails(this.value, {super.key, this.changed = const {}});
  final Product value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      ProductPicture(row: value.toJson()),
      RecordValue('Name', value.name, changed: changed['name'] == true),
      RecordValue(
        'Code',
        value.code.toUpperCase(),
        changed: changed['code'] == true,
      ),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
      RecordValue(
        'Price',
        value.price == 0 ? 5 : value.price,
        changed: changed['price'] == true,
      ),
      RecordValue(
        '5 Star Rating',
        value.starRating == 0 ? 4 : value.starRating,
        changed: changed['starRating'] == true,
      ),
    ],
  );
}

class ProductHistory extends StatelessWidget {
  const ProductHistory(this.value, {super.key, this.changed = const {}});
  final Product value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue('Code', value.code, changed: changed['code'] == true),
      RecordValue('Name', value.name, changed: changed['name'] == true),
      RecordValue(
        'Description',
        value.description,
        changed: changed['description'] == true,
      ),
    ],
  );
}
