import Constants from "expo-constants";
import { Platform } from "react-native";

let client: typeof import("@react-native-firebase/crashlytics") | undefined;

export async function initializeCrashReporting(): Promise<void> {
  const configured = Constants.expoConfig?.extra?.crashlytics?.[Platform.OS] === true;
  if (!configured || __DEV__ || Platform.OS === "web") {
    return;
  }
  // Load only after the guard: unconfigured builds have no Firebase native module.
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  const sdk: typeof import("@react-native-firebase/crashlytics") = require("@react-native-firebase/crashlytics");
  const instance = sdk.getCrashlytics();
  await sdk.setAttribute(instance, "framework", "ReactNative");
  await sdk.setCrashlyticsCollectionEnabled(instance, true);
  client = sdk;
}

// Unexpected caught failures only; omit response bodies, tokens, and user input.
export function recordUnexpectedError(error: unknown): void {
  if (!client || !(error instanceof Error) || error.name === "AbortError") {
    return;
  }
  const safeError = new Error(error.name);
  // Preserve frames without copying the original message at the top of the stack.
  safeError.stack = [error.name, ...(error.stack?.split("\n").slice(1) || [])].join("\n");
  client.recordError(client.getCrashlytics(), safeError);
}
