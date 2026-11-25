import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:classified_ads_mobile/core/services/app_services.dart';
import 'package:classified_ads_mobile/features/admin/admin_screen.dart';
import 'package:classified_ads_mobile/features/admin/notification_playgrounds.dart';

class PlaygroundServices extends AppServices {
  Map<String, dynamic>? queued;

  @override
  Future<dynamic> api(
    String path, {
    String method = 'GET',
    Map<String, dynamic>? body,
  }) async {
    if (method == 'POST') {
      queued = body;
      return {'id': 'queued-id'};
    }
    return {'provider': 'Fake', 'deviceCount': 0, 'items': []};
  }
}

void main() {
  for (final channel in ['email', 'sms']) {
    testWidgets('$channel validates and queues trimmed fields', (tester) async {
      final services = PlaygroundServices();
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: SingleChildScrollView(
              child: NotificationPlaygrounds(
                services: services,
                channel: channel,
                back: () {},
              ),
            ),
          ),
        ),
      );
      await tester.pumpAndSettle();
      await tester.ensureVisible(find.text('Queue message'));
      await tester.tap(find.text('Queue message'));
      await tester.pumpAndSettle();
      expect(find.text('Complete all required fields.'), findsOneWidget);
      expect(services.queued, isNull);
      final fields = find.byType(TextField);
      if (channel == 'email') {
        await tester.enterText(fields.at(0), ' from@example.com ');
        await tester.enterText(fields.at(1), ' to@example.com ');
        await tester.enterText(fields.at(2), ' Subject ');
        await tester.enterText(fields.at(3), ' Message ');
      } else {
        await tester.enterText(fields.at(0), ' +15551234567 ');
        await tester.enterText(fields.at(1), ' Message ');
      }
      await tester.ensureVisible(find.text('Queue message'));
      await tester.tap(find.text('Queue message'));
      await tester.pumpAndSettle();
      expect(services.queued?['body'], 'Message');
      expect(
        services.queued?['recipient'],
        channel == 'email' ? 'to@example.com' : '+15551234567',
      );
      expect(find.text('Message queued successfully.'), findsOneWidget);
    });
  }
  testWidgets(
    'playground validates and queues a message for the current account',
    (tester) async {
      final services = PlaygroundServices();
      await tester.pumpWidget(
        MaterialApp(
          home: Scaffold(
            body: SingleChildScrollView(child: AdminScreen(services: services)),
          ),
        ),
      );
      await tester.tap(find.text('Playgrounds'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Mobile Push'));
      await tester.pumpAndSettle();
      await tester.ensureVisible(find.text('Queue message'));
      await tester.tap(find.text('Queue message'));
      await tester.pumpAndSettle();
      expect(find.text('Title and message are required.'), findsOneWidget);
      expect(services.queued, isNull);
      await tester.enterText(find.byType(TextField).at(0), ' Update ');
      await tester.enterText(find.byType(TextField).at(1), ' Details ');
      await tester.ensureVisible(find.text('Queue message'));
      await tester.tap(find.text('Queue message'));
      await tester.pumpAndSettle();
      expect(services.queued, {
        'title': 'Update',
        'body': 'Details',
        'section': 'notifications',
      });
      expect(find.text('Message queued successfully.'), findsOneWidget);
    },
  );
}
