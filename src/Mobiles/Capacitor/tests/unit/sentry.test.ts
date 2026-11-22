import { afterEach, expect, it, vi } from "vitest";
const init = vi.hoisted(() => vi.fn());
const capture = vi.hoisted(() => vi.fn());
vi.mock("@sentry/capacitor", () => ({ init, captureException: capture }));
vi.mock("@sentry/react", () => ({ init: vi.fn() }));
vi.mock("@capacitor/core", () => ({ Capacitor: { isNativePlatform: () => true } }));
afterEach(() => {
  vi.unstubAllGlobals();
  vi.unstubAllEnvs();
  vi.clearAllMocks();
  vi.resetModules();
});
it("missing configuration does not initialize reporting", async () => {
  const sdk = await import("../../src/core/sentry");
  sdk.initializeSentry();
  sdk.reportSentryFailure(new Error("private"));
  expect(init).not.toHaveBeenCalled();
  expect(capture).not.toHaveBeenCalled();
});
it("configured native production builds filter event payloads while preserving frames", async () => {
  vi.stubEnv("PROD", true);
  vi.stubGlobal("__SENTRY_CONFIG__", { SENTRY_ENABLED: true, SENTRY_DSN: "https://public@example.com/123" });
  const sdk = await import("../../src/core/sentry");
  sdk.initializeSentry();
  expect(init).toHaveBeenCalledTimes(1);
  const options = init.mock.calls[0][0];
  const event = options.beforeSend({ user: { email: "private" }, request: { headers: { Authorization: "secret" } }, extra: { password: "secret" }, message: "private", breadcrumbs: [{ message: "private" }], exception: { values: [{ type: "TypeError", value: "private", stacktrace: { frames: [{ lineno: 42 }] } }] } });
  expect(JSON.stringify(event)).not.toMatch(/private|secret/);
  expect(event.exception.values[0].stacktrace.frames[0].lineno).toBe(42);
  sdk.reportSentryFailure(new Error("handled"));
  expect(capture).toHaveBeenCalledTimes(1);
});
