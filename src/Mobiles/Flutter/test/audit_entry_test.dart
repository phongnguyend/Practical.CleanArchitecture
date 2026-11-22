import 'package:classified_ads_mobile/features/auditlogs/audit_entry.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  testWidgets('audit details expand with readable field names and values', (
    tester,
  ) async {
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(
          body: SingleChildScrollView(
            child: AuditEntry(
              entry: {
                'id': '1',
                'action': 'Updated',
                'userName': 'admin',
                'createdDateTime': '2026-11-22T12:00:00Z',
                'log': '{"displayName":"Updated name","missing":null}',
              },
            ),
          ),
        ),
      ),
    );
    expect(find.text('By admin'), findsOneWidget);
    expect(find.text('Updated name'), findsNothing);
    await tester.tap(find.text('View event details'));
    await tester.pumpAndSettle();
    expect(find.text('display Name'), findsOneWidget);
    expect(find.text('Updated name'), findsOneWidget);
    expect(find.text('Not provided'), findsOneWidget);
    expect(find.text('Copy JSON'), findsOneWidget);
    await tester.tap(find.text('View event details'));
    await tester.pumpAndSettle();
    expect(find.text('Updated name'), findsNothing);
    expect(tester.takeException(), isNull);
  });

  testWidgets('plain events retain text and metadata fallbacks', (
    tester,
  ) async {
    await tester.pumpWidget(
      const MaterialApp(
        home: Scaffold(
          body: AuditEntry(
            entry: {
              'id': '2',
              'createdDateTime': 'unknown',
              'log': 'Plain message',
            },
          ),
        ),
      ),
    );
    expect(find.text('Event'), findsOneWidget);
    expect(find.text('By Unknown user'), findsOneWidget);
    await tester.tap(find.text('View event details'));
    await tester.pumpAndSettle();
    expect(find.text('Plain message'), findsOneWidget);
    expect(tester.takeException(), isNull);
  });
}
