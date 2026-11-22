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
    automaticSilentRenew: native,
    userStore: new WebStorageStateStore({
      store: native ? secureStore : window.localStorage,
    }),
    stateStore: new WebStorageStateStore({
      store: native ? secureStore : window.sessionStorage,
    }),
  },
  undefined,
  navigator,
);
let user: User | null = null;
const changed = () => window.dispatchEvent(new Event("session-changed"));
userManager.events.addUserLoaded((value) => {
  user = value;
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
  pendingLogin = (
    native ? userManager.signinPopup() : userManager.signinRedirect()
  ).finally(() => {
    pendingLogin = undefined;
  });
  return pendingLogin;
}
export async function logout() {
  if (native) {
    try {
      await userManager.signoutPopup();
    } finally {
      await userManager.removeUser();
      user = null;
      changed();
    }
  } else {
    await userManager.signoutRedirect();
  }
}
export const isLoggedIn = () => !!user?.access_token && !user.expired;
export const isAuthenticated = isLoggedIn;
export const getAccessToken = () => (isLoggedIn() ? user!.access_token : "");
export const signoutRedirectCallback = () =>
  userManager.signoutRedirectCallback();
export const getCurrentUser = () => ({
  id: user?.profile.sub,
  userName: user?.profile.preferred_username,
  firstName: user?.profile.given_name,
  lastName: user?.profile.family_name,
});
