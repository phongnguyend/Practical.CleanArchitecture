import * as Sentry from "@sentry/capacitor";
import * as SentryReact from "@sentry/react";
import { Capacitor } from "@capacitor/core";

declare const __SENTRY_CONFIG__: { SENTRY_ENABLED?: boolean; SENTRY_DSN?: string; SENTRY_ENVIRONMENT?: string; SENTRY_RELEASE?: string };
let enabled = false;
export function initializeSentry() {
  const config = typeof __SENTRY_CONFIG__ === "undefined" ? {} : __SENTRY_CONFIG__;
  if (!config.SENTRY_ENABLED || !config.SENTRY_DSN || !import.meta.env.PROD || !Capacitor.isNativePlatform()) {
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
  }, SentryReact.init);
  enabled = true;
}
export function reportSentryFailure(error: unknown) {
  if (enabled) {
    Sentry.captureException(error instanceof Error ? error : new Error("Unexpected React failure"));
  }
}
