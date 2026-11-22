import Constants from "expo-constants";
import * as Sentry from "@sentry/react-native";
let enabled = false;
export function initializeSentry(): void {
  const config = Constants.expoConfig?.extra?.sentry;
  if (__DEV__ || !config?.SENTRY_ENABLED || !config.SENTRY_DSN || enabled) {
    return;
  }
  Sentry.init({
    dsn: config.SENTRY_DSN,
    environment: config.SENTRY_ENVIRONMENT,
    release: config.SENTRY_RELEASE || undefined,
    sendDefaultPii: false,
    maxBreadcrumbs: 0,
    tracesSampleRate: 0,
    beforeSend(event) {
      delete event.user;
      delete event.request;
      delete event.extra;
      delete event.breadcrumbs;
      delete event.message;
      for (const exception of event.exception?.values ?? []) {
        exception.value = exception.type || "Unexpected error";
      }
      return event;
    },
  });
  enabled = true;
}
export function recordSentryError(error: Error): void {
  if (enabled) {
    Sentry.captureException(error);
  }
}
export const wrapWithSentry = Sentry.wrap;
