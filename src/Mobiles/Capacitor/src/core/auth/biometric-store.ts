import { App } from "@capacitor/app";
import { Capacitor } from "@capacitor/core";
import { NativeBiometric, AccessControl, AuthenticationStrength } from "@capgo/capacitor-native-biometric";
import type { AsyncStorage } from "oidc-client-ts";
import { secureStore } from "./secure-store";

const protectedKey = "classifiedads.biometric.session";
const enabledKey = "classifiedads.biometric.enabled";
let enabled = false;
let locked = false;
let generation = 0;
let value: string | null = null;
let pending = false;
export const biometricGeneration = () => generation;
export const biometricEnabled = () => enabled;
export const biometricLocked = () => enabled && locked;
export const biometricSupported = () => Capacitor.isNativePlatform();
export const biometricPromptActive = () => pending;
export async function initializeBiometrics() {
  enabled = await secureStore.getItem(enabledKey) === "true";
  locked = enabled;
  value = null;
}
async function prompt<T>(operation: () => Promise<T>): Promise<T> {
  if (pending) {
    throw new Error("Biometric authentication is already in progress.");
  }
  pending = true;
  try {
    return await operation();
  } finally {
    pending = false;
    if (!(await App.getState()).isActive) {
      lockBiometricStorage();
      window.dispatchEvent(new Event("biometric-lock"));
    }
  }
}
async function protect(data: string) {
  if (!biometricSupported()) {
    throw new Error("Biometric login requires the native app.");
  }
  await prompt(() => NativeBiometric.setData({ key: protectedKey, value: data,
    accessControl: AccessControl.BIOMETRY_CURRENT_SET, authValidityDuration: 0,
    title: "Protect Classified Ads session", negativeButtonText: "Cancel" }));
}
export async function enableBiometricStorage(key: string, data: string) {
  const epoch = generation;
  const availability = await NativeBiometric.isAvailable({ useFallback: false });
  if (!availability.isAvailable || availability.authenticationStrength !== AuthenticationStrength.STRONG) {
    throw new Error("Enroll strong biometrics on this device first.");
  }
  await protect(data);
  const verified = await prompt(() => NativeBiometric.getSecureData({ key: protectedKey, title: "Unlock Classified Ads", reason: "Verify biometric login", negativeButtonText: "Cancel" }));
  if (verified.value !== data || epoch !== generation) {
    throw new Error("App was backgrounded. Enable biometric login again.");
  }
  await secureStore.setItem(enabledKey, "true");
  enabled = true;
  locked = false;
  value = data;
  await secureStore.removeItem(key);
  if (epoch !== generation) {
    lockBiometricStorage();
    throw new Error("Session locked. Unlock again.");
  }
}
export async function disableBiometricStorage(key: string) {
  if (locked || !value) {
    throw new Error("Unlock before changing biometric settings.");
  }
  const epoch = generation;
  const verified = await prompt(() => NativeBiometric.getSecureData({ key: protectedKey, title: "Unlock Classified Ads", negativeButtonText: "Cancel" }));
  if (!verified.value || epoch !== generation || locked || !value) {
    throw new Error("Session changed. Unlock before changing biometric settings.");
  }
  await secureStore.setItem(key, value);
  await NativeBiometric.deleteData({ key: protectedKey });
  await secureStore.removeItem(enabledKey);
  enabled = false;
}
export function lockBiometricStorage() {
  generation++;
  if (enabled) {
    locked = true;
    value = null;
  }
}
export async function unlockBiometricStorage() {
  if (!enabled) {
    throw new Error("Biometric login is not enabled.");
  }
  const epoch = generation;
  const result = await prompt(() => NativeBiometric.getSecureData({ key: protectedKey,
    title: "Unlock Classified Ads", reason: "Unlock your saved session", negativeButtonText: "Cancel" }));
  if (!result.value || epoch !== generation) {
    throw new Error("Session changed. Try unlocking again.");
  }
  value = result.value;
  locked = false;
}
export async function forgetBiometricStorage() {
  generation++;
  value = null;
  locked = true;
  if (Capacitor.isNativePlatform()) {
    await NativeBiometric.deleteData({ key: protectedKey });
    await secureStore.removeItem(enabledKey);
  }
  enabled = false;
  locked = false;
}
export const biometricUserStore: AsyncStorage = {
  get length() { return secureStore.length; },
  key: (index) => secureStore.key(index),
  clear: async () => { await forgetBiometricStorage(); },
  getItem: async (key) => enabled ? (locked ? null : value) : secureStore.getItem(key),
  setItem: async (key, data) => {
    if (!enabled) {
      await secureStore.setItem(key, data);
      return;
    }
    if (locked) {
      throw new Error("Session locked. Unlock before using your session.");
    }
    const epoch = generation;
    await protect(data);
    if (epoch !== generation) {
      throw new Error("Session locked. Unlock again.");
    }
    value = data;
  },
  removeItem: async (key) => {
    value = null;
    await secureStore.removeItem(key);
  },
};
