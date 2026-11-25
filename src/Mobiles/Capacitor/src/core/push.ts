import { Capacitor } from "@capacitor/core";
import { FirebaseMessaging } from "@capacitor-firebase/messaging";
import { secureStore } from "./auth/secure-store";
import { getAccessToken, isAuthenticated } from "../containers/Auth/authService";
import env from "../environments";
import { updateNotifications } from "./notifications";

export const pushConfigured = () => Capacitor.isNativePlatform() && import.meta.env.VITE_PUSH_NOTIFICATIONS_ENABLED === "true";
export type InboxMessage = { id: string; title: string; body: string; section: string; createdAt: string; readAt: string | null };
const sections = ["home", "products", "files", "users", "roles", "configurationentries", "auditlogentries", "notifications"];
export function pushTarget(data: Record<string, unknown> | undefined) {
  if (typeof data?.notificationId !== "string" || !/^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(data.notificationId)) {
    return undefined;
  }
  return { id: data.notificationId, section: typeof data.section === "string" && sections.includes(data.section) ? data.section : "notifications" };
}
export async function pushApi(path: string, method = "GET", body?: unknown) {
  const response = await fetch(`${env.ResourceServer.Endpoint}notifications/${path}`, {
    method,
    headers: { Authorization: `Bearer ${await getAccessToken()}`, "Content-Type": "application/json" },
    body: body === undefined ? undefined : JSON.stringify(body),
    signal: AbortSignal.timeout(20000),
  });
  if (!response.ok) {
    throw new Error(`Notification request failed (${response.status}).`);
  }
  return response.status === 204 ? undefined : response.json();
}
let initialized = false;
let pending: ReturnType<typeof pushTarget>;
let generation = 0;
const enabledKey = "push.enabled";
const installationKey = "push.installation";
async function register(token: string) {
  if (!isAuthenticated() || await secureStore.getItem(enabledKey) !== "true") {
    return;
  }
  let id = await secureStore.getItem(installationKey);
  if (!id) {
    id = crypto.randomUUID();
    await secureStore.setItem(installationKey, id);
  }
  await pushApi(`devices/${id}`, "PUT", { app: "Capacitor", platform: Capacitor.getPlatform(), token });
}
export async function initializePush() {
  if (!pushConfigured() || initialized) {
    return;
  }
  initialized = true;
  await FirebaseMessaging.addListener("tokenReceived", ({ token }) => {
    void register(token).catch(() => undefined);
  });
  await FirebaseMessaging.addListener("notificationReceived", () => {
    if (isAuthenticated()) {
      updateNotifications("Connected", "You have a new notification.");
    }
  });
  await FirebaseMessaging.addListener("notificationActionPerformed", ({ notification }) => {
    pending = pushTarget(notification.data as Record<string, unknown> | undefined);
    window.dispatchEvent(new Event("push-opened"));
  });
}
export async function syncPush() {
  if (pushConfigured() && isAuthenticated() && await secureStore.getItem(enabledKey) === "true") {
    await register((await FirebaseMessaging.getToken()).token);
  }
}
export async function enablePush() {
  if (!pushConfigured() || !(await pushApi("status")).enabled) {
    throw new Error("Push notifications are not configured.");
  }
  const epoch = generation;
  if ((await FirebaseMessaging.requestPermissions()).receive !== "granted") {
    throw new Error("Allow notifications in device settings to receive updates.");
  }
  if (epoch !== generation || !isAuthenticated()) {
    throw new Error("Unlock your workspace before enabling notifications.");
  }
  if (Capacitor.getPlatform() === "android") {
    await FirebaseMessaging.createChannel({ id: "classifiedads_updates", name: "Classified Ads updates", importance: 3 });
  }
  await secureStore.setItem(enabledKey, "true");
  await syncPush();
}
export async function disablePush() {
  generation++;
  pending = undefined;
  if (!Capacitor.isNativePlatform()) {
    return;
  }
  const id = await secureStore.getItem(installationKey);
  await secureStore.removeItem(enabledKey);
  try {
    if (id && isAuthenticated()) {
      await pushApi(`devices/${id}`, "DELETE");
    }
  } finally {
    await secureStore.removeItem(installationKey);
    if (pushConfigured()) {
      await FirebaseMessaging.deleteToken();
      await FirebaseMessaging.removeAllDeliveredNotifications();
    }
  }
}
export async function consumePush() {
  if (!isAuthenticated() || !pending) {
    return undefined;
  }
  const target = pending;
  pending = undefined;
  await pushApi(`inbox/${target.id}/read`, "PUT").catch(() => undefined);
  return target.section === "auditlogentries" ? "auditlogs" : target.section;
}
