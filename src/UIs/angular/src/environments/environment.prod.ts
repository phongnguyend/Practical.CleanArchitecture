export const environment = {
  production: true,
  OpenIdConnect: {
    Authority: "http://host.docker.internal:9000",
    ClientId: "ClassifiedAds.Angular",
  },
  ResourceServer: {
    Endpoint: "http://host.docker.internal:9002/api/",
    NotificationEndpoint: "http://host.docker.internal:9002/hubs/notification",
  },
  CurrentUrl: "http://localhost:4200/",
};
