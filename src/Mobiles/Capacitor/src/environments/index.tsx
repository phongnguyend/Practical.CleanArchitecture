import { Capacitor } from "@capacitor/core";
const endpoint = (
  import.meta.env.VITE_API_URL || "https://localhost:44312/api/"
).replace(/\/?$/, "/");
export default {
  name: import.meta.env.MODE,
  OpenIdConnect: {
    Authority: import.meta.env.VITE_AUTHORITY || "https://localhost:44367",
    ClientId: Capacitor.isNativePlatform()
      ? "ClassifiedAds.Mobile.Capacitor"
      : import.meta.env.VITE_WEB_CLIENT_ID || "ClassifiedAds.React",
  },
  ResourceServer: {
    Endpoint: endpoint,
    NotificationEndpoint:
      import.meta.env.VITE_NOTIFICATION_URL ||
      "https://localhost:44312/hubs/notification",
  },
  CurrentUrl: window.location.origin + "/",
};
