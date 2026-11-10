import 'dart:convert';

typedef Json = Map<String, dynamic>;
String apiError(int status, String text) {
  if (status == 401) {
    return 'Session expired. Please sign out and sign in again.';
  }
  if (status == 403) return 'You do not have permission for this action.';
  try {
    final body = jsonDecode(text);
    if (body is Map) {
      if (body['errorMessage'] != null) return body['errorMessage'].toString();
      if (body['errors'] is Map) {
        return (body['errors'] as Map).values
            .expand((v) => v is List ? v : [v])
            .join('\n');
      }
      if (body['detail'] != null || body['title'] != null) {
        return (body['detail'] ?? body['title']).toString();
      }
    }
  } catch (_) {
    /* Keep status and readable text for non-JSON errors. */
  }
  return 'Request failed ($status): ${text.substring(0, text.length.clamp(0, 1500))}';
}

Json auditSnapshot(Json row) {
  final value = row['data'] ?? row['log'];
  if (value is Map) return Json.from(value);
  try {
    final parsed = jsonDecode(value as String);
    return parsed is Map ? Json.from(parsed) : {};
  } catch (_) {
    return {};
  }
}
