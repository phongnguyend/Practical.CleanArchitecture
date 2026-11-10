import 'package:flutter_test/flutter_test.dart';
import 'package:flutter/material.dart';
import 'package:classified_ads_mobile/shared/models/record.dart';
import 'package:classified_ads_mobile/core/services/app_services.dart';
import 'package:classified_ads_mobile/app/app.dart';
import 'package:classified_ads_mobile/features/products/model.dart';
import 'package:classified_ads_mobile/features/products/details.dart';
import 'package:classified_ads_mobile/features/files/model.dart';
import 'package:classified_ads_mobile/features/files/form.dart';
import 'package:classified_ads_mobile/features/users/model.dart';
import 'package:classified_ads_mobile/features/users/user_actions.dart';
import 'package:classified_ads_mobile/features/configurationentries/model.dart';
import 'package:classified_ads_mobile/features/configurationentries/details.dart';

class SignedOutServices extends AppServices {
  @override
  Future<String> readTheme() async => 'system';
  @override
  Future<void> saveTheme(String value) async {}
  @override
  Future<void> restore() async {}
  @override
  Future<void> stopNotifications() async {}
}

class WorkspaceServices extends SignedOutServices {
  @override
  bool get signedIn => true;
  @override
  Future<void> startNotifications(
    void Function(String) received, {
    void Function(String)? status,
  }) async {}
  @override
  Future<dynamic> api(String path, {String method = 'GET', Json? body}) async =>
      [
        {
          'id': 'product-1',
          'name': 'Sample product',
          'code': 'P01',
          'description': 'Example',
        },
      ];
}

void main() {
  test('Passwords require matching confirmation and preserve whitespace', () {
    expect(validatePassword(' secret ', 'secret'), isNotNull);
    expect(validatePassword(' secret ', ' secret '), isNull);
  });
  test('Products and files validate their own constraints', () {
    expect(Product(name: 'ab').validate(), contains('at least 3'));
    expect(
      Product(name: 'Valid', code: 'abc', description: 'Example').validate(),
      isNull,
    );
    expect(StoredFile(name: 'Report').validate(), contains('required'));
    expect(
      StoredFile(name: 'Report', description: 'Example').validate(),
      isNull,
    );
  });
  test(
    'User API payload preserves account fields and normalizes optional date',
    () {
      final user = User(
        id: 'u1',
        userName: 'tester',
        email: ' test@example.com ',
        accessFailedCount: 2,
        emailConfirmed: true,
      );
      expect(user.validate(), isNull);
      expect(user.toPayload()['id'], 'u1');
      expect(user.toPayload()['email'], 'test@example.com');
      expect(user.toPayload()['accessFailedCount'], 2);
      expect(user.toPayload()['lockoutEnd'], isNull);
      user.accessFailedCount = -1;
      expect(user.validate(), contains('nonnegative'));
    },
  );
  test('Configuration payload preserves exact values', () {
    final setting = ConfigurationEntry(
      key: 'Setting',
      value: '  exact  ',
      isSensitive: true,
    );
    expect(setting.validate(), isNull);
    expect(setting.toPayload()['value'], '  exact  ');
  });
  testWidgets('Sensitive details are masked and history highlights changes', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        home: Scaffold(
          body: Column(
            children: [
              ConfigurationEntryDetails(
                ConfigurationEntry(
                  key: 'Setting',
                  value: 'secret',
                  isSensitive: true,
                ),
              ),
              ProductHistory(
                Product(name: 'Updated product'),
                changed: const {'name': true},
              ),
            ],
          ),
        ),
      ),
    );
    expect(find.text('secret'), findsNothing);
    expect(find.text('******'), findsOneWidget);
    expect(find.text('Name · Changed'), findsOneWidget);
    expect(find.text('Updated product'), findsOneWidget);
  });
  testWidgets('File edit keeps filename and encryption read-only', (
    tester,
  ) async {
    await tester.pumpWidget(
      MaterialApp(
        home: Scaffold(
          body: StoredFileForm(
            value: StoredFile(
              id: 'f1',
              fileName: 'report.pdf',
              encrypted: true,
            ),
            changed: () {},
            services: SignedOutServices(),
            file: null,
            pick: (_) {},
            busy: false,
          ),
        ),
      ),
    );
    final filename = tester.widget<TextField>(
      find.byWidgetPredicate(
        (widget) =>
            widget is TextField && widget.decoration?.labelText == 'File name',
      ),
    );
    expect(filename.enabled, false);
    expect(
      tester.widget<SwitchListTile>(find.byType(SwitchListTile)).onChanged,
      isNull,
    );
  });
  testWidgets('Products load and invalid edits stay open', (tester) async {
    await tester.pumpWidget(ClassifiedAdsApp(services: WorkspaceServices()));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Products').first);
    await tester.pumpAndSettle();
    expect(find.text('Sample product'), findsOneWidget);
    await tester.tap(find.text('Add new'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Save'));
    await tester.pumpAndSettle();
    expect(find.text('Name is required.'), findsOneWidget);
    expect(find.text('Save'), findsOneWidget);
  });
  testWidgets('Signed out does not expose records', (tester) async {
    await tester.pumpWidget(ClassifiedAdsApp(services: SignedOutServices()));
    await tester.pumpAndSettle();
    expect(find.text('Sign in'), findsOneWidget);
    expect(find.text('Add new'), findsNothing);
  });

  testWidgets(
    'Changed product prompts before discard and preserves draft on Cancel',
    (tester) async {
      await tester.pumpWidget(ClassifiedAdsApp(services: WorkspaceServices()));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Products').first);
      await tester.pumpAndSettle();
      await tester.tap(find.text('Add new'));
      await tester.pumpAndSettle();
      await tester.enterText(
        find.byType(TextFormField).first,
        'Unsaved product',
      );
      await tester.pumpAndSettle();
      await tester.tap(find.text('Cancel'));
      await tester.pumpAndSettle();
      expect(find.text('Discard unsaved changes?'), findsOneWidget);
      await tester.tap(find.widgetWithText(TextButton, 'Cancel').last);
      await tester.pumpAndSettle();
      expect(find.text('Unsaved product'), findsOneWidget);
      await tester.tap(find.text('Cancel'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Confirm'));
      await tester.pumpAndSettle();
      expect(find.text('Save'), findsNothing);
    },
  );
}
