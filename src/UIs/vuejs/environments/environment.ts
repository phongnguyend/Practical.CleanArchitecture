const environment = {
  name: "production",
  OpenIdConnect: {
    Authority: "https://localhost:44367",
    ClientId: "ClassifiedAds.Vue"
  },
  ResourceServer: {
    Endpoint: "https://localhost:44312/api/",
    NotificationEndpoint: "https://localhost:44312/hubs/notification"
  },
  CurrentUrl: "http://localhost:8080/"
};
export default environment;
