import Constants from "expo-constants";
import { randomUUID } from "expo-crypto";
import { AndroidImportance, setNotificationChannelAsync } from "expo-notifications";
import * as SecureStore from "expo-secure-store";
import { PermissionsAndroid, Platform } from "react-native";
import { api } from "../network/api";
import { sessionUnlocked } from "../auth/session";

export type InboxMessage = { id: string; title: string; body: string; section: string; createdAt: string; readAt: string | null };
const sections = ["home", "products", "files", "users", "roles", "configurationentries", "auditlogentries", "notifications"];
export function pushTarget(data: Record<string, unknown> | undefined): { id: string; section: string } | undefined {
  if (typeof data?.notificationId !== "string" || !/^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$/i.test(data.notificationId)) {
    return;
  }
  return { id: data.notificationId, section: typeof data.section === "string" && sections.includes(data.section) ? data.section : "notifications" };
}
export const pushConfigured = () => Platform.OS !== "web" && Constants.expoConfig?.extra?.push?.[Platform.OS] === true;
const enabledKey = "push.enabled";
const installationKey = "push.installation";
let pending: ReturnType<typeof pushTarget>;
let generation = 0;
let listening = false;
let opened: (() => void) | undefined;
let received: (() => void) | undefined;
function sdk() {
  // Unconfigured builds must never access the native Firebase module.
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  return require("@react-native-firebase/messaging") as typeof import("@react-native-firebase/messaging");
}
async function installation() {
  let id = await SecureStore.getItemAsync(installationKey);
  if (!id) {
    id = randomUUID();
    await SecureStore.setItemAsync(installationKey, id);
  }
  return id;
}
async function register(token: string) {
  if (!sessionUnlocked() || await SecureStore.getItemAsync(enabledKey) !== "true") {
    return;
  }
  await api(`notifications/devices/${await installation()}`, "PUT", { app: "ReactNative", platform: Platform.OS, token });
}
export async function initializePush(onOpened: () => void, onReceived: () => void) {
  opened = onOpened;
  received = onReceived;
  if (!pushConfigured() || listening) {
    return;
  }
  listening = true;
  const messaging = sdk();
  const client = messaging.getMessaging();
  messaging.onNotificationOpenedApp(client, (message) => {
    pending = pushTarget(message.data);
    opened?.();
  });
  messaging.onMessage(client, () => {
    if (sessionUnlocked()) {
      received?.();
    }
  });
  messaging.onTokenRefresh(client, (token) => {
    void register(token).catch(() => undefined);
  });
  const initial = await messaging.getInitialNotification(client);
  if (initial) {
    pending = pushTarget(initial.data);
    opened?.();
  }
}
export async function syncPush() {
  if (pushConfigured() && sessionUnlocked() && await SecureStore.getItemAsync(enabledKey) === "true") {
    await register(await sdk().getToken(sdk().getMessaging()));
  }
}
export async function enablePush() {
  if (!pushConfigured()) {
    throw new Error("Push notifications are not configured for this build.");
  }
  const status = await api("notifications/status");
  if (!status.enabled) {
    throw new Error("Push notifications are not enabled on the server.");
  }
  const epoch = generation;
  const messaging = sdk();
  const client = messaging.getMessaging();
  if (Platform.OS === "android") {
    await setNotificationChannelAsync("classifiedads_updates", { name: "Classified Ads updates", importance: AndroidImportance.DEFAULT });
  }
  if (Platform.OS === "android" && Number(Platform.Version) >= 33) {
    const permission = await PermissionsAndroid.request(PermissionsAndroid.PERMISSIONS.POST_NOTIFICATIONS);
    if (permission !== PermissionsAndroid.RESULTS.GRANTED) {
      throw new Error("Allow notifications in device settings to receive updates.");
    }
  } else if (Platform.OS === "ios") {
    const permission = await messaging.requestPermission(client);
    if (permission !== 1 && permission !== 2) {
      throw new Error("Allow notifications in device settings to receive updates.");
    }
    await messaging.registerDeviceForRemoteMessages(client);
  }
  if (epoch !== generation || !sessionUnlocked()) {
    throw new Error("Unlock your workspace before enabling notifications.");
  }
  await messaging.setAutoInitEnabled(client, true);
  await SecureStore.setItemAsync(enabledKey, "true");
  await register(await messaging.getToken(client));
}
export async function disablePush() {
  generation++;
  pending = undefined;
  const id = await SecureStore.getItemAsync(installationKey);
  await SecureStore.deleteItemAsync(enabledKey);
  try {
    if (id && sessionUnlocked()) {
      await api(`notifications/devices/${id}`, "DELETE");
    }
  } finally {
    await SecureStore.deleteItemAsync(installationKey);
    if (pushConfigured()) {
      const messaging = sdk();
      await messaging.setAutoInitEnabled(messaging.getMessaging(), false);
      await messaging.deleteToken(messaging.getMessaging());
    }
  }
}
export async function consumePush(): Promise<string | undefined> {
  if (!sessionUnlocked() || !pending) {
    return;
  }
  const target = pending;
  pending = undefined;
  await api(`notifications/inbox/${target.id}/read`, "PUT").catch(() => undefined);
  return target.section;
}
