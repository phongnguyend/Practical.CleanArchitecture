import type { CapacitorConfig } from "@capacitor/cli";
import { existsSync } from "node:fs";
const plugins = [
  "@aparajita/capacitor-secure-storage",
  "@capacitor/app",
  "@capacitor/browser",
  "@capacitor/clipboard",
  "@capacitor/filesystem",
  "@capacitor/share",
];
const withFirebase = (file: string) =>
  existsSync(file) ? [...plugins, "@capacitor-firebase/crashlytics"] : plugins;
const config: CapacitorConfig = {
  appId: "com.classifiedads.mobile.capacitor",
  appName: "Classified Ads",
  webDir: "dist",
  android: { includePlugins: withFirebase("android/app/google-services.json") },
  ios: { includePlugins: withFirebase("ios/App/App/GoogleService-Info.plist") },
  plugins: { CapacitorHttp: { enabled: true } },
};
export default config;
