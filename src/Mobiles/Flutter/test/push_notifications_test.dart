import 'package:flutter_test/flutter_test.dart';
import 'package:classified_ads_mobile/core/services/app_services.dart';
import 'package:classified_ads_mobile/core/services/push_notifications.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();
  test('unconfigured push does not touch Firebase or request permission', () async {
    final push = PushNotifications(AppServices());
    await push.initialize(() {}, () {});
    await push.sync();
    expect(await push.consume(), isNull);
    await expectLater(push.enable(), throwsException);
    push.dispose();
  });
  test('notification destinations are validated', () {
    expect(pushSection({'notificationId': 'bad'}), isNull);
    const id = '11111111-1111-4111-8111-111111111111';
    expect(pushSection({'notificationId': id, 'section': 'products'}), 'products');
    expect(pushSection({'notificationId': id, 'section': 'https://example.com'}), 'notifications');
  });
}
