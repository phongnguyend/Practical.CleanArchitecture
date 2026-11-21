// Native Firebase modules are linked only for configured platforms.
// Rebuild the native app whenever configuration or collection mode changes.
const fs = require("node:fs");
const path = require("node:path");
const allowed = process.env.CRASHLYTICS_ENABLED !== "false";
const android = allowed && fs.existsSync(path.resolve(__dirname, process.env.FIREBASE_ANDROID_CONFIG || "google-services.json"));
const ios = allowed && fs.existsSync(path.resolve(__dirname, process.env.FIREBASE_IOS_CONFIG || "GoogleService-Info.plist"));
const platforms = { ...(android ? {} : { android: null }), ...(ios ? {} : { ios: null }) };
module.exports = {
  dependencies: {
    "@react-native-firebase/app": { platforms },
    "@react-native-firebase/crashlytics": { platforms },
  },
};
