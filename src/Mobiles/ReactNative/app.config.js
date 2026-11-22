const fs = require("node:fs");
const path = require("node:path");

module.exports = ({ config }) => {
  const androidFile = process.env.FIREBASE_ANDROID_CONFIG || "./google-services.json";
  const iosFile = process.env.FIREBASE_IOS_CONFIG || "./GoogleService-Info.plist";
  const allowed = process.env.CRASHLYTICS_ENABLED !== "false";
  const android = allowed && fs.existsSync(path.resolve(__dirname, androidFile));
  const ios = allowed && fs.existsSync(path.resolve(__dirname, iosFile));
  if (process.env.CRASHLYTICS_ENABLED === "true" && !android && !ios) {
    throw new Error("Crashlytics was enabled but Firebase configuration is missing.");
  }
  const sentryFile = path.join(__dirname, "sentry.config.json");
  const publicConfig = fs.existsSync(sentryFile) ? JSON.parse(fs.readFileSync(sentryFile, "utf8")) : {};
  const sentry = Object.fromEntries(["SENTRY_ENABLED", "SENTRY_DSN", "SENTRY_ENVIRONMENT", "SENTRY_RELEASE"].map((key) => [key, publicConfig[key]]));
  return {
    ...config,
    android: { ...config.android, ...(android ? { googleServicesFile: androidFile } : {}) },
    ios: { ...config.ios, ...(ios ? { googleServicesFile: iosFile } : {}) },
    extra: { ...config.extra, sentry, crashlytics: { android, ios } },
    plugins: [
      ...(config.plugins || []),
      ...(sentry.SENTRY_ENABLED ? [["@sentry/react-native/expo", { organization: process.env.SENTRY_ORG, project: process.env.SENTRY_PROJECT, disableAutoUpload: process.env.SENTRY_DISABLE_AUTO_UPLOAD === "true" || !(process.env.SENTRY_AUTH_TOKEN && process.env.SENTRY_ORG && process.env.SENTRY_PROJECT) }]] : []),
      ...(android || ios ? [
        "@react-native-firebase/app",
        "@react-native-firebase/crashlytics",
        ["expo-build-properties", { ios: { useFrameworks: "dynamic" } }],
      ] : []),
    ],
  };
};
