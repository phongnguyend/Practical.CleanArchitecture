const environment = {
  name: "development",
  OpenIdConnect: {
    Authority: "https://localhost:44367",
    ClientId: "ClassifiedAds.React",
  },
  ResourceServer: {
    Endpoint: "https://localhost:44312/api/",
    NotificationEndpoint: "https://localhost:44312/hubs/notification",
  },
  CurrentUrl: "http://localhost:3000/",
};
export default environment;
