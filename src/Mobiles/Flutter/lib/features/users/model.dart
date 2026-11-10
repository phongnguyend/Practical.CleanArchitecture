import '../../shared/models/record.dart';

class User {
  String id;
  String userName;
  String email;
  bool emailConfirmed;
  String phoneNumber;
  bool phoneNumberConfirmed;
  bool twoFactorEnabled;
  bool lockoutEnabled;
  String lockoutEnd;
  int accessFailedCount;
  User({
    this.id = '',
    this.userName = '',
    this.email = '',
    this.emailConfirmed = false,
    this.phoneNumber = '',
    this.phoneNumberConfirmed = false,
    this.twoFactorEnabled = false,
    this.lockoutEnabled = false,
    this.lockoutEnd = '',
    this.accessFailedCount = 0,
  });
  factory User.fromJson(Json json) => User(
    id: json['id']?.toString() ?? '',
    userName: json['userName']?.toString() ?? '',
    email: json['email']?.toString() ?? '',
    emailConfirmed: json['emailConfirmed'] == true,
    phoneNumber: json['phoneNumber']?.toString() ?? '',
    phoneNumberConfirmed: json['phoneNumberConfirmed'] == true,
    twoFactorEnabled: json['twoFactorEnabled'] == true,
    lockoutEnabled: json['lockoutEnabled'] == true,
    lockoutEnd: json['lockoutEnd']?.toString() ?? '',
    accessFailedCount: (json['accessFailedCount'] as num?)?.toInt() ?? 0,
  );
  Json toJson() => {
    'id': id,
    'userName': userName,
    'email': email,
    'emailConfirmed': emailConfirmed,
    'phoneNumber': phoneNumber,
    'phoneNumberConfirmed': phoneNumberConfirmed,
    'twoFactorEnabled': twoFactorEnabled,
    'lockoutEnabled': lockoutEnabled,
    'lockoutEnd': lockoutEnd,
    'accessFailedCount': accessFailedCount,
  };
  Json toPayload() => {
    if (id.isNotEmpty) 'id': id,
    'userName': userName,
    'email': email.trim(),
    'emailConfirmed': emailConfirmed,
    'phoneNumber': phoneNumber,
    'phoneNumberConfirmed': phoneNumberConfirmed,
    'twoFactorEnabled': twoFactorEnabled,
    'lockoutEnabled': lockoutEnabled,
    'lockoutEnd': lockoutEnd.isEmpty
        ? null
        : DateTime.parse(lockoutEnd).toUtc().toIso8601String(),
    'accessFailedCount': accessFailedCount,
  };
  String? validate() {
    if (userName.trim().isEmpty) return 'Username is required.';
    if (userName.trim().length < 3) {
      return 'Username needs at least 3 characters.';
    }
    if (email.trim().isEmpty) return 'Email is required.';
    if (email.trim().length < 3) return 'Email needs at least 3 characters.';
    if (email.isNotEmpty &&
        !RegExp(r'^[^\s@]+@[^\s@]+\.[^\s@]+$').hasMatch(email.trim())) {
      return 'Enter a valid email.';
    }
    if (lockoutEnd.isNotEmpty && DateTime.tryParse(lockoutEnd) == null) {
      return 'Enter a valid date and time.';
    }
    if (accessFailedCount < 0) {
      return 'Failed attempts must be a nonnegative integer.';
    }
    return null;
  }
}
