import { beforeEach, expect, jest, test } from "@jest/globals";
import * as SecureStore from "expo-secure-store";
import * as auth from "../src/core/auth/session";

const mockValues = new Map<string, string>();
jest.mock("expo-secure-store", () => ({
  WHEN_UNLOCKED_THIS_DEVICE_ONLY: "device-only",
  canUseBiometricAuthentication: () => true,
  getItemAsync: jest.fn(async (key: string) => mockValues.get(key) ?? null),
  setItemAsync: jest.fn(async (key: string, value: string) => { mockValues.set(key, value); }),
  deleteItemAsync: jest.fn(async (key: string) => { mockValues.delete(key); }),
}));
jest.mock("expo-auth-session", () => ({}));
jest.mock("expo-web-browser", () => ({}));
const normal = "classifiedads.session";
const protectedKey = "classifiedads.biometric.session";
const marker = "classifiedads.biometric.enabled";
const session = () => JSON.stringify({ accessToken: "test-access", refreshToken: "test-refresh", expiresAt: Date.now() + 3600000 });
beforeEach(async () => {
  mockValues.clear();
  jest.mocked(SecureStore.getItemAsync).mockImplementation(async key => mockValues.get(key) ?? null);
  await auth.restore();
  jest.clearAllMocks();
});
async function enrolled() {
  mockValues.set(normal, session());
  await auth.restore();
  await auth.enableBiometrics();
}
test("enabling requires a protected read and removes the old session", async () => {
  await enrolled();
  expect(auth.biometricEnabled()).toBe(true);
  expect(mockValues.has(normal)).toBe(false);
  expect(SecureStore.getItemAsync).toHaveBeenCalledWith(protectedKey, expect.objectContaining({ requireAuthentication: true }));
});
test("startup stays locked without reading protected or normal tokens", async () => {
  mockValues.set(marker, "true");
  mockValues.set(protectedKey, session());
  expect(await auth.restore()).toBe(false);
  expect(SecureStore.getItemAsync).toHaveBeenCalledTimes(1);
  await expect(auth.accessToken()).rejects.toThrow("unlock");
});
test("cancelled unlock cannot access an API token", async () => {
  await enrolled();
  auth.lockSession();
  jest.mocked(SecureStore.getItemAsync).mockRejectedValueOnce(new Error("cancelled"));
  await expect(auth.unlockSession()).rejects.toThrow("cancelled");
  await expect(auth.accessToken()).rejects.toThrow("unlock");
});
test("successful biometric read unlocks a valid session", async () => {
  await enrolled();
  auth.lockSession();
  await auth.unlockSession();
  expect(await auth.accessToken()).toBe("test-access");
});
test("backgrounding during unlock rejects the stale result", async () => {
  await enrolled();
  auth.lockSession();
  jest.mocked(SecureStore.getItemAsync).mockImplementationOnce(async () => {
    auth.lockSession();
    return session();
  });
  await expect(auth.unlockSession()).rejects.toThrow();
  await expect(auth.accessToken()).rejects.toThrow("unlock");
});
test("backgrounding during disable preserves biometric protection", async () => {
  await enrolled();
  jest.mocked(SecureStore.getItemAsync).mockImplementationOnce(async () => {
    auth.lockSession();
    return session();
  });
  await expect(auth.disableBiometrics()).rejects.toThrow("Session changed");
  expect(mockValues.has(normal)).toBe(false);
  expect(auth.biometricEnabled()).toBe(true);
});
test("logout deletes both token stores and enrollment", async () => {
  await enrolled();
  await auth.logout();
  expect(mockValues.size).toBe(0);
  expect(auth.biometricEnabled()).toBe(false);
  await expect(auth.accessToken()).rejects.toThrow();
});
