import 'package:classified_ads_mobile/shared/widgets/action_group.dart';
import 'package:classified_ads_mobile/shared/widgets/action_label.dart';
import 'package:classified_ads_mobile/shared/widgets/app_theme.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';

void main() {
  for (final scale in [1.0, 2.0]) {
    testWidgets('actions fit a narrow screen at text scale $scale', (
      tester,
    ) async {
      await tester.pumpWidget(
        MaterialApp(
          theme: appTheme(Brightness.light),
          home: MediaQuery(
            data: MediaQueryData(textScaler: TextScaler.linear(scale)),
            child: Scaffold(
              body: Align(
                alignment: Alignment.topLeft,
                child: SizedBox(
                  width: 320,
                  child: ActionGroup(
                    children: [
                      for (final title in [
                        'Set password',
                        'Confirm email',
                        'Reset email',
                      ])
                        TextButton(onPressed: () {}, child: ActionLabel(title)),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      );
      expect(tester.takeException(), isNull);
      final buttons = find.byType(TextButton);
      final bounds = [
        for (var i = 0; i < 3; i++) tester.getRect(buttons.at(i)),
      ];
      for (final rect in bounds) {
        expect(rect.left, greaterThanOrEqualTo(0));
        expect(rect.right, lessThanOrEqualTo(320));
        expect(rect.height, greaterThanOrEqualTo(48));
      }
      for (var i = 0; i < bounds.length; i++) {
        for (var j = i + 1; j < bounds.length; j++) {
          expect(bounds[i].overlaps(bounds[j]), isFalse);
        }
      }
      if (scale == 1) {
        expect(bounds[1].left - bounds[0].right, 12);
        expect(bounds[0].width, bounds[1].width);
      } else {
        expect(bounds[1].top - bounds[0].bottom, 12);
        expect(bounds[0].width, 320);
      }
    });
  }
}
