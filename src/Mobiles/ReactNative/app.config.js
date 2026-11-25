const fs = require("node:fs");
const path = require("node:path");

module.exports = ({ config }) => {
  const androidFile = process.env.FIREBASE_ANDROID_CONFIG || "./google-services.json";
  const iosFile = process.env.FIREBASE_IOS_CONFIG || "./GoogleService-Info.plist";
  const pushFile = path.join(__dirname, "push.config.json");
  const pushConfig = fs.existsSync(pushFile) ? JSON.parse(fs.readFileSync(pushFile, "utf8")) : {};
  const push = process.env.PUSH_NOTIFICATIONS_ENABLED !== undefined ? process.env.PUSH_NOTIFICATIONS_ENABLED === "true" : pushConfig.enabled === true;
  const allowed = process.env.CRASHLYTICS_ENABLED !== "false";
  const android = (allowed || push) && fs.existsSync(path.resolve(__dirname, androidFile));
  const ios = (allowed || push) && fs.existsSync(path.resolve(__dirname, iosFile));
  if (process.env.CRASHLYTICS_ENABLED === "true" && !android && !ios) {
    throw new Error("Crashlytics was enabled but Firebase configuration is missing.");
  }
  if (push && !android && !ios) {
    throw new Error("Mobile push was enabled but Firebase configuration is missing.");
  }
  const sentryFile = path.join(__dirname, "sentry.config.json");
  const publicConfig = fs.existsSync(sentryFile) ? JSON.parse(fs.readFileSync(sentryFile, "utf8")) : {};
  const sentry = Object.fromEntries(["SENTRY_ENABLED", "SENTRY_DSN", "SENTRY_ENVIRONMENT", "SENTRY_RELEASE"].map((key) => [key, publicConfig[key]]));
  return {
    ...config,
    android: { ...config.android, ...(android ? { googleServicesFile: androidFile } : {}) },
    ios: { ...config.ios, infoPlist: { ...config.ios?.infoPlist, NSFaceIDUsageDescription: "Unlock your Classified Ads session with Face ID." }, ...(ios ? { googleServicesFile: iosFile } : {}) },
    extra: { ...config.extra, sentry, push: { android: push && android, ios: push && ios }, crashlytics: { android: allowed && android, ios: allowed && ios } },
    plugins: [
      ...(config.plugins || []),
      ...(sentry.SENTRY_ENABLED ? [["@sentry/react-native/expo", { organization: process.env.SENTRY_ORG, project: process.env.SENTRY_PROJECT, disableAutoUpload: process.env.SENTRY_DISABLE_AUTO_UPLOAD === "true" || !(process.env.SENTRY_AUTH_TOKEN && process.env.SENTRY_ORG && process.env.SENTRY_PROJECT) }]] : []),
      ...(android || ios ? [
        "@react-native-firebase/app",
        ...(allowed ? ["@react-native-firebase/crashlytics"] : []),
        ...(push ? ["@react-native-firebase/messaging", ["expo-notifications", { defaultChannel: "classifiedads_updates" }]] : []),
        ["expo-build-properties", { ios: { useFrameworks: "dynamic" } }],
      ] : []),
    ],
  };
};
