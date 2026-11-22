jest.mock("@sentry/react-native", () => ({ init: jest.fn(), captureException: jest.fn(), wrap: jest.fn() }));
import { expect, jest, test } from "@jest/globals";
import Constants from "expo-constants";
import { initializeCrashReporting, recordUnexpectedError } from "../src/core/diagnostics/crash-reporting";
import { recordError, setCrashlyticsCollectionEnabled } from "@react-native-firebase/crashlytics";

jest.mock("expo-constants", () => ({ __esModule: true, default: {
  expoConfig: { extra: { crashlytics: { android: false } } },
} }));
jest.mock("react-native", () => ({ Platform: { OS: "android" } }));
jest.mock("@react-native-firebase/crashlytics", () => ({
  getCrashlytics: jest.fn(() => "instance"),
  setAttribute: jest.fn<() => Promise<void>>().mockResolvedValue(undefined),
  setCrashlyticsCollectionEnabled: jest.fn<() => Promise<void>>().mockResolvedValue(undefined),
  recordError: jest.fn(),
}));

test("unconfigured and debug builds do not enable reporting; release reports omit messages", async () => {
  const oldDebug = __DEV__;
  try {
    Object.defineProperty(globalThis, "__DEV__", { value: false, writable: true });
    await initializeCrashReporting();
    expect(setCrashlyticsCollectionEnabled).not.toHaveBeenCalled();
    Constants.expoConfig!.extra!.crashlytics.android = true;
    Object.defineProperty(globalThis, "__DEV__", { value: true, writable: true });
    await initializeCrashReporting();
    expect(setCrashlyticsCollectionEnabled).not.toHaveBeenCalled();
    Object.defineProperty(globalThis, "__DEV__", { value: false, writable: true });
    await initializeCrashReporting();
    expect(setCrashlyticsCollectionEnabled).toHaveBeenCalledWith(expect.anything(), true);
    const error = new TypeError("private response body");
    error.stack = "TypeError: private response body\n    at example (screen.ts:10:2)";
    recordUnexpectedError(error);
    const reported = jest.mocked(recordError).mock.calls[0][1];
    expect(reported.message).toBe("TypeError");
    expect(reported.stack).toContain("screen.ts:10:2");
    expect(reported.stack).not.toContain("private response body");
    const cancellation = new Error("cancelled");
    cancellation.name = "AbortError";
    recordUnexpectedError(cancellation);
    expect(recordError).toHaveBeenCalledTimes(1);
  } finally {
    Object.defineProperty(globalThis, "__DEV__", { value: oldDebug, writable: true });
  }
});
