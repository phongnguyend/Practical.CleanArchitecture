export const config = {
  api: (
    process.env.EXPO_PUBLIC_API_URL || "https://localhost:44312/api/"
  ).replace(/\/?$/, "/"),
  issuer: process.env.EXPO_PUBLIC_AUTHORITY || "https://localhost:44367",
  hub:
    process.env.EXPO_PUBLIC_NOTIFICATION_URL ||
    "https://localhost:44312/hubs/notification",
  clientId: "ClassifiedAds.Mobile.ReactNative",
  redirect: "classifiedads.rn:/oauthredirect",
  logoutRedirect: "classifiedads.rn:/logout",
};
