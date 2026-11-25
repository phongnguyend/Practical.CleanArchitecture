import type { CapacitorConfig } from "@capacitor/cli";
import { existsSync } from "node:fs";
const plugins = [
  "@sentry/capacitor",
  "@capgo/capacitor-native-biometric",
  "@aparajita/capacitor-secure-storage",
  "@capacitor/app",
  "@capacitor/browser",
  "@capacitor/clipboard",
  "@capacitor/filesystem",
  "@capacitor/share",
];
const withFirebase = (file: string) =>
  existsSync(file) ? [...plugins, "@capacitor-firebase/crashlytics", ...(process.env.VITE_PUSH_NOTIFICATIONS_ENABLED === "true" ? ["@capacitor-firebase/messaging"] : [])] : plugins;
const config: CapacitorConfig = {
  appId: "com.classifiedads.mobile.capacitor",
  appName: "Classified Ads",
  webDir: "dist",
  experimental: { ios: { spm: { packageOptions: { "@capacitor-firebase/messaging": { symlink: true } } } } },
  android: { includePlugins: withFirebase("android/app/google-services.json") },
  ios: { includePlugins: withFirebase("ios/App/App/GoogleService-Info.plist") },
  plugins: { CapacitorHttp: { enabled: true } },
};
export default config;
