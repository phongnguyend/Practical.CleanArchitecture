import 'package:flutter/material.dart';

import '../../core/services/app_services.dart';
import '../../shared/models/record.dart';
import '../../shared/widgets/record_page.dart';
import '../../shared/widgets/record_widgets.dart';
import '../../shared/widgets/import_dialog.dart';
import 'model.dart';
import 'repository.dart';
import 'form.dart';
import 'details.dart';

class ProductScreen extends StatefulWidget {
  const ProductScreen({super.key, required this.services});
  final AppServices services;
  @override
  State<ProductScreen> createState() => _ProductScreenState();
}

class _ProductScreenState extends State<ProductScreen> {
  late final repository = ProductRepository(widget.services);
  bool showImages = false;
  @override
  Widget build(BuildContext context) => RecordPage<Product>(
    title: 'Products',
    repository: repository,
    empty: () => Product(),
    copy: (value) => Product.fromJson(value.toJson()),
    id: (value) => value.id,
    label: (value) => value.name,
    searchText: (value) => value.name,
    serialize: (value) => value.toJson(),
    validate: (value) => value.validate(),
    form: (value, changed, file, pick, busy) => ProductForm(
      value: value,
      changed: changed,
      file: file,
      pick: pick,
      busy: busy,
      services: widget.services,
    ),
    details: (value) => ProductDetails(value),
    summary: (value) => Column(
      children: [
        if (showImages) ProductPicture(row: value.toJson()),
        ProductSummary(value),
        TextButton(
          onPressed: () => ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(
                'The rating ${value.starRating == 0 ? 4 : value.starRating} was clicked!',
              ),
            ),
          ),
          child: Text('★ ${value.starRating == 0 ? 4 : value.starRating}/5'),
        ),
      ],
    ),
    history: repository.history,
    historyBody: (entry) => ProductHistory(
      Product.fromJson(auditSnapshot(entry)),
      changed: Map<String, dynamic>.from(entry['highLight'] ?? {}),
    ),
    toolbar: (run, reload) => [
      TextButton(
        onPressed: () => setState(() => showImages = !showImages),
        child: Text(showImages ? 'Hide Image' : 'Show Image'),
      ),
      TextButton(
        onPressed: () async {
          final done = await showDialog<bool>(
            context: context,
            builder: (context) => ImportDialog(
              suffix: '.csv',
              submit: repository.importFile,
              services: widget.services,
            ),
          );
          if (done == true) {
            await reload();
            if (context.mounted) {
              ScaffoldMessenger.of(
                context,
              ).showSnackBar(const SnackBar(content: Text('Import processed')));
            }
          }
        },
        child: const Text('Import'),
      ),
      TextButton(
        onPressed: () => run(
          () => widget.services.download(
            'products/exportascsv',
            'Products.csv',
            Rect.fromLTWH(0, 0, MediaQuery.sizeOf(context).width, 1),
          ),
        ),
        child: const Text('Export CSV'),
      ),
      TextButton(
        onPressed: () => run(
          () => widget.services.download(
            'products/exportaspdf',
            'Products.pdf',
            Rect.fromLTWH(0, 0, MediaQuery.sizeOf(context).width, 1),
          ),
        ),
        child: const Text('Export PDF'),
      ),
    ],
  );
}
