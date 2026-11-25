import * as AuthSession from "expo-auth-session";
import * as SecureStore from "expo-secure-store";
import * as WebBrowser from "expo-web-browser";
import { config } from "../config/environment";
import { disablePush } from "../notifications/push";

type Session = {
  accessToken: string;
  refreshToken?: string;
  idToken?: string;
  expiresAt: number;
};
let session: Session | null = null;
let refreshing: Promise<string> | undefined;
let biometric = false;
let generation = 0;
let prompting = false;
const normalKey = "classifiedads.session";
const protectedKey = "classifiedads.biometric.session";
const enabledKey = "classifiedads.biometric.enabled";
const protectedOptions: SecureStore.SecureStoreOptions = {
  requireAuthentication: true,
  authenticationPrompt: "Unlock Classified Ads",
  keychainService: "classifiedads.biometric",
  keychainAccessible: SecureStore.WHEN_UNLOCKED_THIS_DEVICE_ONLY,
};
export const sessionUnlocked = () => session !== null;
export const biometricEnabled = () => biometric;
export const biometricAvailable = () => SecureStore.canUseBiometricAuthentication();
export function lockSession() {
  generation++;
  if (biometric) {
    session = null;
    refreshing = undefined;
    return true;
  }
  return false;
}
async function protectedOperation<T>(operation: () => Promise<T>): Promise<T> {
  if (prompting) {
    throw new Error("Biometric authentication is already in progress.");
  }
  prompting = true;
  try {
    return await operation();
  } finally {
    prompting = false;
  }
}
export async function enableBiometrics() {
  if (!session?.refreshToken || !biometricAvailable()) {
    throw new Error("Sign in and enroll biometrics on this device first.");
  }
  const epoch = generation;
  const value = JSON.stringify(session);
  await protectedOperation(async () => {
    await SecureStore.setItemAsync(protectedKey, value, protectedOptions);
    if (await SecureStore.getItemAsync(protectedKey, protectedOptions) !== value) {
      throw new Error("Biometric enrollment did not complete.");
    }
  });
  if (epoch !== generation) {
    throw new Error("App was backgrounded. Enable biometric login again.");
  }
  await SecureStore.setItemAsync(enabledKey, "true");
  biometric = true;
  await SecureStore.deleteItemAsync(normalKey);
  if (epoch !== generation) {
    lockSession();
    throw new Error("Session locked. Unlock again.");
  }
}
export async function disableBiometrics() {
  if (!session) {
    throw new Error("Unlock before changing biometric settings.");
  }
  const epoch = generation;
  const stored = await protectedOperation(() => SecureStore.getItemAsync(protectedKey, protectedOptions));
  if (!stored || epoch !== generation || !session) {
    throw new Error("Session changed. Unlock before changing biometric settings.");
  }
  await SecureStore.setItemAsync(normalKey, JSON.stringify(session));
  await SecureStore.deleteItemAsync(protectedKey, protectedOptions);
  await SecureStore.deleteItemAsync(enabledKey);
  biometric = false;
}
export async function unlockSession() {
  if (!biometric) {
    throw new Error("Biometric login is not enabled.");
  }
  const epoch = generation;
  const stored = await protectedOperation(() => SecureStore.getItemAsync(protectedKey, protectedOptions));
  if (!stored || epoch !== generation) {
    throw new Error("Biometric credentials are unavailable. Please sign in again.");
  }
  session = JSON.parse(stored);
  try {
    await accessToken();
  } catch (error) {
    session = null;
    throw error;
  }
  return true;
}
async function forgetBiometrics() {
  generation++;
  session = null;
  await SecureStore.deleteItemAsync(protectedKey, protectedOptions);
  await SecureStore.deleteItemAsync(enabledKey);
  biometric = false;
}
async function save(token: AuthSession.TokenResponse) {
  const epoch = generation;
  const next = {
    accessToken: token.accessToken,
    refreshToken: token.refreshToken || session?.refreshToken,
    idToken: token.idToken || session?.idToken,
    expiresAt: Date.now() + (token.expiresIn || 300) * 1000,
  };
  if (biometric) {
    await protectedOperation(() => SecureStore.setItemAsync(protectedKey, JSON.stringify(next), protectedOptions));
  } else {
    await SecureStore.setItemAsync(normalKey, JSON.stringify(next));
  }
  if (epoch !== generation) {
    throw new Error("Session locked. Please unlock again.");
  }
  session = next;
  return next.accessToken;
}
export async function restore() {
  biometric = await SecureStore.getItemAsync(enabledKey) === "true";
  session = null;
  if (biometric) {
    return false;
  }
  const stored = await SecureStore.getItemAsync(normalKey);
  session = stored ? JSON.parse(stored) : null;
  return !!session;
}
export async function login() {
  await disablePush().catch(() => undefined);
  await forgetBiometrics();
  await SecureStore.deleteItemAsync(normalKey);
  const discovery = await AuthSession.fetchDiscoveryAsync(config.issuer);
  const request = new AuthSession.AuthRequest({
    clientId: config.clientId,
    redirectUri: config.redirect,
    scopes: ["openid", "profile", "offline_access", "ClassifiedAds.WebAPI"],
    responseType: AuthSession.ResponseType.Code,
    usePKCE: true,
  });
  const response = await request.promptAsync(discovery);
  if (response.type !== "success") {
    throw new Error("Sign-in was cancelled or failed.");
  }
  await save(
    await AuthSession.exchangeCodeAsync(
      {
        clientId: config.clientId,
        redirectUri: config.redirect,
        code: response.params.code,
        extraParams: { code_verifier: request.codeVerifier! },
      },
      discovery,
    ),
  );
}
export async function accessToken(): Promise<string> {
  if (!session) {
    throw new Error("Please sign in or unlock.");
  }
  if (session.expiresAt > Date.now() + 60000) {
    return session.accessToken;
  }
  if (!session.refreshToken) {
    throw new Error("Session expired. Please sign in again.");
  }
  if (!refreshing) {
    const previous = session;
    refreshing = (async () => {
      const tokens = await AuthSession.refreshAsync(
        { clientId: config.clientId, refreshToken: previous.refreshToken! },
        await AuthSession.fetchDiscoveryAsync(config.issuer),
      );
      if (session !== previous) {
        throw new Error("Session changed. Please sign in again.");
      }
      return save(tokens);
    })().finally(() => {
      refreshing = undefined;
    });
  }
  return refreshing;
}
export async function logout() {
  await disablePush().catch(() => undefined);
  const idToken = session?.idToken;
  await forgetBiometrics();
  await SecureStore.deleteItemAsync(normalKey);
  if (idToken) {
    const discovery = await AuthSession.fetchDiscoveryAsync(config.issuer);
    if (discovery.endSessionEndpoint) {
      await WebBrowser.openAuthSessionAsync(
        `${discovery.endSessionEndpoint}?${new URLSearchParams({ id_token_hint: idToken, post_logout_redirect_uri: config.logoutRedirect })}`,
        config.logoutRedirect,
      );
    }
  }
}
