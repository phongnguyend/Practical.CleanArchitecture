import { biometricGeneration, biometricUserStore, biometricLocked, initializeBiometrics, biometricEnabled, biometricPromptActive, lockBiometricStorage, unlockBiometricStorage, enableBiometricStorage, disableBiometricStorage, forgetBiometricStorage } from "../../core/auth/biometric-store";
import { clearNotifications } from "../../core/notifications";
import { Capacitor } from "@capacitor/core";
import { App } from "@capacitor/app";
import { User, UserManager, WebStorageStateStore } from "oidc-client-ts";
import env from "../../environments";
import { secureStore } from "../../core/auth/secure-store";
import { NativeBrowserNavigator } from "../../core/auth/browser-navigator";
import {
  isAuthCallback,
  loginRedirect,
  logoutRedirect,
  safeReturnPath,
} from "../../core/auth/callback";

const native = Capacitor.isNativePlatform();
const navigator = native ? new NativeBrowserNavigator() : undefined;
export const userManager = new UserManager(
  {
    authority: env.OpenIdConnect.Authority,
    client_id: env.OpenIdConnect.ClientId,
    redirect_uri: native
      ? loginRedirect
      : `${env.CurrentUrl}oidc-login-redirect`,
    popup_redirect_uri: loginRedirect,
    post_logout_redirect_uri: native ? logoutRedirect : env.CurrentUrl,
    popup_post_logout_redirect_uri: logoutRedirect,
    scope: `openid profile ClassifiedAds.WebAPI${native ? " offline_access" : ""}`,
    response_type: "code",
    automaticSilentRenew: false,
    userStore: new WebStorageStateStore({
      store: native ? biometricUserStore : window.localStorage,
    }),
    stateStore: new WebStorageStateStore({
      store: native ? secureStore : window.sessionStorage,
    }),
  },
  undefined,
  navigator,
);
let user: User | null = null;
const userKey = `oidc.user:${env.OpenIdConnect.Authority}:${env.OpenIdConnect.ClientId}`;
export async function enableBiometrics() {
  if (!user?.refresh_token) {
    throw new Error("Sign in before enabling biometric login.");
  }
  userManager.stopSilentRenew();
  await enableBiometricStorage(userKey, user.toStorageString());
  changed();
}
export async function disableBiometrics() {
  await disableBiometricStorage(userKey);
  userManager.startSilentRenew();
  changed();
}
export function lockSession() {
  if (!biometricEnabled()) {
    return;
  }
  userManager.stopSilentRenew();
  lockBiometricStorage();
  user = null;
  clearNotifications();
  changed();
}
export async function unlockSession() {
  await unlockBiometricStorage();
  const epoch = biometricGeneration();
  try {
    let restored = await userManager.getUser();
    if (biometricLocked() || epoch !== biometricGeneration()) {
      throw new Error("Session locked. Unlock again.");
    }
    if (restored?.expired) {
      restored = await userManager.signinSilent();
    }
    if (!restored || restored.expired || biometricLocked() || epoch !== biometricGeneration()) {
      throw new Error("Session locked or expired. Sign in again.");
    }
    user = restored;
    changed();
  } catch (error) {
    lockSession();
    throw error;
  }
}
const changed = () => window.dispatchEvent(new Event("session-changed"));
userManager.events.addUserLoaded((value) => {
  if (biometricLocked()) {
    return;
  }
  user = value;
  if (native && !biometricEnabled()) {
    userManager.startSilentRenew();
  }
  changed();
});
userManager.events.addUserUnloaded(() => {
  user = null;
  changed();
});
userManager.events.addAccessTokenExpired(() => {
  changed();
});
export async function loadUser() {
  if (native) {
    await initializeBiometrics();
    window.addEventListener("biometric-lock", lockSession);
    await App.addListener("appStateChange", ({ isActive }) => {
      if (!isActive && !biometricPromptActive()) {
        lockSession();
      }
    });
    if (biometricEnabled()) {
      return null;
    }
    const launch = await App.getLaunchUrl();
    if (launch?.url && isAuthCallback(launch.url)) {
      user = await userManager.signinRedirectCallback(launch.url);
      return user;
    }
  }
  user = await userManager.getUser();
  if (native && user?.expired && user.refresh_token) {
    try {
      user = await userManager.signinSilent();
    } catch {
      await userManager.removeUser();
      user = null;
    }
  }
  return user;
}
let pendingLogin: Promise<unknown> | undefined;
export function login(returnUrl?: string): Promise<unknown> {
  if (pendingLogin) {
    return pendingLogin;
  }
  sessionStorage.setItem("returnUrl", safeReturnPath(returnUrl ?? null));
  pendingLogin = (async () => {
    if (native) {
      userManager.stopSilentRenew();
      await refreshing?.catch(() => undefined);
      await forgetBiometricStorage();
      await userManager.removeUser();
      return userManager.signinPopup();
    }
    return userManager.signinRedirect();
  })().finally(() => {
    pendingLogin = undefined;
  });
  return pendingLogin;
}
export async function logout() {
  if (native) {
    userManager.stopSilentRenew();
    await refreshing?.catch(() => undefined);
    try {
      await userManager.signoutPopup();
    } finally {
      await forgetBiometricStorage();
      await userManager.removeUser();
      user = null;
      changed();
    }
  } else {
    await userManager.signoutRedirect();
  }
}
export const isLoggedIn = () => !!user?.access_token && (!user.expired || (native && !!user.refresh_token));
export const isAuthenticated = isLoggedIn;
let refreshing: Promise<User | null> | undefined;
export async function getAccessToken(): Promise<string> {
  if (!user) {
    throw new Error("Please sign in or unlock.");
  }
  if (!user.expired) {
    return user.access_token;
  }
  if (!native || !user.refresh_token) {
    throw new Error("Session expired. Sign in again.");
  }
  refreshing ??= userManager.signinSilent().finally(() => { refreshing = undefined; });
  const result = await refreshing;
  if (!user || !result || result.expired) {
    throw new Error("Session locked or expired. Sign in again.");
  }
  return result.access_token;
}
export const signoutRedirectCallback = () =>
  userManager.signoutRedirectCallback();
export const getCurrentUser = () => ({
  id: user?.profile.sub,
  userName: user?.profile.preferred_username,
  firstName: user?.profile.given_name,
  lastName: user?.profile.family_name,
});
