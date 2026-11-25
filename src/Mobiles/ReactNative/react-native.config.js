// Native Firebase modules are linked only for configured platforms.
// Rebuild the native app whenever configuration or collection mode changes.
const fs = require("node:fs");
const path = require("node:path");
const allowed = process.env.CRASHLYTICS_ENABLED !== "false";
const pushFile = path.join(__dirname, "push.config.json");
const pushConfig = fs.existsSync(pushFile) ? JSON.parse(fs.readFileSync(pushFile, "utf8")) : {};
const push = process.env.PUSH_NOTIFICATIONS_ENABLED !== undefined ? process.env.PUSH_NOTIFICATIONS_ENABLED === "true" : pushConfig.enabled === true;
const android = fs.existsSync(path.resolve(__dirname, process.env.FIREBASE_ANDROID_CONFIG || "google-services.json"));
const ios = fs.existsSync(path.resolve(__dirname, process.env.FIREBASE_IOS_CONFIG || "GoogleService-Info.plist"));
const platforms = (enabled) => ({ ...(enabled && android ? {} : { android: null }), ...(enabled && ios ? {} : { ios: null }) });
module.exports = {
  dependencies: {
    "@react-native-firebase/app": { platforms: platforms(allowed || push) },
    "@react-native-firebase/crashlytics": { platforms: platforms(allowed) },
    "@react-native-firebase/messaging": { platforms: platforms(push) },
  },
};
