import { App } from "@capacitor/app";
import { Browser } from "@capacitor/browser";
import type { PluginListenerHandle } from "@capacitor/core";
import type {
  INavigator,
  IWindow,
  NavigateParams,
  NavigateResponse,
} from "oidc-client-ts";
import { isAuthCallback, loginRedirect, logoutRedirect } from "./callback";

export class NativeBrowserNavigator implements INavigator {
  async callback(): Promise<void> {}
  async prepare(): Promise<IWindow> {
    let cancel: (() => void) | undefined;
    return {
      close: () => cancel?.(),
      navigate: (params: NavigateParams) =>
        new Promise<NavigateResponse>((resolve, reject) => {
          let finished = false;
          const listeners: PluginListenerHandle[] = [];
          const cleanup = () => {
            clearTimeout(timer);
            for (const listener of listeners) {
              void listener.remove();
            }
            void Browser.close().catch(() => undefined);
          };
          const fail = () => {
            if (!finished) {
              finished = true;
              cleanup();
              reject(
                new Error(
                  "Sign-in was cancelled or timed out. Please try again.",
                ),
              );
            }
          };
          cancel = fail;
          const timer = setTimeout(fail, 5 * 60 * 1000);
          void (async () => {
            listeners.push(
              await App.addListener("appUrlOpen", ({ url }) => {
                if (
                  !finished &&
                  (isAuthCallback(url, loginRedirect) ||
                    isAuthCallback(url, logoutRedirect))
                ) {
                  finished = true;
                  cleanup();
                  resolve({ url });
                }
              }),
            );
            listeners.push(await Browser.addListener("browserFinished", fail));
            if (!finished) {
              await Browser.open({ url: params.url });
            }
          })().catch(fail);
        }),
    };
  }
}
