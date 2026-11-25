import { beforeEach, describe, expect, it, vi } from "vitest";
vi.mock("@capacitor/core", () => ({ Capacitor: { isNativePlatform: () => false, getPlatform: () => "web" } }));
vi.mock("@capacitor-firebase/messaging", () => ({ FirebaseMessaging: { getToken: vi.fn(), requestPermissions: vi.fn(), addListener: vi.fn() } }));
vi.mock("../../src/containers/Auth/authService", () => ({ isAuthenticated: () => false, getAccessToken: vi.fn() }));
vi.mock("../../src/core/auth/secure-store", () => ({ secureStore: { getItem: vi.fn(), setItem: vi.fn(), removeItem: vi.fn() } }));
import { FirebaseMessaging } from "@capacitor-firebase/messaging";
import { consumePush, enablePush, initializePush, pushTarget, syncPush } from "../../src/core/push";

describe("mobile push", () => {
  beforeEach(() => vi.clearAllMocks());
  it("does not load native messaging or request permission when disabled", async () => {
    await initializePush();
    await syncPush();
    await expect(enablePush()).rejects.toThrow("not configured");
    expect(FirebaseMessaging.getToken).not.toHaveBeenCalled();
    expect(FirebaseMessaging.requestPermissions).not.toHaveBeenCalled();
    expect(FirebaseMessaging.addListener).not.toHaveBeenCalled();
  });
  it("does not navigate an unauthenticated session", async () => {
    expect(await consumePush()).toBeUndefined();
  });
  it("accepts only known destinations and valid notification IDs", () => {
    expect(pushTarget({ notificationId: "bad" })).toBeUndefined();
    const notificationId = "11111111-1111-4111-8111-111111111111";
    expect(pushTarget({ notificationId, section: "https://example.com" })?.section).toBe("notifications");
    expect(pushTarget({ notificationId, section: "products" })?.section).toBe("products");
  });
});
