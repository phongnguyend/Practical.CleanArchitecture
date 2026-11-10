import * as AuthSession from "expo-auth-session";
import * as SecureStore from "expo-secure-store";
import * as WebBrowser from "expo-web-browser";
import { config } from "../config/environment";

type Session = {
  accessToken: string;
  refreshToken?: string;
  idToken?: string;
  expiresAt: number;
};
let session: Session | null = null;
let refreshing: Promise<string> | undefined;
async function save(token: AuthSession.TokenResponse) {
  session = {
    accessToken: token.accessToken,
    refreshToken: token.refreshToken || session?.refreshToken,
    idToken: token.idToken || session?.idToken,
    expiresAt: Date.now() + (token.expiresIn || 300) * 1000,
  };
  await SecureStore.setItemAsync(
    "classifiedads.session",
    JSON.stringify(session),
  );
  return session.accessToken;
}
export async function restore() {
  const stored = await SecureStore.getItemAsync("classifiedads.session");
  session = stored ? JSON.parse(stored) : null;
  return !!session;
}
export async function login() {
  const discovery = await AuthSession.fetchDiscoveryAsync(config.issuer);
  const request = new AuthSession.AuthRequest({
    clientId: config.clientId,
    redirectUri: config.redirect,
    scopes: ["openid", "profile", "offline_access", "ClassifiedAds.WebAPI"],
    responseType: AuthSession.ResponseType.Code,
    usePKCE: true,
  });
  const response = await request.promptAsync(discovery);
  if (response.type !== "success")
    throw new Error("Sign-in was cancelled or failed.");
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
  if (!session) throw new Error("Please sign in.");
  if (session.expiresAt > Date.now() + 60000) return session.accessToken;
  if (!session.refreshToken)
    throw new Error("Session expired. Please sign out and sign in again.");
  if (!refreshing) {
    const previous = session;
    refreshing = (async () => {
      const tokens = await AuthSession.refreshAsync(
        { clientId: config.clientId, refreshToken: previous.refreshToken! },
        await AuthSession.fetchDiscoveryAsync(config.issuer),
      );
      if (session !== previous)
        throw new Error("Session changed. Please sign in again.");
      return save(tokens);
    })().finally(() => {
      refreshing = undefined;
    });
  }
  return refreshing;
}
export async function logout() {
  const idToken = session?.idToken;
  session = null;
  await SecureStore.deleteItemAsync("classifiedads.session");
  if (idToken) {
    const discovery = await AuthSession.fetchDiscoveryAsync(config.issuer);
    if (discovery.endSessionEndpoint)
      await WebBrowser.openAuthSessionAsync(
        `${discovery.endSessionEndpoint}?${new URLSearchParams({ id_token_hint: idToken, post_logout_redirect_uri: config.logoutRedirect })}`,
        config.logoutRedirect,
      );
  }
}
