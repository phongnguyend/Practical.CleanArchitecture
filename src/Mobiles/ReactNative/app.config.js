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
  return {
    ...config,
    android: { ...config.android, ...(android ? { googleServicesFile: androidFile } : {}) },
    ios: { ...config.ios, ...(ios ? { googleServicesFile: iosFile } : {}) },
    extra: { ...config.extra, crashlytics: { android, ios } },
    plugins: [
      ...(config.plugins || []),
      ...(android || ios ? [
        "@react-native-firebase/app",
        "@react-native-firebase/crashlytics",
        ["expo-build-properties", { ios: { useFrameworks: "dynamic" } }],
      ] : []),
    ],
  };
};
