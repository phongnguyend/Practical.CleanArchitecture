import { useEffect } from "react";
import {
  HubConnectionBuilder,
  HttpTransportType,
  HubConnectionState,
} from "@microsoft/signalr";
import { Capacitor } from "@capacitor/core";
import { App } from "@capacitor/app";
import ToastContainer, { toast } from "../Toast/ToastContainer";
import {
  getAccessToken,
  isAuthenticated,
} from "../../containers/Auth/authService";
import { updateNotifications } from "../../core/notifications";
import env from "../../environments";
export default function Notification() {
  useEffect(() => {
    if (!isAuthenticated()) {
      return;
    }
    let disposed = false;
    let active = true;
    const connection = new HubConnectionBuilder()
      .withUrl(env.ResourceServer.NotificationEndpoint, {
        accessTokenFactory: getAccessToken,
        transport: Capacitor.isNativePlatform()
          ? HttpTransportType.LongPolling
          : undefined,
      })
      .withAutomaticReconnect()
      .build();
    const start = async () => {
      if (
        disposed ||
        !active ||
        connection.state !== HubConnectionState.Disconnected
      ) {
        return;
      }
      updateNotifications("Connecting");
      try {
        await connection.start();
        if (!disposed) {
          updateNotifications("Connected");
        }
      } catch {
        if (!disposed) {
          updateNotifications("Disconnected");
        }
      }
    };
    connection.onreconnecting(() => updateNotifications("Reconnecting"));
    connection.onreconnected(() => updateNotifications("Connected"));
    connection.onclose(() => updateNotifications("Disconnected"));
    connection.on("ReceiveMessage", (message) => {
      if (!disposed) {
        updateNotifications("Connected", message);
        toast.info(
          typeof message === "string" ? message : JSON.stringify(message),
        );
      }
    });
    void start();
    const listener = App.addListener("appStateChange", ({ isActive }) => {
      active = isActive;
      if (active) {
        void start();
      } else {
        void connection.stop();
      }
    });
    const retry = setInterval(() => {
      void start();
    }, 15000);
    return () => {
      disposed = true;
      clearInterval(retry);
      void listener.then((handle) => handle.remove());
      void connection.stop();
    };
  }, []);
  return <ToastContainer />;
}
