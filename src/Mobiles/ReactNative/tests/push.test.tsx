import React from "react";
import { beforeEach, expect, jest, test } from "@jest/globals";
import { render, screen, waitFor } from "@testing-library/react-native";
import PushInbox from "../src/features/notifications/PushInbox";
import { api } from "../src/core/network/api";
import { enablePush, initializePush, pushTarget, syncPush } from "../src/core/notifications/push";

jest.mock("../src/core/network/api", () => ({ api: jest.fn() }));
jest.mock("../src/core/auth/session", () => ({ sessionUnlocked: () => false }));
jest.mock("expo-constants", () => ({ __esModule: true, default: { expoConfig: { extra: { push: { android: false, ios: false } } } } }));
jest.mock("expo-notifications", () => ({ AndroidImportance: { DEFAULT: 3 }, setNotificationChannelAsync: jest.fn() }));
jest.mock("expo-crypto", () => ({ randomUUID: () => "11111111-1111-4111-8111-111111111111" }));

beforeEach(() => {
  jest.mocked(api).mockImplementation(async (path) => path === "notifications/status" ? { enabled: false, allowTestMessages: false } : { items: [], total: 0 });
});

test("unconfigured push never initializes Firebase or requests a registration", async () => {
  await initializePush(jest.fn(), jest.fn());
  await syncPush();
  await expect(enablePush()).rejects.toThrow("not configured");
  expect(api).not.toHaveBeenCalled();
});

test("notification routes reject malformed IDs and external destinations", () => {
  expect(pushTarget({ notificationId: "bad", section: "products" })).toBeUndefined();
  expect(pushTarget({ notificationId: "11111111-1111-4111-8111-111111111111", section: "https://example.com" })?.section).toBe("notifications");
  expect(pushTarget({ notificationId: "11111111-1111-4111-8111-111111111111", section: "products" })?.section).toBe("products");
});

test("inbox explains disabled configuration and keeps refresh available", async () => {
  await render(<PushInbox navigate={jest.fn()} />);
  await waitFor(() => expect(api).toHaveBeenCalledWith("notifications/inbox?page=1"));
  expect(screen.getByText("No saved notifications yet.")).toBeTruthy();
  expect(screen.getByRole("button", { name: "Enable notifications" })).toBeDisabled();
  expect(screen.getByRole("button", { name: "Refresh inbox" })).toBeEnabled();
});
