import { beforeEach, expect, it, vi } from "vitest";
const state = vi.hoisted(() => ({ values: new Map<string, string>(), protected: "", native: true }));
const get = vi.hoisted(() => vi.fn());
vi.mock("@capacitor/core", () => ({ Capacitor: { isNativePlatform: () => state.native } }));
vi.mock("@capacitor/app", () => ({ App: { getState: async () => ({ isActive: true }) } }));
vi.mock("../../src/core/auth/secure-store", () => ({ secureStore: {
  getItem: async (key: string) => state.values.get(key) ?? null,
  setItem: async (key: string, value: string) => { state.values.set(key, value); },
  removeItem: async (key: string) => { state.values.delete(key); },
} }));
vi.mock("@capgo/capacitor-native-biometric", () => ({
  AccessControl: { BIOMETRY_CURRENT_SET: 1 }, AuthenticationStrength: { STRONG: "strong" },
  NativeBiometric: {
    isAvailable: async () => ({ isAvailable: true, authenticationStrength: "strong" }),
    setData: async ({ value }: { value: string }) => { state.protected = value; },
    getSecureData: get,
    deleteData: async () => { state.protected = ""; },
  },
}));
import * as auth from "../../src/core/auth/biometric-store";
beforeEach(async () => {
  state.values.clear();
  state.protected = "";
  state.native = true;
  get.mockReset().mockImplementation(async () => ({ value: state.protected }));
  await auth.initializeBiometrics();
});
async function enrolled() {
  state.values.set("oidc.user", "session");
  await auth.enableBiometricStorage("oidc.user", "session");
}
it("moves credentials behind biometrics and locks without reading them on restart", async () => {
  await enrolled();
  expect(state.values.has("oidc.user")).toBe(false);
  get.mockClear();
  await auth.initializeBiometrics();
  expect(await auth.biometricUserStore.getItem("oidc.user")).toBeNull();
  expect(get).not.toHaveBeenCalled();
});
it("cancellation stays locked and forbids token writes", async () => {
  await enrolled();
  auth.lockBiometricStorage();
  get.mockRejectedValueOnce(new Error("cancelled"));
  await expect(auth.unlockBiometricStorage()).rejects.toThrow("cancelled");
  expect(await auth.biometricUserStore.getItem("oidc.user")).toBeNull();
  await expect(auth.biometricUserStore.setItem("oidc.user", "rotated")).rejects.toThrow("locked");
});
it("requires a successful protected read to unlock", async () => {
  await enrolled();
  auth.lockBiometricStorage();
  await auth.unlockBiometricStorage();
  expect(await auth.biometricUserStore.getItem("oidc.user")).toBe("session");
});
it("ignores an unlock completing after a background transition", async () => {
  await enrolled();
  auth.lockBiometricStorage();
  get.mockImplementationOnce(async () => {
    auth.lockBiometricStorage();
    return { value: "session" };
  });
  await expect(auth.unlockBiometricStorage()).rejects.toThrow("Session changed");
  expect(auth.biometricLocked()).toBe(true);
});
it("does not downgrade protection when disable is interrupted", async () => {
  await enrolled();
  get.mockImplementationOnce(async () => {
    auth.lockBiometricStorage();
    return { value: "session" };
  });
  await expect(auth.disableBiometricStorage("oidc.user")).rejects.toThrow("Session changed");
  expect(state.values.has("oidc.user")).toBe(false);
});
it("clears protected credentials and enrollment on forget", async () => {
  await enrolled();
  await auth.forgetBiometricStorage();
  expect(state.protected).toBe("");
  expect(state.values.size).toBe(0);
  expect(auth.biometricEnabled()).toBe(false);
});
it("rejects the plugin's web fallback", async () => {
  state.native = false;
  await expect(enrolled()).rejects.toThrow("native app");
  expect(auth.biometricEnabled()).toBe(false);
});
