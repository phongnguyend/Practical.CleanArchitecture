import { Capacitor } from "@capacitor/core";
import { FirebaseCrashlytics } from "@capacitor-firebase/crashlytics";
// Never forward response bodies, URLs, tokens, field values, or raw error messages.
export async function reportFailure(
  source: "javascript" | "promise" | "react",
  error?: unknown,
) {
  if (
    !Capacitor.isNativePlatform() ||
    !Capacitor.isPluginAvailable("FirebaseCrashlytics")
  ) {
    return;
  }
  try {
    await FirebaseCrashlytics.recordException({
      message: `Unhandled ${source} failure`,
      ...(error instanceof Error
        ? {
            stacktrace: (error.stack ?? "")
              .split("\n")
              .slice(1)
              .flatMap((line) => {
                // Only bundled asset basenames and line numbers; no messages or URL queries.
                const match = line.match(
                  /\/(assets\/[A-Za-z0-9_-]+\.js):(\d+):\d+/,
                );
                return match
                  ? [{ fileName: match[1], lineNumber: Number(match[2]) }]
                  : [];
              }),
          }
        : {}),
    });
  } catch {
    // Reporting must never prevent application startup or recovery.
  }
}
export function initializeCrashlytics() {
  window.addEventListener("error", (event) => {
    void reportFailure("javascript", event.error);
  });
  window.addEventListener("unhandledrejection", (event) => {
    void reportFailure("promise", event.reason);
  });
}
