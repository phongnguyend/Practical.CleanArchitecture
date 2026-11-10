import 'dart:async';

import 'package:flutter/material.dart';

import '../models/record.dart';
import 'home_button.dart';

String _homeClock(DateTime value) {
  final hour = value.hour % 12 == 0 ? 12 : value.hour % 12;
  final minute = value.minute.toString().padLeft(2, '0');
  final second = value.second.toString().padLeft(2, '0');
  final period = value.hour < 12 ? 'AM' : 'PM';
  return '${value.month}/${value.day}/${value.year}, $hour:$minute:$second $period';
}

class HomeInfo extends StatefulWidget {
  const HomeInfo({super.key, required this.openProject});
  final VoidCallback openProject;
  @override
  State<HomeInfo> createState() => _HomeInfoState();
}

class _HomeInfoState extends State<HomeInfo> {
  DateTime now = DateTime.now();
  Timer? timer;
  @override
  void initState() {
    super.initState();
    timer = Timer.periodic(const Duration(seconds: 1), (_) {
      if (mounted) setState(() => now = DateTime.now());
    });
  }

  @override
  void dispose() {
    timer?.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Card(
    margin: EdgeInsets.zero,
    child: Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        spacing: 6,
        children: [
          Text(
            'Welcome to Classified Ads',
            style: Theme.of(context).textTheme.titleLarge,
          ),
          Text('Flutter · ${_homeClock(now)}'),
          const Text('Developed by Phong Nguyen · @phongnguyend'),
          HomeButton(
            onPressed: widget.openProject,
            label: 'Practical.CleanArchitecture',
          ),
        ],
      ),
    ),
  );
}

class ProductPicture extends StatelessWidget {
  const ProductPicture({super.key, required this.row});
  final Json row;
  @override
  Widget build(BuildContext context) {
    final placeholder = Container(
      width: 120,
      height: 100,
      alignment: Alignment.center,
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(12),
      ),
      child: const Icon(Icons.inventory_2, size: 48),
    );
    return row['imageUrl'] is String && (row['imageUrl'] as String).isNotEmpty
        ? Image.network(
            row['imageUrl'],
            width: 120,
            height: 100,
            fit: BoxFit.contain,
            semanticLabel: row['name'],
            errorBuilder: (_, error, stack) => placeholder,
          )
        : placeholder;
  }
}
