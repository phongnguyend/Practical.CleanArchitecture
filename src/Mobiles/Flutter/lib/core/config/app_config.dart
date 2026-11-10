class AppConfig {
  static const api = String.fromEnvironment(
    'API_URL',
    defaultValue: 'https://localhost:44312/api/',
  );
  static const authority = String.fromEnvironment(
    'AUTHORITY',
    defaultValue: 'https://localhost:44367',
  );
  static const hub = String.fromEnvironment(
    'NOTIFICATION_URL',
    defaultValue: 'https://localhost:44312/hubs/notification',
  );
  static const clientId = 'ClassifiedAds.Mobile.Flutter';
  static const redirect = 'classifiedads.flutter:/oauthredirect';
  static const logout = 'classifiedads.flutter:/logout';
}
