import { describe, expect, it, vi, beforeEach } from "vitest";
import {
  isAuthCallback,
  loginRedirect,
  logoutRedirect,
  safeReturnPath,
} from "../../src/core/auth/callback";
const storage = vi.hoisted(() => new Map<string, string>());
vi.mock("@aparajita/capacitor-secure-storage", () => ({
  SecureStorage: {
    keys: async () => [...storage.keys()],
    getItem: async (key: string) => storage.get(key) ?? null,
    setItem: async (key: string, value: string) => {
      storage.set(key, value);
    },
    removeItem: async (key: string) => {
      storage.delete(key);
    },
    clear: async () => {
      storage.clear();
    },
  },
}));
import { secureStore } from "../../src/core/auth/secure-store";
describe("OIDC callbacks", () => {
  it("accepts only this app's registered callback, including query state", () => {
    expect(isAuthCallback(loginRedirect + "?code=test&state=test")).toBe(true);
    for (const url of [
      "https://evil.example/oauthredirect",
      "classifiedads.capacitor.evil:/oauthredirect",
      "classifiedads.capacitor://evil/oauthredirect",
      "classifiedads.capacitor:/other",
      "invalid",
    ]) {
      expect(isAuthCallback(url)).toBe(false);
    }
    expect(isAuthCallback(logoutRedirect, logoutRedirect)).toBe(true);
  });
  it("rejects external return destinations", () => {
    for (const path of [
      null,
      "https://evil.example",
      "//evil.example",
      "/\\evil.example",
    ]) {
      expect(safeReturnPath(path)).toBe("/home");
    }
    expect(safeReturnPath("/products")).toBe("/products");
  });
});
describe("encrypted OIDC storage adapter", () => {
  beforeEach(() => storage.clear());
  it("supports the asynchronous storage contract used for PKCE state and sessions", async () => {
    await secureStore.setItem("oidc.state", "state");
    expect(await secureStore.getItem("oidc.state")).toBe("state");
    expect(await secureStore.length).toBe(1);
    expect(await secureStore.key(0)).toBe("oidc.state");
    await secureStore.removeItem("oidc.state");
    expect(await secureStore.getItem("oidc.state")).toBeNull();
    await secureStore.setItem("oidc.user", "session");
    await secureStore.clear();
    expect(await secureStore.length).toBe(0);
  });
});
