import { expect, jest, test } from "@jest/globals";
import Constants from "expo-constants";
import * as Sentry from "@sentry/react-native";
import { initializeSentry, recordSentryError } from "../src/core/diagnostics/sentry";
jest.mock("expo-constants", () => ({ __esModule: true, default: { expoConfig: { extra: { sentry: {} } } } }));
jest.mock("@sentry/react-native", () => ({ init: jest.fn(), captureException: jest.fn(), wrap: jest.fn() }));
test("Sentry is opt-in, excludes development, and scrubs event values", () => {
  const original = __DEV__;
  try {
    Object.defineProperty(globalThis, "__DEV__", { value: false, writable: true });
    initializeSentry();
    expect(Sentry.init).not.toHaveBeenCalled();
    Constants.expoConfig!.extra!.sentry = { SENTRY_ENABLED: true, SENTRY_DSN: "https://public@example.com/123" };
    Object.defineProperty(globalThis, "__DEV__", { value: true, writable: true });
    initializeSentry();
    expect(Sentry.init).not.toHaveBeenCalled();
    Object.defineProperty(globalThis, "__DEV__", { value: false, writable: true });
    initializeSentry();
    const options = jest.mocked(Sentry.init).mock.calls[0][0]!;
    const event = options.beforeSend!({ type: undefined, user: { email: "private" }, message: "private", request: { data: "secret" }, exception: { values: [{ type: "TypeError", value: "secret" }] } }, {});
    expect(JSON.stringify(event)).not.toMatch(/private|secret/);
    recordSentryError(new Error("handled"));
    expect(Sentry.captureException).toHaveBeenCalledTimes(1);
  } finally {
    Object.defineProperty(globalThis, "__DEV__", { value: original, writable: true });
  }
});
