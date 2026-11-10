import 'package:flutter/material.dart';

import '../../shared/widgets/form_controls.dart';
import 'model.dart';

class UserSummary extends StatelessWidget {
  const UserSummary(this.value, {super.key, this.changed = const {}});
  final User value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue('Email', value.email, changed: changed['email'] == true),
    ],
  );
}

class UserDetails extends StatelessWidget {
  const UserDetails(this.value, {super.key, this.changed = const {}});
  final User value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue(
        'Username',
        value.userName,
        changed: changed['userName'] == true,
      ),
      RecordValue('Email', value.email, changed: changed['email'] == true),
      RecordValue(
        'Email confirmed',
        value.emailConfirmed ? 'Yes' : 'No',
        changed: changed['emailConfirmed'] == true,
      ),
      RecordValue(
        'Phone number',
        value.phoneNumber,
        changed: changed['phoneNumber'] == true,
      ),
      RecordValue(
        'Phone confirmed',
        value.phoneNumberConfirmed ? 'Yes' : 'No',
        changed: changed['phoneNumberConfirmed'] == true,
      ),
      RecordValue(
        'Two-factor enabled',
        value.twoFactorEnabled ? 'Yes' : 'No',
        changed: changed['twoFactorEnabled'] == true,
      ),
      RecordValue(
        'Lockout enabled',
        value.lockoutEnabled ? 'Yes' : 'No',
        changed: changed['lockoutEnabled'] == true,
      ),
      RecordValue(
        'Lockout end (ISO 8601)',
        localDate(value.lockoutEnd),
        changed: changed['lockoutEnd'] == true,
      ),
      RecordValue(
        'Failed attempts',
        value.accessFailedCount,
        changed: changed['accessFailedCount'] == true,
      ),
    ],
  );
}

class UserHistory extends StatelessWidget {
  const UserHistory(this.value, {super.key, this.changed = const {}});
  final User value;
  final Map<String, dynamic> changed;
  @override
  Widget build(BuildContext context) => Column(
    crossAxisAlignment: CrossAxisAlignment.start,
    children: [
      RecordValue(
        'Username',
        value.userName,
        changed: changed['userName'] == true,
      ),
      RecordValue('Email', value.email, changed: changed['email'] == true),
      RecordValue(
        'Email confirmed',
        value.emailConfirmed ? 'Yes' : 'No',
        changed: changed['emailConfirmed'] == true,
      ),
      RecordValue(
        'Phone number',
        value.phoneNumber,
        changed: changed['phoneNumber'] == true,
      ),
      RecordValue(
        'Phone confirmed',
        value.phoneNumberConfirmed ? 'Yes' : 'No',
        changed: changed['phoneNumberConfirmed'] == true,
      ),
      RecordValue(
        'Two-factor enabled',
        value.twoFactorEnabled ? 'Yes' : 'No',
        changed: changed['twoFactorEnabled'] == true,
      ),
      RecordValue(
        'Lockout enabled',
        value.lockoutEnabled ? 'Yes' : 'No',
        changed: changed['lockoutEnabled'] == true,
      ),
      RecordValue(
        'Lockout end (ISO 8601)',
        localDate(value.lockoutEnd),
        changed: changed['lockoutEnd'] == true,
      ),
      RecordValue(
        'Failed attempts',
        value.accessFailedCount,
        changed: changed['accessFailedCount'] == true,
      ),
    ],
  );
}
