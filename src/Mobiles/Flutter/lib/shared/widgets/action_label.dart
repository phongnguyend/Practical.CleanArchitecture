import 'package:flutter/material.dart';

const _icons = <String, String>{
  'Home': 'home',
  'Products': 'products',
  'Files': 'files',
  'Users': 'users',
  'Roles': 'roles',
  'Configuration Entries': 'configuration',
  'Audit logs': 'history',
  'Notifications': 'notifications',
  'Practical.CleanArchitecture': 'link',
  'Sign out': 'logout',
  'Add new': 'add',
  'Edit': 'edit',
  'Delete': 'delete',
  'View': 'view',
  'History': 'history',
  'Refresh': 'refresh',
  'Save': 'save',
  'Cancel': 'close',
  'Close': 'close',
  'Confirm': 'check',
  'Back': 'back',
  'Previous': 'back',
  'Download': 'download',
  'Upload': 'upload',
  'Upload file': 'upload',
  'Import': 'upload',
  'Export CSV': 'download',
  'Export PDF': 'download',
  'Copy JSON': 'copy',
  'Copied': 'check',
  'Show JSON': 'view',
  'Hide JSON': 'view',
  'Theme': 'theme',
  'Next': 'next',
  'Choose file': 'files',
  'Show more': 'add',
  'Choose date/time': 'calendar',
  'Choose date and time': 'calendar',
  'Choose date': 'calendar',
  'Choose time': 'history',
  'Clear': 'close',
  'Sign in': 'link',
  'Done': 'check',
  'Set password': 'lock',
  'Reset email': 'mail',
  'Confirm email': 'mail',
  'Keep editing': 'edit',
  'Discard': 'delete',
  'Export XLSX': 'download',
  'Show Image': 'image',
  'Hide Image': 'image',
  'system': 'configuration',
  'light': 'theme',
  'dark': 'theme',
  'System': 'configuration',
  'Light': 'theme',
  'Dark': 'theme',
};

/// Decorative icons supplement the visible, accessible action label.
class ActionLabel extends StatelessWidget {
  const ActionLabel(this.label, {super.key, this.color});
  final String label;
  final Color? color;
  @override
  Widget build(BuildContext context) {
    final icon = _icons[label];
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (icon != null) ...[
          ExcludeSemantics(
            child: Image.asset(
              'assets/icons/action_$icon.png',
              width: 18,
              height: 18,
              color: color ?? DefaultTextStyle.of(context).style.color,
            ),
          ),
          const SizedBox(width: 8),
        ],
        Flexible(
          child: Text(
            label,
            textAlign: TextAlign.center,
            style: color == null ? null : TextStyle(color: color),
          ),
        ),
      ],
    );
  }
}
