import { SecureStorage } from "@aparajita/capacitor-secure-storage";
import type { AsyncStorage } from "oidc-client-ts";
export const secureStore: AsyncStorage = {
  get length() {
    return SecureStorage.keys().then((keys) => keys.length);
  },
  clear: () => SecureStorage.clear(),
  getItem: (key) => SecureStorage.getItem(key),
  setItem: (key, value) => SecureStorage.setItem(key, value),
  removeItem: (key) => SecureStorage.removeItem(key),
  key: async (index) => (await SecureStorage.keys())[index] ?? null,
};
