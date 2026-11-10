const environment = {
  name: "production",
  OpenIdConnect: {
    Authority: "https://localhost:44367",
    ClientId: import.meta.env.VITE_OIDC_CLIENT_ID || "ClassifiedAds.Vue"
  },
  ResourceServer: {
    Endpoint: "https://localhost:44312/api/",
    NotificationEndpoint: "https://localhost:44312/hubs/notification"
  },
  CurrentUrl: new URL(import.meta.env.VITE_APP_URL || "http://localhost:8080/").href
};
export default environment;
